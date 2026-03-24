using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Listens to SkillController's static events and automatically spawns
/// a SkillButtonUI for every registered skill.
/// </summary>
public class AbilityUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform _buttonContainer;    // Horizontal/Vertical Layout Group
    [SerializeField] private GameObject _abilityButtonPrefab; // Prefab with SkillButtonUI component

    private Dictionary<string, AbilityUIButton> buttons = new();

    private void OnEnable()
    {
        AbilityController.OnAbilityRegistered += HandleAbilityRegistered;
        AbilityController.OnAbilityStateChanged += HandleStateChanged;
        AbilityController.OnAbilityCooldownTick += HandleCooldownTick;
    }

    private void OnDisable()
    {
        AbilityController.OnAbilityRegistered -= HandleAbilityRegistered;
        AbilityController.OnAbilityStateChanged -= HandleStateChanged;
        AbilityController.OnAbilityCooldownTick -= HandleCooldownTick;
    }

    private void HandleAbilityRegistered(AbilityInstance ability)
    {
        if (buttons.ContainsKey(ability.Definition.ID)) return;

        var go = Instantiate(_abilityButtonPrefab, _buttonContainer);
        var btn = go.GetComponent<AbilityUIButton>();

        if (btn == null)
            btn = go.AddComponent<AbilityUIButton>();

        btn.Initialize(ability);
        buttons[ability.Definition.ID] = btn;
    }

    private void HandleStateChanged(AbilityInstance ability, AbilityState state)
    {
        if (buttons.TryGetValue(ability.Definition.ID, out var btn))
            btn.OnStateChanged(state);
    }

    private void HandleCooldownTick(AbilityInstance ability)
    {
        if (buttons.TryGetValue(ability.Definition.ID, out var btn))
            btn.UpdateCooldown(ability.CooldownRemaining, ability.Definition.Cooldown);
    }
}
