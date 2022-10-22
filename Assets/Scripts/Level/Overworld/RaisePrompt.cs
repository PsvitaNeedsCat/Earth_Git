using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RaisePrompt : MonoBehaviour
{
    private Image m_renderer = null;
    private Vector2 m_analogDirection = Vector2.zero;

    private void Awake()
    {
        m_renderer = GetComponent<Image>();
    }

    // Add and remove listener for player moving the right analog stick
    private void OnEnable()
    {
        //PlayerInput.s_controls.PlayerCombat.Target.performed += ctx => SetAnalogDirection(ctx.ReadValue<Vector2>());
        //PlayerInput.s_controls.PlayerCombat.Target.canceled += ctx => SetAnalogDirection(ctx.ReadValue<Vector2>());
        PlayerInput.s_controls.PlayerCombat.KeyboardTarget.performed += ctx => SetAnalogDirection(ctx.ReadValue<Vector2>());
        PlayerInput.s_controls.PlayerCombat.KeyboardTarget.canceled += ctx => SetAnalogDirection(ctx.ReadValue<Vector2>());

        SetAnalogDirection(Vector2.zero);
    }
    private void OnDisable()
    {
        //PlayerInput.s_controls.PlayerCombat.Target.performed -= ctx => SetAnalogDirection(ctx.ReadValue<Vector2>());
        //PlayerInput.s_controls.PlayerCombat.Target.canceled -= ctx => SetAnalogDirection(ctx.ReadValue<Vector2>());
        PlayerInput.s_controls.PlayerCombat.KeyboardTarget.performed -= ctx => SetAnalogDirection(ctx.ReadValue<Vector2>());
        PlayerInput.s_controls.PlayerCombat.KeyboardTarget.canceled -= ctx => SetAnalogDirection(ctx.ReadValue<Vector2>());
    }

    private void SetAnalogDirection(Vector2 _direction)
    {
        m_renderer.enabled = _direction.magnitude > 0.0f;
    }
}
