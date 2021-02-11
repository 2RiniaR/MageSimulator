using System;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace MageSimulator.Scenes.Magic.Scripts.Animation
{
    [RequireComponent(typeof(Animator))]
    public class MagicCircle : MonoBehaviour
    {
        public TMP_Text chantText;

        public string activeParameterName;
        public string fireTriggerName;

        [SerializeField] private Animator animator;
        private ObservableStateMachineTrigger _stateMachine;

        private void Start()
        {
            _stateMachine = animator.GetBehaviour<ObservableStateMachineTrigger>();
            _stateMachine.OnStateMachineExitAsObservable().Subscribe(_ => Destroy(gameObject)).AddTo(this);
        }

        public void SetChantText(string text)
        {
            chantText.text = text;
        }

        public void Fire()
        {
            animator.SetTrigger(fireTriggerName);
        }
    }
}