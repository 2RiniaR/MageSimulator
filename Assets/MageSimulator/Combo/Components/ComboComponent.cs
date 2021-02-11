using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using MageSimulator.Combo.Effects;
using MageSimulator.Combo.Events;
using UnityEngine;

namespace MageSimulator.Combo.Components
{
    [DisallowMultipleComponent]
    public abstract class ComboComponent : ComboEffect
    {
        public bool InitializeChildrenFlag { get; private set; } = false;

        protected override void Initialize()
        {
            InitializeEffects();
            InitializeChildren();
        }

        protected void InitializeEffects()
        {
            foreach (var effect in GetComponents<ComboEffect>())
            {
                if (effect != this)
                    effect.InitializeInternal(CancellationTokenSource.Token).Forget();
            }
        }

        protected void InitializeChildren()
        {
            foreach (var comp in SearchChildrenComponent(transform))
                comp.InitializeInternal(CancellationTokenSource.Token).Forget();

            InitializeChildrenFlag = true;
        }

        private static IEnumerable<ComboComponent> SearchChildrenComponent(Transform current)
        {
            return Enumerable.Range(0, current.childCount).Select(current.GetChild).SelectMany(child =>
            {
                var component = child.GetComponent<ComboComponent>();
                return component != null ? new[] {component} : SearchChildrenComponent(child);
            });
        }

        protected override void PassEvent(ComboEvent e)
        {
            PassEventEffects(e);
            PublishEvent(e);
        }

        protected void PassEventEffects(ComboEvent e)
        {
            foreach (var effect in GetComponents<ComboEffect>())
                if (effect != this) effect.PassEventInternal(e);
        }

        protected void PublishEvent(ComboEvent e)
        {
            if (transform.parent == null) return;
            var parentComponent = transform.parent.GetComponentInParent<ComboComponent>();
            if (parentComponent == null) return;
            parentComponent.PassEventInternal(e);
        }
    }
}