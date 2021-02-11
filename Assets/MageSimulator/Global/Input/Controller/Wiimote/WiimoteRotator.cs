namespace MageSimulator.Global.Input.Controller.Wiimote
{
    public class WiimoteRotator : WiimoteAction
    {
        private void Update()
        {
            if (WiimoteDevice != null)
                transform.rotation = WiimoteDevice.Rotation;
        }
    }
}