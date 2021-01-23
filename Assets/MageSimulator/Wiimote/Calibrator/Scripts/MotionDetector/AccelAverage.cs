using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MageSimulator.Wiimote.Calibrator.Scripts.MotionDetector
{
    public class AccelAverage
    {
        public int HistoryLength = 10;
        private Vector3 _previousAccel = Vector3.zero;
        private readonly List<Vector3> _accelDiffHistory = new List<Vector3>();

        public void Update(Vector3 accel)
        {
            _accelDiffHistory.Add(accel - _previousAccel);
            if (_accelDiffHistory.Count > HistoryLength)
                _accelDiffHistory.RemoveRange(0, _accelDiffHistory.Count - HistoryLength);
            _previousAccel = accel;
        }

        public float GetMagnitude()
        {
            return _accelDiffHistory.Count == 0 ? 0f : _accelDiffHistory.Average(x => x.magnitude);
        }
    }
}