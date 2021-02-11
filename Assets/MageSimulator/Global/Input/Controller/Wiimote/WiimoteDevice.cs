using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using MageSimulator.Global.Input.Controller.Wiimote.MotionDetector;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using WiimoteApi;

namespace MageSimulator.Global.Input.Controller.Wiimote
{
    [StructLayout(LayoutKind.Explicit, Size = 38)]
    public struct WiimoteDeviceState : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('W', 'I', 'M', 'T');

        [InputControl(name = "A", layout = "Button", bit = (int)WiimoteButton.A)]
        [InputControl(name = "B", layout = "Button", bit = (int)WiimoteButton.B)]
        [InputControl(name = "DLeft", layout = "Button", bit = (int)WiimoteButton.DLeft)]
        [InputControl(name = "DRight", layout = "Button", bit = (int)WiimoteButton.DRight)]
        [InputControl(name = "DUp", layout = "Button", bit = (int)WiimoteButton.DUp)]
        [InputControl(name = "DDown", layout = "Button", bit = (int)WiimoteButton.DDown)]
        [InputControl(name = "One", layout = "Button", bit = (int)WiimoteButton.One)]
        [InputControl(name = "Two", layout = "Button", bit = (int)WiimoteButton.Two)]
        [InputControl(name = "Plus", layout = "Button", bit = (int)WiimoteButton.Plus)]
        [InputControl(name = "Minus", layout = "Button", bit = (int)WiimoteButton.Minus)]
        [InputControl(name = "Home", layout = "Button", bit = (int)WiimoteButton.Home)]
        [InputControl(name = "C", layout = "Button", bit = (int)WiimoteButton.C)]
        [InputControl(name = "Z", layout = "Button", bit = (int)WiimoteButton.Z)]
        [FieldOffset(0)]
        public ushort buttons;

        [InputControl(layout = "Stick", processors = "stickDeadzone", name = "Nunchuck Stick")]
        [FieldOffset(2)]
        public Vector2 nunchuckStick;

        [InputControl(layout = "Vector3", name = "Accelerometer")]
        [FieldOffset(10)]
        public Vector3 acceleration;

