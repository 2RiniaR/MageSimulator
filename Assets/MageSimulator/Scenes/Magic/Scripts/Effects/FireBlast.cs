using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace MageSimulator.Scenes.Magic.Scripts.Effects
{
    [RequireComponent(typeof(ParticleSystem))]
    public class FireBlast : MonoBehaviour
    {
        private ParticleSystem _particleSystem;
        private CancellationTokenSource _cancellationTokenSource;

        private void OnDisable()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        private void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.RegisterRaiseCancelOnDestroy(this);
            _particleSystem = GetComponent<ParticleSystem>();
        }
    }
}