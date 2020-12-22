using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using WiimoteApi;

namespace MageSimulator.Wiimote.Scripts.InputSystem
{
    using InputSystem = UnityEngine.InputSystem.InputSystem;

    [StructLayout(LayoutKind.Explicit, Size = 32)]
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

        [InputControl(layout = "Stick", processors = "stickDeadzone", displayName = "Nunchuck Stick")]
        [FieldOffset(2)]
        public Vector2 nunchuckStick;
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
        // public ButtonControl Button { get; private set; }
        // public StickControl Stick { get; private set; }

        static WiimoteDevice()
        {
            InputSystem.RegisterLayout<WiimoteDevice>();
        }

        [RuntimeInitializeOnLoadMethod]
        private static void InitializeInPlayer() {}
        public static WiimoteDevice Current { get; private set; }

        public static IReadOnlyList<WiimoteDevice> All => AllWiimotes;
        private static readonly List<WiimoteDevice> AllWiimotes = new List<WiimoteDevice>();

        public override void MakeCurrent()
        {
            base.MakeCurrent();
            Current = this;
        }

        protected override void OnAdded()
        {
            base.OnAdded();
            AllWiimotes.Add(this);
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();
            AllWiimotes.Remove(this);
        }

        protected override void FinishSetup()
        {
            base.FinishSetup();
            // Button = GetChildControl<ButtonControl>("Button");
            // Stick = GetChildControl<StickControl>("Stick");
        }

        public void OnUpdate()
        {
            var hid = description.product;
            var wiimote = WiimoteManager.Wiimotes.Find(x => x.hidapi_path == hid);
            if (wiimote == null) return;

            while (wiimote.ReadWiimoteData() > 0)
            {
                // if (wiimote.current_ext == ExtensionController.MOTIONPLUS)
                //     _wiimoteRotation.Rotate();
            }

            var state = new WiimoteDeviceState();
            ushort buttons = 0;

            buttons |= (ushort)((wiimote.Button.a ? 1 : 0) << 0);
            buttons |= (ushort)((wiimote.Button.b ? 1 : 0) << 1);
            buttons |= (ushort)((wiimote.Button.d_left ? 1 : 0) << 2);
            buttons |= (ushort)((wiimote.Button.d_right ? 1 : 0) << 3);
            buttons |= (ushort)((wiimote.Button.d_up ? 1 : 0) << 4);
            buttons |= (ushort)((wiimote.Button.d_down ? 1 : 0) << 5);
            buttons |= (ushort)((wiimote.Button.one ? 1 : 0) << 6);
            buttons |= (ushort)((wiimote.Button.two ? 1 : 0) << 7);
            buttons |= (ushort)((wiimote.Button.plus ? 1 : 0) << 8);
            buttons |= (ushort)((wiimote.Button.minus ? 1 : 0) << 9);
            buttons |= (ushort)((wiimote.Button.home ? 1 : 0) << 10);

            if (wiimote.Nunchuck != null)
            {
                buttons |= (ushort)((wiimote.Nunchuck.c ? 1 : 0) << 11);
                buttons |= (ushort)((wiimote.Nunchuck.z ? 1 : 0) << 12);
            }

            state.buttons = buttons;
            state.nunchuckStick = Vector2.zero;

            Debug.Log(string.Join(" ", Enumerable.Range(0, 16).Select(i => ((state.buttons >> i) & 1) == 1 ? "o" : "_")));
            InputSystem.QueueStateEvent(this, state);
        }
    }
}