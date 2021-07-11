using System;
using UnityEngine;

namespace MageSimulator.Scenes.Magic.Scripts.Animation
{
    public class MagicAnimator : MonoBehaviour
    {
        [Serializable]
        public struct ActiveMagicAnimation
        {
            public Animator animator;
            public string propertyName;
        }

        [Serializable]
        public struct ActiveMagicAnimatorComposite
        {
            public ActiveMagicAnimation buttonA;
            public ActiveMagicAnimation buttonB;
            public ActiveMagicAnimation buttonAb;
        }

        public enum ActiveActionType
        {
            ButtonA,
            ButtonB,
            ButtonAb
        }

        public ActiveMagicAnimatorComposite activeAnimations;
        public Transform magicSignParent;
        public MagicCircle MagicCircle { get; private set; }

        public void SetActiveAction(ActiveActionType type, bool isActive)
        {
            switch (type)
            {
                case ActiveActionType.ButtonA:
                    SetActiveAction(activeAnimations.buttonA, isActive);
                    break;
                case ActiveActionType.ButtonB:
                    SetActiveAction(activeAnimations.buttonB, isActive);
                    break;
                case ActiveActionType.ButtonAb:
                    SetActiveAction(activeAnimations.buttonAb, isActive);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static void SetActiveAction(ActiveMagicAnimation anim, bool isActive)
        {
            anim.animator.SetBool(anim.propertyName, isActive);
        }

        public void SetMagicSign(MagicCircle circle)
        {
            MagicCircle = Instantiate(circle, magicSignParent);
        }
    }
}