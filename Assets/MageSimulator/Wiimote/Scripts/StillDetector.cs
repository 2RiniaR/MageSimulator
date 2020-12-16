using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MageSimulator.Wiimote.Scripts
{
    public class StillDetector
    {
        public int HistoryLength { get; set; }
        private Vector3 _previousAccel = Vector3.zero;
        private readonly List<Vector3> _accelDiffHistory = new List<Vector3>();

        public StillDetector(int historyLength)
        {
            HistoryLength = historyLength;
        }

        /// <summary>
        ///     加速度の変化量の平均を取得する
        /// </summary>
        public float AccelMagnitudeAverage => _accelDiffHistory.Average(x => x.magnitude);

        /// <summary>
        ///     変化量の更新
        /// </summary>
        public void Update()
        {
            var accel = WiimoteActivator.Instance.GetAccelVector();
            _accelDiffHistory.Add(accel - _previousAccel);
            if (_accelDiffHistory.Count > HistoryLength)
                _accelDiffHistory.RemoveRange(0, _accelDiffHistory.Count - HistoryLength);
            _previousAccel = accel;
        }
    }
}