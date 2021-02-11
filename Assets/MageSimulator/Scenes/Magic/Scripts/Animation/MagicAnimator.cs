using System;
using UnityEngine;
using UnityEngine.Serialization;

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
            [FormerlySerializedAs("buttonAB")] public ActiveMagicAnimation buttonAb;
        }

        public enum ActiveActionType
        {
            ButtonA,
            ButtonB,
            ButtonAb
        }

        public ActiveMagicAnimatorComposite activeAnimations;
        public Transform magicSignParent;
        public MagicSignAnimator MagicSignAnimator { get; private set; }

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

        public void SetMagicSign(MagicSignAnimator signAnimator)
        {
            if (MagicSignAnimator != null)
                Destroy(MagicSignAnimator.gameObject);
            MagicSignAnimator = Instantiate(signAnimator, magicSignParent);
        }
    }
}