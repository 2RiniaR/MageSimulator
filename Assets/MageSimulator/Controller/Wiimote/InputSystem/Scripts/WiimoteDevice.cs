using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using WiimoteApi;

namespace MageSimulator.Controller.Wiimote.InputSystem.Scripts
{
    using InputSystem = UnityEngine.InputSystem.InputSystem;

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

        private WiimoteApi.Wiimote _wiimote;
        private Quaternion _rotation = Quaternion.identity;

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
            _wiimote = WiimoteManager.Wiimotes.Find(x => x.hidapi_path == hid);
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();
            AllWiimotes.Remove(this);
        }

        public void OnUpdate()
        {
            if (_wiimote == null) return;

            // _wiimote.ReadWiimoteData() が副作用を持っているので、whileループを削除してはいけない
            while (_wiimote.ReadWiimoteData() > 0)
            {
                if (_wiimote.current_ext == ExtensionController.MOTIONPLUS)
                    Rotate();
            }

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
            buttons |= (ushort)((_wiimote.Button.a ? 1 : 0) << 0);
            buttons |= (ushort)((_wiimote.Button.b ? 1 : 0) << 1);
            buttons |= (ushort)((_wiimote.Button.d_left ? 1 : 0) << 2);
            buttons |= (ushort)((_wiimote.Button.d_right ? 1 : 0) << 3);
            buttons |= (ushort)((_wiimote.Button.d_up ? 1 : 0) << 4);
            buttons |= (ushort)((_wiimote.Button.d_down ? 1 : 0) << 5);
            buttons |= (ushort)((_wiimote.Button.one ? 1 : 0) << 6);
            buttons |= (ushort)((_wiimote.Button.two ? 1 : 0) << 7);
            buttons |= (ushort)((_wiimote.Button.plus ? 1 : 0) << 8);
            buttons |= (ushort)((_wiimote.Button.minus ? 1 : 0) << 9);
            buttons |= (ushort)((_wiimote.Button.home ? 1 : 0) << 10);
            if (_wiimote.Nunchuck == null) return buttons;

            buttons |= (ushort)((_wiimote.Nunchuck.c ? 1 : 0) << 11);
            buttons |= (ushort)((_wiimote.Nunchuck.z ? 1 : 0) << 12);
            return buttons;
        }

        private Vector2 GetNunchuckStickInput()
        {
            return _wiimote.Nunchuck != null
                ? new Vector2(_wiimote.Nunchuck.stick[0], _wiimote.Nunchuck.stick[1])
                : Vector2.zero;
        }

        public Vector3 GetAccelVector()
        {
            var accel = _wiimote.Accel.GetCalibratedAccelData();
            return new Vector3(accel[0], -accel[2], -accel[1]).normalized;
        }

        private Vector3 GetRollVector()
        {
            if (_wiimote.MotionPlus == null)
                return Vector3.zero;
            return new Vector3(_wiimote.MotionPlus.PitchSpeed, _wiimote.MotionPlus.YawSpeed,
                _wiimote.MotionPlus.RollSpeed) / 95f;
        }

        private void Rotate()
        {
            var role = GetRollVector();
            var quaternion = Quaternion.Euler(role.x, role.y, role.z);
            _rotation *= quaternion;
        }

        public void ResetRotation()
        {
            _rotation = Quaternion.identity;
            _wiimote.MotionPlus?.SetZeroValues();
        }

        public void SetCalibrateStep(AccelCalibrationStep step)
        {
            _wiimote?.Accel?.CalibrateAccel(step);
        }
    }
}