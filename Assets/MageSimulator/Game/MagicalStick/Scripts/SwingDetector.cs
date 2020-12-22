using System.Collections.Generic;

namespace MageSimulator.MagicalStick
{
    public class SwingDetector
    {
        private const float SwingPitchPerSecondThreshold = 150f;
        private readonly Queue<float> _pitchHistory = new Queue<float>();

        public void Update()
        {

        }
    }
}