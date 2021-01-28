using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace MageSimulator.Utils.Scripts
{
    public class WindowAnimator : MonoBehaviour
    {
        [Serializable]
        private struct StateComposite
        {
            public string windowLayerName;
            public string hidden;
            public string opening;
            public string displayed;
            public string closing;
        }

        [Serializable]
        private struct TriggerComposite
        {
            public string open;
            public string close;
        }

        public IObservable<Unit> OnOpenRequested => _onOpenRequested;
        public IObservable<Unit> OnOpened => _onOpened;
        public IObservable<Unit> OnCloseRequested => _onCloseRequested;
        public IObservable<Unit> OnClosed => _onClosed;
        [SerializeField] private StateComposite states;
        [SerializeField] private TriggerComposite triggers;
        private readonly Subject<Unit> _onOpenRequested = new Subject<Unit>();
        private readonly Subject<Unit> _onOpened = new Subject<Unit>();
        private readonly Subject<Unit> _onCloseRequested = new Subject<Unit>();
        private readonly Subject<Unit> _onClosed = new Subject<Unit>();
        private Animator _animator;
        private ObservableStateMachineTrigger _stateMachine;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _stateMachine = _animator.GetBehaviour<ObservableStateMachineTrigger>();
            if (_animator == null)
                Debug.LogWarning("Animator が存在しません");
            if (_stateMachine == null)
                Debug.LogWarning("Animator に ObservableStateMachineTrigger が存在しません");
        }

        private void Start()
        {
            _stateMachine.OnStateEnterAsObservable()
                .Where(x => x.StateInfo.IsName(states.opening))
                .AsUnitObservable()
                .Subscribe(_onOpenRequested)
                .AddTo(this);

            _stateMachine.OnStateEnterAsObservable()
                .Where(x => x.StateInfo.IsName(states.displayed))
                .AsUnitObservable()
                .Subscribe(_onOpened)
                .AddTo(this);

            _stateMachine.OnStateEnterAsObservable()
                .Where(x => x.StateInfo.IsName(states.closing))
                .AsUnitObservable()
                .Subscribe(_onCloseRequested)
                .AddTo(this);

            _stateMachine.OnStateEnterAsObservable()
                .Where(x => x.StateInfo.IsName(states.hidden))
                .AsUnitObservable()
                .Subscribe(_onClosed)
                .AddTo(this);
        }

        public UniTask OpenAsync(CancellationToken token = new CancellationToken())
        {
            if (!_animator.GetCurrentAnimatorStateInfo(_animator.GetLayerIndex(states.windowLayerName)).IsName(states.hidden))
                return UniTask.CompletedTask;
            _animator.SetTrigger(triggers.open);
            return OnOpened.First().ToUniTask(cancellationToken: token);
        }

        public UniTask CloseAsync(CancellationToken token = new CancellationToken())
        {
            if (!_animator.GetCurrentAnimatorStateInfo(_animator.GetLayerIndex(states.windowLayerName)).IsName(states.displayed))
                return UniTask.CompletedTask;
            _animator.SetTrigger(triggers.close);
            return OnClosed.First().ToUniTask(cancellationToken: token);
        }
    }
}