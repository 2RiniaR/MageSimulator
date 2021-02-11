using UnityEngine;
using UnityEngine.EventSystems;

namespace MageSimulator.Combo.Effects
{
    public class Focus : ComboEffect
    {
        public enum FocusActivateTiming
        {
            None,
            Start,
            InitializeComponent,
        }

        public GameObject target;
        public FocusActivateTiming activateTiming = FocusActivateTiming.InitializeComponent;

        protected override void Start()
        {
            if (activateTiming == FocusActivateTiming.Start)
                SetSelectingObject();
            base.Start();
        }

        protected override void Initialize()
        {
            if (activateTiming == FocusActivateTiming.InitializeComponent)
                SetSelectingObject();
        }

        public void SetSelectingObject()
        {
            EventSystem.current.SetSelectedGameObject(target);
        }
    }
}
