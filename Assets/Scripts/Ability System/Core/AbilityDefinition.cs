using UnityEngine;

/// <summary>
/// ScriptableObject that defines a skill's static data.
/// Create via: Assets > Create > SkillSystem > SkillDefinition
/// </summary>
[CreateAssetMenu(fileName = "NewSkill", menuName = "SkillSystem/SkillDefinition")]
public class AbilityDefinition : ScriptableObject
{
    [Header("Ability Data")]
    public string SkillId;
    public string DisplayName;
    [TextArea] public string Description;

    [Header("UI")]
    public Sprite Icon;
    public string ShortcutKeyName;

    [Header("Input")]
    public string InputActionName;         // New Input System action name
    public KeyCode LegacyInputKeyCode;          // Legacy Input System key
    public AbilityTriggerType TriggerType = AbilityTriggerType.OnPress;

    [Header("Timing")]
    public float CastTime = 0f;           // Preparation time before activation
    public float Duration = 0f;           // 0 = instant
    public float Cooldown = 1f;

    [Header("Animations")]
    public AnimatorOverrideController AnimationOverride;

    [Header("Prefabs & Effects")]
    public GameObject EffectPrefab;       // Instantiated on setup (e.g. shield VFX)
    public bool PreInstantiateEffect = true;

    [Header("Skill Behaviour")]
    public AbilityBehaviour BehaviourOverride; // Optional: custom MonoBehaviour logic
}
