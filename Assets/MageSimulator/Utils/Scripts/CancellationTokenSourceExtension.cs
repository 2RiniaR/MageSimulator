using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace MageSimulator.Utils.Scripts
{
    public static class CancellationTokenSourceExtension
    {
        public static void CancelWith(this CancellationTokenSource cts, Component component)
        {
            cts.CancelWith(component.gameObject);
        }

        public static void CancelWith(this CancellationTokenSource cts, GameObject gameObject)
        {
            gameObject.OnDestroyAsync().ContinueWith(cts.Cancel).Forget();
        }
    }
}