        [InputControl(layout = "Quaternion", name = "Rotation")]
        [FieldOffset(22)]
        public Quaternion rotation;
    }

    public enum WiimoteButton
    {
        A,
        B,
        DLeft,
        DRight,
        DUp,
        DDown,
        One,
        Two,
        Plus,
        Minus,
        Home,
        C,
        Z
    }

    #if UNITY_EDITOR
        [InitializeOnLoad]
    #endif
    [InputControlLayout(displayName = "Wiimote", stateType = typeof(WiimoteDeviceState))]
    public class WiimoteDevice : InputDevice, IInputUpdateCallbackReceiver
    {
        static WiimoteDevice()
        {
            InputSystem.RegisterLayout<WiimoteDevice>();
        }

        [RuntimeInitializeOnLoadMethod]
        private static void InitializeInPlayer() {}
        public static WiimoteDevice Current { get; private set; }

        public static IReadOnlyList<WiimoteDevice> All => AllWiimotes;
        private static readonly List<WiimoteDevice> AllWiimotes = new List<WiimoteDevice>();

        public WiimoteApi.Wiimote Wiimote { get; private set; }
        private Quaternion _rotation = Quaternion.identity;
        public Quaternion Rotation => _rotation;
        private readonly AccelAverage _accelAverage = new AccelAverage();

        public override void MakeCurrent()
        {
            base.MakeCurrent();
            Current = this;
        }

        protected override void OnAdded()
        {
            base.OnAdded();
            AllWiimotes.Add(this);
            var hid = description.product;
            Wiimote = WiimoteManager.Wiimotes.Find(x => x.hidapi_path == hid);

            if (AllWiimotes.Count == 1)
                Current = this;
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();
            AllWiimotes.Remove(this);
        }

        public void OnUpdate()
        {
            if (Wiimote == null) return;

            // Wiimote.ReadWiimoteData() が副作用を持っているので、whileループを削除してはいけない
            while (Wiimote.ReadWiimoteData() > 0)
            {
                if (Wiimote.current_ext == ExtensionController.MOTIONPLUS)
                    Rotate();
            }

            _accelAverage.Update(GetAccelVector());
            if (_accelAverage.GetMagnitude() < 0.1)
                _rotation = Quaternion.RotateTowards(_rotation, GetAdjustedRotationFromAccel(), 10f * Time.deltaTime);

            var state = new WiimoteDeviceState
            {
                buttons = GetButtonInput(),
                nunchuckStick = GetNunchuckStickInput(),
                acceleration = GetAccelVector(),
                rotation = _rotation
            };

            InputSystem.QueueStateEvent(this, state);
        }

        private ushort GetButtonInput()
        {
            ushort buttons = 0;
            buttons |= (ushort)((Wiimote.Button.a ? 1 : 0) << 0);
            buttons |= (ushort)((Wiimote.Button.b ? 1 : 0) << 1);
            buttons |= (ushort)((Wiimote.Button.d_left ? 1 : 0) << 2);
            buttons |= (ushort)((Wiimote.Button.d_right ? 1 : 0) << 3);
            buttons |= (ushort)((Wiimote.Button.d_up ? 1 : 0) << 4);
            buttons |= (ushort)((Wiimote.Button.d_down ? 1 : 0) << 5);
            buttons |= (ushort)((Wiimote.Button.one ? 1 : 0) << 6);
            buttons |= (ushort)((Wiimote.Button.two ? 1 : 0) << 7);
            buttons |= (ushort)((Wiimote.Button.plus ? 1 : 0) << 8);
            buttons |= (ushort)((Wiimote.Button.minus ? 1 : 0) << 9);
            buttons |= (ushort)((Wiimote.Button.home ? 1 : 0) << 10);
            if (Wiimote.Nunchuck == null) return buttons;

            buttons |= (ushort)((Wiimote.Nunchuck.c ? 1 : 0) << 11);
            buttons |= (ushort)((Wiimote.Nunchuck.z ? 1 : 0) << 12);
            return buttons;
        }

        private Vector2 GetNunchuckStickInput()
        {
            return Wiimote.Nunchuck != null
                ? new Vector2(Wiimote.Nunchuck.stick[0], Wiimote.Nunchuck.stick[1])
                : Vector2.zero;
        }

        public Vector3 GetAccelVector()
        {
            var accel = Wiimote.Accel.GetCalibratedAccelData();
            return new Vector3(accel[0], -accel[2], accel[1]).normalized;
        }

        private Vector3 GetRollVector()
        {
            if (Wiimote.MotionPlus == null)
                return Vector3.zero;
            return new Vector3(Wiimote.MotionPlus.PitchSpeed, -Wiimote.MotionPlus.YawSpeed,
                Wiimote.MotionPlus.RollSpeed) / 95f;
        }

        private Quaternion GetAdjustedRotationFromAccel()
        {
            var accel = GetAccelVector();
            var rot = Quaternion.FromToRotation(accel, Vector3.down);
            var roll = _rotation * Quaternion.Inverse(rot);
            return Quaternion.AngleAxis(roll.eulerAngles.y, Vector3.up) * rot;
        }

        private void Rotate()
        {
            var role = GetRollVector();
            var quaternion = Quaternion.Euler(role.x, role.y, role.z);
            _rotation *= quaternion;
        }

        public void ResetRotation()
        {
            ResetRotation(Quaternion.identity);
        }

        public void ResetRotation(Quaternion rotation)
        {
            _rotation = rotation;
            Wiimote.MotionPlus?.SetZeroValues();
            Debug.Log($"Wiimote({name}) Calibrated: MOTION_PLUS");
        }

        public void SetCalibrateStep(AccelCalibrationStep step)
        {
            Wiimote?.Accel?.CalibrateAccel(step);
            Debug.Log($"Wiimote({name}) Calibrated: {step.ToString()}");
        }

        public void SetRumble(bool isActive)
        {
            Wiimote.RumbleOn = isActive;
            Wiimote.SendWithType(OutputDataType.STATUS_INFO_REQUEST, new byte[] { 0x00 });
        }
    }
}