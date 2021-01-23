using System;
// using MageSimulator.Wiimote.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MageSimulator.MagicalStick
{
    public class MagicalStick : MonoBehaviour
    {
        [Serializable]
        public struct AnimatorModel
        {
            public Animator animator;
            public string isTakingPoseParameterName;
            public string isButtonPerformedParameterName;
            public string onStickSwungTriggerName;
            public string onChantedTriggerName;
        }

        public Transform rotateObject;
        public AnimatorModel animatorModel;
        public InputAction dummyChantAction;

        private void Start()
        {
            dummyChantAction.Enable();
            dummyChantAction.performed += OnDummyChanted;
        }

        private void OnDummyChanted(InputAction.CallbackContext ctx)
        {
            animatorModel.animator.SetTrigger(animatorModel.onChantedTriggerName);
        }

        private void Update()
        {
            /*
            rotateObject.localRotation = WiimoteActivator.Instance.Rotation;
            animatorModel.animator.SetBool(animatorModel.isButtonPerformedParameterName,
                WiimoteActivator.Instance.Wiimote.Button.a);
            animatorModel.animator.SetBool(animatorModel.isTakingPoseParameterName,
                WiimoteActivator.Instance.Rotation.eulerAngles.x < 45f);

            var currentStateHash = animatorModel.animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
            if (currentStateHash == Animator.StringToHash("Base Layer.ReadyToShoot") && WiimoteActivator.Instance.Rotation.eulerAngles.x > 60f)
                animatorModel.animator.SetTrigger(animatorModel.onStickSwungTriggerName);
            */
        }
    }
}