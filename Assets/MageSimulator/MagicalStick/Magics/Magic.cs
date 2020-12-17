using UnityEngine;

namespace MageSimulator.MagicalStick.Magics
{
    public abstract class Magic : MonoBehaviour
    {
        public abstract bool IsTakingChantingPose();
        public abstract bool IsButtonPerformed();
    }
}
