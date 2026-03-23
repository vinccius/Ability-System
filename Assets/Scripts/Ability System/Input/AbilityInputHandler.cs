// Comment the line below if you have not the New Input System package installed:
#define USE_NEW_INPUT_SYSTEM

using System;
using System.Collections.Generic;
using UnityEngine;

#if USE_NEW_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Abstracts Unity's New Input System and Legacy Input System behind a common interface.
/// Toggle USE_NEW_INPUT_SYSTEM define to switch.
/// </summary>
public class AbilityInputHandler : IDisposable
{
    public event Action<AbilityInputEvent> OnInput;

#if USE_NEW_INPUT_SYSTEM
    private InputActionAsset playerInput;
    private Dictionary<string, InputAction> actions = new();

    public void Initialize(InputActionAsset playerInput, string[] actionNames)
    {
        this.playerInput = playerInput;

        foreach (var name in actionNames)
        {
            var action = playerInput.FindAction(name, false);
            if (action == null) continue;

            var captured = name;
            action.started += _ => Raise(captured, InputEventType.Pressed);
            action.performed += _ => Raise(captured, InputEventType.Hold);
            action.canceled += _ => Raise(captured, InputEventType.Released);
            action.Enable();
            actions[name] = action;
        }
    }
#else
    private Dictionary<string, KeyCode> keyMap = new()

    public void Initialize(System.Collections.Generic.Dictionary<string, KeyCode> keyMap)
    {
        this.keyMap = keyMap;
    }

    /// <summary>
    /// Must be called from MonoBehaviour.Update()
    /// </summary>
    public void PollLegacyInput()
    {
        foreach (var kvp in keyMap)
        {
            if (Input.GetKeyDown(kvp.Value))
                Raise(kvp.Key, InputEventType.Pressed);
            else if (Input.GetKey(kvp.Value))
                Raise(kvp.Key, InputEventType.Hold);
            else if (Input.GetKeyUp(kvp.Value))
                Raise(kvp.Key, InputEventType.Released);
        }
    }
#endif

    private void Raise(string actionName, InputEventType type)
    {
        var inputEvent = new AbilityInputEvent 
        { 
            ActionName = actionName, 
            EventType = type 
        };

        OnInput?.Invoke(inputEvent);
    }

    public void Dispose()
    {
#if USE_NEW_INPUT_SYSTEM
            foreach (var action in actions.Values) action.Disable();
            actions.Clear();
#else
        keyMap.Clear();
#endif
    }
}
