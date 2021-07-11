using System;
using MageSimulator.Combo.Effects;
using MageSimulator.Scenes.Magic.Scripts.Animation;

namespace MageSimulator.Scenes.Magic.Scripts.Combo
{
    public class MagicAnimationEffect : ComboEffect
    {
        public enum Trigger
        {
            Activate,
            Fire,
            Cancel
        }

        public Trigger trigger;
        public MagicCircle activateCircle;

        private MagicAnimator _animator;

        protected override void Start()
        {
            _animator = FindObjectOfType<MagicAnimator>();
            base.Start();
        }

        protected override void Initialize()
        {
            switch (trigger)
            {
                case Trigger.Activate:
                    Activate();
                    break;
                case Trigger.Fire:
                    Fire();
                    break;
                case Trigger.Cancel:
                    Cancel();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Activate()
        {
            _animator.SetMagicSign(activateCircle);
        }

        private void Fire()
        {
            if (_animator.MagicCircle != null)
                _animator.MagicCircle.Fire();
        }

        private void Cancel()
        {
            if (_animator.MagicCircle != null)
                _animator.MagicCircle.Cancel();
        }
    }
}