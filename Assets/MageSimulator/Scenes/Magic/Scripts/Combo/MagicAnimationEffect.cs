using System;
using MageSimulator.Combo.Effects;
using MageSimulator.Combo.Events;
using MageSimulator.Scenes.Magic.Scripts.Animation;
using MageSimulator.VoiceRecognition.SpeakIndicator.Scripts;

namespace MageSimulator.Scenes.Magic.Scripts.Combo
{
    public class MagicAnimationEffect : ComboEffect
    {
        public enum Trigger
        {
            Activate,
            Chanted,
            Fire,
            Deactivate
        }

        public Trigger trigger;
        public MagicSignAnimator activateSignAnimator;

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
                case Trigger.Chanted:
                    Chanted();
                    break;
                case Trigger.Fire:
                    Fire();
                    break;
                case Trigger.Deactivate:
                    Deactivate();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Activate()
        {
            _animator.SetMagicSign(activateSignAnimator);
            _animator.MagicSignAnimator.SetActive(true);
        }

        private void Chanted()
        {
            if (_animator.MagicSignAnimator != null)
                _animator.MagicSignAnimator.Chanted();
        }

        private void Fire()
        {
            if (_animator.MagicSignAnimator != null)
                _animator.MagicSignAnimator.Fire();
        }

        private void Deactivate()
        {
            if (_animator.MagicSignAnimator != null)
                _animator.MagicSignAnimator.SetActive(false);
        }
    }
}