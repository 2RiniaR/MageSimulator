using System;
using System.Collections.Generic;
using MageSimulator.Config.Scripts;
using MageSimulator.Interactive.Scripts;
using MageSimulator.Wiimote.Calibrator.Scripts.MotionDetector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MageSimulator.Wiimote.Calibrator.Scripts
{
    public class WiimoteCalibrator : MonoBehaviour
    {
        [Serializable]
        public struct DetectorComposite
        {
            public Step1MotionDetector step1;
            public Step2MotionDetector step2;
            public Step3MotionDetector step3;
            public Step4MotionDetector step4;
        }

        [Serializable]
        public struct AnimationComposite
        {
            public Animator animator;
            public string isStayingAnimatorPropertyName;
            public string onRetryTriggerName;
            public string onOkTriggerName;
            public string onForceApplyTriggerName;
            public string onCancelTriggerName;
        }

        [Serializable]
        public struct ActionComposite
        {
            public InputAction cancelAction;
            public InputAction forceApplyAction;
        }

        public AnimationComposite animations;
        public DetectorComposite detectors;
        public ActionComposite actions;
        public ItemSelectPage confirmPage;

        private ApplicationSettings _applicationSettings;
        private MotionDetector.MotionDetector _currentDetector;
        private Dictionary<MotionType, Func<MotionDetector.MotionDetector>> _typeDetectorMap;

        private void Awake()
        {
            _applicationSettings = Resources.Load<ApplicationSettings>("ApplicationSettings");
            _typeDetectorMap = new Dictionary<MotionType, Func<MotionDetector.MotionDetector>>()
            {
                {MotionType.Step1, () => detectors.step1},
                {MotionType.Step2, () => detectors.step2},
                {MotionType.Step3, () => detectors.step3},
                {MotionType.Step4, () => detectors.step4},
            };
        }

        private void OnEnable()
        {
            actions.forceApplyAction.Enable();
            actions.cancelAction.Enable();
            actions.forceApplyAction.performed += OnForceApplied;
            actions.cancelAction.performed += OnCanceled;
        }

        private void OnDisable()
        {
            actions.cancelAction.performed -= OnCanceled;
            actions.forceApplyAction.performed -= OnForceApplied;
            actions.cancelAction.Disable();
            actions.forceApplyAction.Disable();
        }

        private void Start()
        {
            if (confirmPage != null)
                confirmPage.onMoveToNext.AddListener(OnSubmitConfirm);
        }

        public void DeactivateDetector()
        {
            if (_currentDetector == null) return;
            _currentDetector.onConditionChanged.RemoveListener(OnMotionConditionChanged);
            _currentDetector = null;
        }

        public void SetDetector(MotionType type)
        {
            if (!_typeDetectorMap.TryGetValue(type, out var detectorGetter))
                return;
            var detector = detectorGetter();
            if (detector == null || _currentDetector == detector)
                return;

            DeactivateDetector();
            _currentDetector = detector;
            OnMotionConditionChanged(false);
            _currentDetector.onConditionChanged.AddListener(OnMotionConditionChanged);
            _currentDetector.SetDevice(_applicationSettings.wiimotePid);
        }

        private void OnMotionConditionChanged(bool isCondition)
        {
            animations.animator.SetBool(animations.isStayingAnimatorPropertyName, isCondition);
        }

        private void OnSubmitConfirm()
        {
            switch (confirmPage.selectingItem)
            {
                case 0:
                    animations.animator.SetTrigger(animations.onOkTriggerName);
                    break;
                case 1:
                    animations.animator.SetTrigger(animations.onRetryTriggerName);
                    break;
            }
        }

        private void OnForceApplied(InputAction.CallbackContext ctx)
        {
            animations.animator.SetTrigger(animations.onForceApplyTriggerName);
        }

        private void OnCanceled(InputAction.CallbackContext ctx)
        {
            animations.animator.SetTrigger(animations.onCancelTriggerName);
        }
    }
}
