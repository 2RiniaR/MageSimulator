using System;
using UnityEngine;

namespace MageSimulator.MagicalStick
{
    public class MagicalStick : MonoBehaviour
    {
        [Serializable]
        public struct AnimatorModel
        {
            public Animator animator;
            public string startChargeTriggerName;
            public string cancelChargeTriggerName;
            public string attackTriggerName;
        }
    }
}