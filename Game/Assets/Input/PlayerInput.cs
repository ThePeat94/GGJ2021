// GENERATED AUTOMATICALLY FROM 'Assets/Input/PlayerInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""MainGame"",
            ""id"": ""31318777-1404-478a-926d-2557c940dbe7"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""d35e711b-4713-44a7-9a73-b71533b22e7f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Mouse"",
                    ""type"": ""Value"",
                    ""id"": ""7a216d67-5a73-4d64-8d76-090aa16f4b90"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""783d84a5-4a55-40f7-bfef-eb77f2cafe88"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Button"",
                    ""id"": ""e0bdfa27-c099-4d2a-abfe-814fe7efd368"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Reload"",
                    ""type"": ""Button"",
                    ""id"": ""6ff90f81-aacf-4891-856f-e166fc10ba5c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Quit"",
                    ""type"": ""Button"",
                    ""id"": ""84b0682a-c666-430e-8b40-49d9d1d88b63"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Keys"",
                    ""id"": ""2c4be5f8-52f7-4252-ba6b-d85c26c19fd6"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""0fc86b36-f813-410a-85a4-72e731b2a40d"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""e970f0da-b3f8-4097-90f0-fa89e4eef2c6"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""ceac21aa-fd2b-4241-bff6-951da8d8b2e6"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""c7e000f5-c8df-4eea-b9a8-d92364b9312a"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""ede2a7c1-207d-4bf4-a628-e583109b3a45"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Mouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""85d90f13-f605-4790-9b70-9f6fd93b1644"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bf0d1967-4eaf-430f-998d-2641e80d9db4"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c7b75a6e-5cf0-4f46-8e58-515a64557aa8"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ec9f2d7e-14ca-4cc1-977a-fe3be8dfc3bc"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Quit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // MainGame
        m_MainGame = asset.FindActionMap("MainGame", throwIfNotFound: true);
        m_MainGame_Move = m_MainGame.FindAction("Move", throwIfNotFound: true);
        m_MainGame_Mouse = m_MainGame.FindAction("Mouse", throwIfNotFound: true);
        m_MainGame_Attack = m_MainGame.FindAction("Attack", throwIfNotFound: true);
        m_MainGame_Aim = m_MainGame.FindAction("Aim", throwIfNotFound: true);
        m_MainGame_Reload = m_MainGame.FindAction("Reload", throwIfNotFound: true);
        m_MainGame_Quit = m_MainGame.FindAction("Quit", throwIfNotFound: true);
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

    // MainGame
    private readonly InputActionMap m_MainGame;
    private IMainGameActions m_MainGameActionsCallbackInterface;
    private readonly InputAction m_MainGame_Move;
    private readonly InputAction m_MainGame_Mouse;
    private readonly InputAction m_MainGame_Attack;
    private readonly InputAction m_MainGame_Aim;
    private readonly InputAction m_MainGame_Reload;
    private readonly InputAction m_MainGame_Quit;
    public struct MainGameActions
    {
        private @PlayerInput m_Wrapper;
        public MainGameActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_MainGame_Move;
        public InputAction @Mouse => m_Wrapper.m_MainGame_Mouse;
        public InputAction @Attack => m_Wrapper.m_MainGame_Attack;
        public InputAction @Aim => m_Wrapper.m_MainGame_Aim;
        public InputAction @Reload => m_Wrapper.m_MainGame_Reload;
        public InputAction @Quit => m_Wrapper.m_MainGame_Quit;
        public InputActionMap Get() { return m_Wrapper.m_MainGame; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MainGameActions set) { return set.Get(); }
        public void SetCallbacks(IMainGameActions instance)
        {
            if (m_Wrapper.m_MainGameActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_MainGameActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_MainGameActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_MainGameActionsCallbackInterface.OnMove;
                @Mouse.started -= m_Wrapper.m_MainGameActionsCallbackInterface.OnMouse;
                @Mouse.performed -= m_Wrapper.m_MainGameActionsCallbackInterface.OnMouse;
                @Mouse.canceled -= m_Wrapper.m_MainGameActionsCallbackInterface.OnMouse;
                @Attack.started -= m_Wrapper.m_MainGameActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_MainGameActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_MainGameActionsCallbackInterface.OnAttack;
                @Aim.started -= m_Wrapper.m_MainGameActionsCallbackInterface.OnAim;
                @Aim.performed -= m_Wrapper.m_MainGameActionsCallbackInterface.OnAim;
                @Aim.canceled -= m_Wrapper.m_MainGameActionsCallbackInterface.OnAim;
                @Reload.started -= m_Wrapper.m_MainGameActionsCallbackInterface.OnReload;
                @Reload.performed -= m_Wrapper.m_MainGameActionsCallbackInterface.OnReload;
                @Reload.canceled -= m_Wrapper.m_MainGameActionsCallbackInterface.OnReload;
                @Quit.started -= m_Wrapper.m_MainGameActionsCallbackInterface.OnQuit;
                @Quit.performed -= m_Wrapper.m_MainGameActionsCallbackInterface.OnQuit;
                @Quit.canceled -= m_Wrapper.m_MainGameActionsCallbackInterface.OnQuit;
            }
            m_Wrapper.m_MainGameActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Mouse.started += instance.OnMouse;
                @Mouse.performed += instance.OnMouse;
                @Mouse.canceled += instance.OnMouse;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @Aim.started += instance.OnAim;
                @Aim.performed += instance.OnAim;
                @Aim.canceled += instance.OnAim;
                @Reload.started += instance.OnReload;
                @Reload.performed += instance.OnReload;
                @Reload.canceled += instance.OnReload;
                @Quit.started += instance.OnQuit;
                @Quit.performed += instance.OnQuit;
                @Quit.canceled += instance.OnQuit;
            }
        }
    }
    public MainGameActions @MainGame => new MainGameActions(this);
    public interface IMainGameActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnMouse(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnReload(InputAction.CallbackContext context);
        void OnQuit(InputAction.CallbackContext context);
    }
}
