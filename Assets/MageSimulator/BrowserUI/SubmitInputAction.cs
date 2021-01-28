using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MageSimulator.BrowserUI
{
    public class SubmitInputAction : Link
    {
        public InputAction linkAction;

        private void OnEnable()
        {
            linkAction.Enable();
        }

        private void OnDisable()
        {
            linkAction.Disable();
        }

        protected override UniTask OnInitializeComponent(CancellationToken token = new CancellationToken())
        {
            Observable.FromEvent<InputAction.CallbackContext>(
                    h => linkAction.performed += h,
                    h => linkAction.performed -= h)
                .Subscribe(OnLinkRequested).AddTo(this);
            return UniTask.CompletedTask;
        }

        private void OnLinkRequested(InputAction.CallbackContext ctx)
        {
            Request();
        }
    }
}