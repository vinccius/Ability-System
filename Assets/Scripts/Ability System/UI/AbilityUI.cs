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
    [SerializeField] private GameObject _skillButtonPrefab; // Prefab with SkillButtonUI component

    private Dictionary<string, AbilityUIButton> buttons = new();

    private void OnEnable()
    {
        SkillController.OnSkillRegistered += HandleSkillRegistered;
        SkillController.OnSkillStateChanged += HandleStateChanged;
        SkillController.OnSkillCooldownTick += HandleCooldownTick;
    }

    private void OnDisable()
    {
        SkillController.OnSkillRegistered -= HandleSkillRegistered;
        SkillController.OnSkillStateChanged -= HandleStateChanged;
        SkillController.OnSkillCooldownTick -= HandleCooldownTick;
    }

    private void HandleSkillRegistered(AbilityInstance ability)
    {
        if (buttons.ContainsKey(ability.Definition.SkillId)) return;

        var go = Instantiate(_skillButtonPrefab, _buttonContainer);
        var btn = go.GetComponent<AbilityUIButton>();

        if (btn == null)
            btn = go.AddComponent<AbilityUIButton>();

        btn.Initialize(ability);
        buttons[ability.Definition.SkillId] = btn;
    }

    private void HandleStateChanged(AbilityInstance ability, SkillState state)
    {
        if (buttons.TryGetValue(ability.Definition.SkillId, out var btn))
            btn.OnStateChanged(state);
    }

    private void HandleCooldownTick(AbilityInstance ability)
    {
        if (buttons.TryGetValue(ability.Definition.SkillId, out var btn))
            btn.UpdateCooldown(ability.CooldownRemaining, ability.Definition.Cooldown);
    }
}
