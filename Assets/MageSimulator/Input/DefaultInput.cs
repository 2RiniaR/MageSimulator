// GENERATED AUTOMATICALLY FROM 'Assets/MageSimulator/Input/DefaultInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace MageSimulator.Input
{
    public class @DefaultInput : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @DefaultInput()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""DefaultInput"",
    ""maps"": [
        {
            ""name"": ""PlayerDebug"",
            ""id"": ""7d1c868c-3928-4f83-914f-bb251e675266"",
            ""actions"": [
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""982b9ea8-ee7a-471d-b494-71d0ea432e3d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""8235c77e-d061-4f45-8119-e745310d025c"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // PlayerDebug
            m_PlayerDebug = asset.FindActionMap("PlayerDebug", throwIfNotFound: true);
            m_PlayerDebug_Attack = m_PlayerDebug.FindAction("Attack", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // PlayerDebug
        private readonly InputActionMap m_PlayerDebug;
        private IPlayerDebugActions m_PlayerDebugActionsCallbackInterface;
        private readonly InputAction m_PlayerDebug_Attack;
        public struct PlayerDebugActions
        {
            private @DefaultInput m_Wrapper;
            public PlayerDebugActions(@DefaultInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @Attack => m_Wrapper.m_PlayerDebug_Attack;
            public InputActionMap Get() { return m_Wrapper.m_PlayerDebug; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerDebugActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerDebugActions instance)
            {
                if (m_Wrapper.m_PlayerDebugActionsCallbackInterface != null)
                {
                    @Attack.started -= m_Wrapper.m_PlayerDebugActionsCallbackInterface.OnAttack;
                    @Attack.performed -= m_Wrapper.m_PlayerDebugActionsCallbackInterface.OnAttack;
                    @Attack.canceled -= m_Wrapper.m_PlayerDebugActionsCallbackInterface.OnAttack;
                }
                m_Wrapper.m_PlayerDebugActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Attack.started += instance.OnAttack;
                    @Attack.performed += instance.OnAttack;
                    @Attack.canceled += instance.OnAttack;
                }
            }
        }
        public PlayerDebugActions @PlayerDebug => new PlayerDebugActions(this);
        public interface IPlayerDebugActions
        {
            void OnAttack(InputAction.CallbackContext context);
        }
    }
}
