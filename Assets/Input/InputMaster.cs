// GENERATED AUTOMATICALLY FROM 'Assets/Input/InputMaster.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputMaster : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMaster()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMaster"",
    ""maps"": [
        {
            ""name"": ""Menu"",
            ""id"": ""9715d13e-be4c-4fed-ac81-92c165f1a69f"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Button"",
                    ""id"": ""b6af1225-be71-4d54-ab6e-e142cc7194ec"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""Submit"",
                    ""type"": ""Button"",
                    ""id"": ""f30df758-7bb3-4072-b2c5-84f7484e3a30"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""Click"",
                    ""type"": ""Button"",
                    ""id"": ""bd4502fa-8163-469b-95f3-71097ef261ee"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""Mouse Position"",
                    ""type"": ""Value"",
                    ""id"": ""a5f9dd84-e73c-4d48-b799-8baa5141df74"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""D-Pad"",
                    ""id"": ""b26d9c3b-952c-4226-8e4e-be1c05305126"",
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
                    ""id"": ""76b8a151-2255-4949-a6f5-4b67d539e9e1"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""4dfc0ce7-81f7-4d1b-93c9-7c0836563d2e"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""e6ba9680-5d13-46db-989a-ac4836c6a4d7"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""d23f7ad4-b009-487d-b6bb-facc92b7af1b"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrow Keys"",
                    ""id"": ""f001e86b-f663-430f-b9d6-91468de971da"",
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
                    ""id"": ""fa3c3cdb-7a68-4b62-b4e3-99551c60abd0"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""90b4f746-0a8c-410f-ba42-38efe0f06369"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""829b481b-5a5e-4ed3-b30b-609316feafe2"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""99ded343-7e36-408d-8d93-37b1bbe7fc71"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""b16f698f-7eb6-4446-b806-e65c235db161"",
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
                    ""id"": ""a48a0303-b7b8-44d7-851f-c6b41cf2d2f6"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""1aba964f-2bc8-4ff5-bba5-2786e983e177"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""c6831a26-e239-4bb3-85ac-0d80f74261df"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""4d648eeb-a4a8-4225-a538-bdca42b99867"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""8c9cad9d-2402-451a-8674-f8fbe68adfd5"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""16717776-9df0-43f2-a797-018b173d0566"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""029c8cee-7957-4cb4-99fb-c136a587494a"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0fffa659-8564-49d0-962c-0ccd0fad9b48"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""78b86a92-bc7b-493a-aaf7-9c8db91de9fb"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Mouse Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PlayerMovement"",
            ""id"": ""672e594b-2a40-458e-8095-96768e6ea5e4"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""cf278aff-a7f4-4a45-9a33-f86b3cecb9cf"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""882e16a2-c952-4b57-8c2d-12004160656a"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone(min=0.4)"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""6b6630c0-1976-41b3-a802-8878db7de79c"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""6d9aeb26-f389-4356-b7fe-4b25f34af722"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""b9d3ef7c-b91b-4b95-82f5-7608f9c1ed8a"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""212bfdc5-83f5-4a48-9442-802f4c2cfc32"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""3c4c9bc6-c1f3-41d3-9b6f-4af26ca77690"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrow Keys"",
                    ""id"": ""48f41c47-a0e9-4cc5-80bb-f4b7eb56d43b"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""9ca37fa5-d93f-4baf-80cc-8d242b225c38"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""0cd0a3bc-3832-40e7-bcc0-0583c36c5860"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""a0340c54-88da-4979-a69d-c6f8ba01eb0e"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""59ef043b-9004-485d-b367-27ca1dd0e678"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""PlayerCombat"",
            ""id"": ""ec4733ed-a7fd-443e-972a-42745a135f1b"",
            ""actions"": [
                {
                    ""name"": ""Punch"",
                    ""type"": ""Button"",
                    ""id"": ""b1dabd34-cc74-4a67-8378-308e0911b037"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Raise"",
                    ""type"": ""Button"",
                    ""id"": ""41026273-1866-4acf-9ccc-fd8922ffd7a4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""No Power"",
                    ""type"": ""Button"",
                    ""id"": ""964ef8e0-1d6f-4de1-af36-1aa425d1fd97"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Water Power"",
                    ""type"": ""Button"",
                    ""id"": ""29de6a34-4843-4e4b-9179-f8e1abcd5825"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e6f47dce-30fd-403b-a375-1716bf37ae42"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Punch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8f732128-24de-481d-8277-24105d233379"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Punch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""68e55889-b3d1-455e-9f43-be127963d0be"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Raise"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3575caa9-cd06-46b0-b015-ce06f113a11d"",
                    ""path"": ""<Keyboard>/k"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Raise"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9918f710-bd8b-4f7c-9b51-e58973575e3a"",
                    ""path"": ""<Keyboard>/u"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""No Power"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ac5e9507-3cc3-4a36-b22c-6bf86b0c162e"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""No Power"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b5cd684a-30d7-41a7-9174-ac727a26e169"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Water Power"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""67ef5946-2a83-491a-9c41-c0efd18ff155"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Water Power"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": true,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Menu
        m_Menu = asset.FindActionMap("Menu", throwIfNotFound: true);
        m_Menu_Move = m_Menu.FindAction("Move", throwIfNotFound: true);
        m_Menu_Submit = m_Menu.FindAction("Submit", throwIfNotFound: true);
        m_Menu_Click = m_Menu.FindAction("Click", throwIfNotFound: true);
        m_Menu_MousePosition = m_Menu.FindAction("Mouse Position", throwIfNotFound: true);
        // PlayerMovement
        m_PlayerMovement = asset.FindActionMap("PlayerMovement", throwIfNotFound: true);
        m_PlayerMovement_Movement = m_PlayerMovement.FindAction("Movement", throwIfNotFound: true);
        // PlayerCombat
        m_PlayerCombat = asset.FindActionMap("PlayerCombat", throwIfNotFound: true);
        m_PlayerCombat_Punch = m_PlayerCombat.FindAction("Punch", throwIfNotFound: true);
        m_PlayerCombat_Raise = m_PlayerCombat.FindAction("Raise", throwIfNotFound: true);
        m_PlayerCombat_NoPower = m_PlayerCombat.FindAction("No Power", throwIfNotFound: true);
        m_PlayerCombat_WaterPower = m_PlayerCombat.FindAction("Water Power", throwIfNotFound: true);
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

    // Menu
    private readonly InputActionMap m_Menu;
    private IMenuActions m_MenuActionsCallbackInterface;
    private readonly InputAction m_Menu_Move;
    private readonly InputAction m_Menu_Submit;
    private readonly InputAction m_Menu_Click;
    private readonly InputAction m_Menu_MousePosition;
    public struct MenuActions
    {
        private @InputMaster m_Wrapper;
        public MenuActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Menu_Move;
        public InputAction @Submit => m_Wrapper.m_Menu_Submit;
        public InputAction @Click => m_Wrapper.m_Menu_Click;
        public InputAction @MousePosition => m_Wrapper.m_Menu_MousePosition;
        public InputActionMap Get() { return m_Wrapper.m_Menu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
        public void SetCallbacks(IMenuActions instance)
        {
            if (m_Wrapper.m_MenuActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnMove;
                @Submit.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnSubmit;
                @Submit.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnSubmit;
                @Submit.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnSubmit;
                @Click.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnClick;
                @Click.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnClick;
                @Click.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnClick;
                @MousePosition.started -= m_Wrapper.m_MenuActionsCallbackInterface.OnMousePosition;
                @MousePosition.performed -= m_Wrapper.m_MenuActionsCallbackInterface.OnMousePosition;
                @MousePosition.canceled -= m_Wrapper.m_MenuActionsCallbackInterface.OnMousePosition;
            }
            m_Wrapper.m_MenuActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Submit.started += instance.OnSubmit;
                @Submit.performed += instance.OnSubmit;
                @Submit.canceled += instance.OnSubmit;
                @Click.started += instance.OnClick;
                @Click.performed += instance.OnClick;
                @Click.canceled += instance.OnClick;
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
            }
        }
    }
    public MenuActions @Menu => new MenuActions(this);

    // PlayerMovement
    private readonly InputActionMap m_PlayerMovement;
    private IPlayerMovementActions m_PlayerMovementActionsCallbackInterface;
    private readonly InputAction m_PlayerMovement_Movement;
    public struct PlayerMovementActions
    {
        private @InputMaster m_Wrapper;
        public PlayerMovementActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_PlayerMovement_Movement;
        public InputActionMap Get() { return m_Wrapper.m_PlayerMovement; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerMovementActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerMovementActions instance)
        {
            if (m_Wrapper.m_PlayerMovementActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMovement;
            }
            m_Wrapper.m_PlayerMovementActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
            }
        }
    }
    public PlayerMovementActions @PlayerMovement => new PlayerMovementActions(this);

    // PlayerCombat
    private readonly InputActionMap m_PlayerCombat;
    private IPlayerCombatActions m_PlayerCombatActionsCallbackInterface;
    private readonly InputAction m_PlayerCombat_Punch;
    private readonly InputAction m_PlayerCombat_Raise;
    private readonly InputAction m_PlayerCombat_NoPower;
    private readonly InputAction m_PlayerCombat_WaterPower;
    public struct PlayerCombatActions
    {
        private @InputMaster m_Wrapper;
        public PlayerCombatActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Punch => m_Wrapper.m_PlayerCombat_Punch;
        public InputAction @Raise => m_Wrapper.m_PlayerCombat_Raise;
        public InputAction @NoPower => m_Wrapper.m_PlayerCombat_NoPower;
        public InputAction @WaterPower => m_Wrapper.m_PlayerCombat_WaterPower;
        public InputActionMap Get() { return m_Wrapper.m_PlayerCombat; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerCombatActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerCombatActions instance)
        {
            if (m_Wrapper.m_PlayerCombatActionsCallbackInterface != null)
            {
                @Punch.started -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnPunch;
                @Punch.performed -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnPunch;
                @Punch.canceled -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnPunch;
                @Raise.started -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnRaise;
                @Raise.performed -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnRaise;
                @Raise.canceled -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnRaise;
                @NoPower.started -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnNoPower;
                @NoPower.performed -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnNoPower;
                @NoPower.canceled -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnNoPower;
                @WaterPower.started -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnWaterPower;
                @WaterPower.performed -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnWaterPower;
                @WaterPower.canceled -= m_Wrapper.m_PlayerCombatActionsCallbackInterface.OnWaterPower;
            }
            m_Wrapper.m_PlayerCombatActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Punch.started += instance.OnPunch;
                @Punch.performed += instance.OnPunch;
                @Punch.canceled += instance.OnPunch;
                @Raise.started += instance.OnRaise;
                @Raise.performed += instance.OnRaise;
                @Raise.canceled += instance.OnRaise;
                @NoPower.started += instance.OnNoPower;
                @NoPower.performed += instance.OnNoPower;
                @NoPower.canceled += instance.OnNoPower;
                @WaterPower.started += instance.OnWaterPower;
                @WaterPower.performed += instance.OnWaterPower;
                @WaterPower.canceled += instance.OnWaterPower;
            }
        }
    }
    public PlayerCombatActions @PlayerCombat => new PlayerCombatActions(this);
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    public interface IMenuActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnSubmit(InputAction.CallbackContext context);
        void OnClick(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
    }
    public interface IPlayerMovementActions
    {
        void OnMovement(InputAction.CallbackContext context);
    }
    public interface IPlayerCombatActions
    {
        void OnPunch(InputAction.CallbackContext context);
        void OnRaise(InputAction.CallbackContext context);
        void OnNoPower(InputAction.CallbackContext context);
        void OnWaterPower(InputAction.CallbackContext context);
    }
}
