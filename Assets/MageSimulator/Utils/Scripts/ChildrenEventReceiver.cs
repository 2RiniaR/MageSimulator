using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace MageSimulator.Utils.Scripts
{
    public class ChildrenEventReceiver : MonoBehaviour
    {
        public UnityEvent onSubmit;
        public UnityEvent onCancel;

        private void Start()
        {
            var triggers = GetComponentsInChildren<ObservableEventTrigger>();

            triggers.Select(x => x.OnSubmitAsObservable())
                .Merge()
                .Subscribe(_ => onSubmit.Invoke())
                .AddTo(this);

            triggers.Select(x => x.OnCancelAsObservable())
                .Merge()
                .Subscribe(_ => onCancel.Invoke())
                .AddTo(this);

            // 子要素の変更を監視していないので、監視できるようにするべき
            // this.OnTransformChildrenChangedAsObservable()
        }
    }
}