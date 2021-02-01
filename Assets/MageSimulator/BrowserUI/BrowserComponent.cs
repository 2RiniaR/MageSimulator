using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MageSimulator.BrowserUI.Events;
using UniRx;
using UnityEngine;

namespace MageSimulator.BrowserUI
{
    [DisallowMultipleComponent]
    public abstract class BrowserComponent : MonoBehaviour
    {
        private bool _initializeChildrenFlag = false;
        private bool _finalizeFlag = false;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly AsyncSubject<Unit> _onStart = new AsyncSubject<Unit>();

        protected virtual void Start()
        {
            _cancellationTokenSource.CancelWith(this);
            CheckInitializeOnStart();
            _onStart.OnNext(Unit.Default);
            _onStart.OnCompleted();
        }

        private void CheckInitializeOnStart()
        {
            if (transform.parent == null)
            {
                Initialize();
                return;
            }
            var parentComponent = transform.parent.GetComponentInParent<BrowserComponent>();
            if (parentComponent == null || parentComponent._initializeChildrenFlag)
                Initialize();
        }

        protected virtual void Initialize()
        {
            InitializeChildren();
        }

        private async UniTask InitializeInternal(CancellationToken token = new CancellationToken())
        {
            await _onStart.ToUniTask(cancellationToken: token);
            if (token.IsCancellationRequested) return;
            await UniTask.SwitchToMainThread(token);
            Initialize();
        }

        protected void InitializeChildren()
        {
            foreach (var comp in SearchChildrenComponent(transform))
                comp.InitializeInternal(_cancellationTokenSource.Token).Forget();
            _initializeChildrenFlag = true;
        }

        public static IEnumerable<BrowserComponent> SearchChildrenComponent(Transform current)
        {
            return Enumerable.Range(0, current.childCount).Select(current.GetChild).SelectMany(child =>
            {
                var component = child.GetComponent<BrowserComponent>();
                return component != null ? new[] {component} : SearchChildrenComponent(child);
            });
        }

        protected virtual void PassEvent(BrowserEvent e)
        {
            PublishEvent(e);
        }

        private void PassEventInternal(BrowserEvent e)
        {
            if (_finalizeFlag) return;
            if (e is FinishPageEvent)
            {
                Debug.Log("Finalize: " + name);
                _finalizeFlag = true;
            }
            PassEvent(e);
        }

        protected void PublishEvent(BrowserEvent e)
        {
            if (transform.parent == null) return;
            var parentComponent = transform.parent.GetComponentInParent<BrowserComponent>();
            if (parentComponent == null) return;
            parentComponent.PassEventInternal(e);
        }
    }
}