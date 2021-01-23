using System;
using System.Linq;

namespace MageSimulator.DataCollection.Scripts
{
    public static class SoundStatisticsHelper
    {
        public static float GetAverageLoss(float[] data1, float[] data2)
        {
            return Enumerable.Zip(data1, data2, (d1, d2) => Math.Abs(d1 - d2))
                .Average();
        }
    }
}