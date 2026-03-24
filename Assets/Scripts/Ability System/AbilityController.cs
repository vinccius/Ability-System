#define USE_NEW_INPUT_SYSTEM

using System.Collections.Generic;
using UnityEngine;
using System;

#if USE_NEW_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Central MonoBehaviour. Attach to the player GameObject.
/// </summary>
public class AbilityController : MonoBehaviour
{
    public IReadOnlyList<AbilityInstance> AbilitiesList => abilitiesList;

    [Header("Skill Definitions")]
    [SerializeField] private List<AbilityDefinition> _abilityDefinitions = new();

    [Header("References")]
    [SerializeField] private Animator _characterAnimator;
    [SerializeField] private Transform _effectSpawnPoint;

#if USE_NEW_INPUT_SYSTEM
    [SerializeField] private InputActionAsset _playerInput;
#endif

    private List<AbilityInstance> abilitiesList = new();
    private Dictionary<string, AbilityInstance> abilityByAction = new();
    private AbilityInputHandler inputHandler;

    public static event Action<AbilityInstance> OnAbilityRegistered;
    public static event Action<AbilityInstance, AbilityState> OnAbilityStateChanged;
    public static event Action<AbilityInstance> OnAbilityCooldownTick;

    private void Awake()
    {
        if (_characterAnimator == null)
            _characterAnimator = GetComponent<Animator>();

        inputHandler = new AbilityInputHandler();
        inputHandler.OnInput += HandleInput;
    }

    private void Start()
    {
        RegisterAndSetupAbilities();
        InitializeInput();
    }

    private void Update()
    {
#if !USE_NEW_INPUT_SYSTEM
        _inputHandler.PollLegacyInput();
#endif
        foreach (var ability in abilitiesList)
            ability.Tick(Time.deltaTime);
    }

    private void OnDestroy()
    {
        foreach (var skill in abilitiesList)
            skill.RunTeardown();

        inputHandler.OnInput -= HandleInput;
        inputHandler.Dispose();
    }

    private void RegisterAndSetupAbilities()
    {
        foreach (var definition in _abilityDefinitions)
        {
            if (definition == null) continue;

            var instance = new AbilityInstance(definition, gameObject, _characterAnimator);

            if (definition.EffectPrefab != null && definition.PreInstantiateEffect)
            {
                var spawnParent = _effectSpawnPoint != null ? _effectSpawnPoint : transform;
                var targetEffect = Instantiate(definition.EffectPrefab, spawnParent);
                targetEffect.SetActive(false);
                instance.EffectInstance = targetEffect;
            }

            instance.OnStateChanged += (instance, state) => OnAbilityStateChanged?.Invoke(instance, state);
            instance.OnCooldownTick += (instance) => OnAbilityCooldownTick?.Invoke(instance);
            instance.RunSetup();

            abilitiesList.Add(instance);
            abilityByAction[definition.InputActionName] = instance;

            OnAbilityRegistered?.Invoke(instance);
        }
    }

    private void InitializeInput()
    {
#if USE_NEW_INPUT_SYSTEM
            var actionNames = new string[_abilityDefinitions.Count];

            for (int i = 0; i < _abilityDefinitions.Count; i++)
                actionNames[i] = _abilityDefinitions[i].InputActionName;

            inputHandler.Initialize(_playerInput, actionNames);
#else
        var keyMap = new Dictionary<string, KeyCode>();

        foreach (var definition in _abilityDefinitions)
            if (definition != null)
                keyMap[def.inputActionName] = definition.legacyKeyCode;

        _inputHandler.Initialize(keyMap);
#endif
    }

    private void HandleInput(AbilityInputEvent inputEvent)
    {
        if (!abilityByAction.TryGetValue(inputEvent.ActionName, out var ability)) return;

        var trigger = ability.Definition.TriggerType;

        switch (trigger)
        {
            case AbilityTriggerType.OnPress when inputEvent.EventType == InputEventType.Pressed:
                ability.TryActivate();
                break;

            case AbilityTriggerType.OnHold when inputEvent.EventType == InputEventType.Hold:
                ability.TryActivate();
                break;

            case AbilityTriggerType.OnHold when inputEvent.EventType == InputEventType.Released:
                ability.ForceDeactivate();
                break;

            case AbilityTriggerType.OnRelease when inputEvent.EventType == InputEventType.Released:
                ability.TryActivate();
                break;

            case AbilityTriggerType.Toggle when inputEvent.EventType == InputEventType.Pressed:
                if (ability.State == AbilityState.Active)
                    ability.ForceDeactivate();
                else
                    ability.TryActivate();
                break;
        }
    }

    /// <summary>
    /// Dynamically register a skill at runtime.
    /// </summary>
    public void RegisterSkill(AbilityDefinition definition)
    {
        _abilityDefinitions.Add(definition);
        
        var instance = new AbilityInstance(definition, gameObject, _characterAnimator);
        instance.OnStateChanged += (instance, state) => OnAbilityStateChanged?.Invoke(instance, state);
        instance.OnCooldownTick += (instance) => OnAbilityCooldownTick?.Invoke(instance);
        instance.RunSetup();
        abilitiesList.Add(instance);
        abilityByAction[definition.InputActionName] = instance;

#if USE_NEW_INPUT_SYSTEM
        inputHandler.Initialize(_playerInput, new[] { definition.InputActionName });
#else
        var keyMap = new Dictionary<string, KeyCode> { [definition.inputActionName] = definition.legacyKeyCode };
        _inputHandler.Initialize(keyMap);
#endif
        OnAbilityRegistered?.Invoke(instance);
    }

    public void CancelAbility(string abilityId)
    {
        foreach (var ability in abilitiesList)
        {
            if (ability.Definition.ID == abilityId)
            {
                ability.Cancel();
                return; 
            }
        }
    }
}