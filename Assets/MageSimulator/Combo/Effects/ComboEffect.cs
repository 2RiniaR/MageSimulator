using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.Combo.Components;
using MageSimulator.Combo.Events;
using MageSimulator.Utils.Scripts;
using UniRx;
using UnityEngine;

namespace MageSimulator.Combo.Effects
{
    public class ComboEffect : MonoBehaviour
    {
        // 親のStart()が自身のStart()よりも早いタイミングで実行されたとき、Initialize()が二重実行されてしまうのでフラグを用意する
        protected bool InitializeFlag = false;
        protected bool FinalizeFlag = false;
        protected readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        public IObservable<ComboEvent> OnEventPassed => _onEventPassed;
        public IObservable<Unit> OnInitialized => _onInitialized;

        private readonly AsyncSubject<Unit> _onStart = new AsyncSubject<Unit>();
        private readonly Subject<ComboEvent> _onEventPassed = new Subject<ComboEvent>();
        private readonly Subject<Unit> _onInitialized = new Subject<Unit>();

        protected virtual void Start()
        {
            CancellationTokenSource.CancelWith(this);
            CheckInitializeOnStart();
            _onStart.OnNext(Unit.Default);
            _onStart.OnCompleted();
        }

        private void CheckInitializeOnStart()
        {
            if (transform.parent == null)
            {
                InitializeInternal(CancellationTokenSource.Token).Forget();
                return;
            }
            var parentComponent = transform.parent.GetComponentInParent<ComboComponent>();
            if (parentComponent == null || parentComponent.InitializeChildrenFlag)
            {
                InitializeInternal(CancellationTokenSource.Token).Forget();
            }
        }

        protected virtual void Initialize() { }

        public async UniTask InitializeInternal(CancellationToken token = new CancellationToken())
        {
            if (InitializeFlag) return;
            InitializeFlag = true;

            await _onStart.ToUniTask(cancellationToken: token);
            if (token.IsCancellationRequested) return;

            await UniTask.SwitchToMainThread(token);
            if (token.IsCancellationRequested) return;

            Initialize();
            _onInitialized.OnNext(Unit.Default);
        }

        protected virtual void PassEvent(ComboEvent e) { }

        public void PassEventInternal(ComboEvent e)
        {
            if (FinalizeFlag) return;
            if (e is FinishPageEvent) FinalizeFlag = true;

            PassEvent(e);
            _onEventPassed.OnNext(e);
        }
    }
}