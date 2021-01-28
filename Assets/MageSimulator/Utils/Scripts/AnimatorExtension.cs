using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;

namespace MageSimulator.Utils.Scripts
{
    public static class AnimatorExtension
    {
        public static UniTask CompleteOnAnimationFinish(this ObservableStateMachineTrigger stateMachine, int layerIndex,
            string stateName, CancellationToken token = new CancellationToken())
        {
            return stateMachine.OnStateEnterAsObservable()
                .Where(x => x.LayerIndex == layerIndex && x.StateInfo.IsName(stateName))
                .First()
                .ToUniTask(cancellationToken: token);
        }
    }
}