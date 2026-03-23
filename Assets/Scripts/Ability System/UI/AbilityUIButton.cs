using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls the visual state of a single skill button.
/// </summary>
public class AbilityUIButton : MonoBehaviour
{
    [Header("Wired automatically by SkillHUDManager")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _cooldownOverlay;
    [SerializeField] private TextMeshProUGUI _keyLabel;
    [SerializeField] private TextMeshProUGUI _nameLabel;
    [SerializeField] private Image _background;

    [Header("Colors")]
    [SerializeField] private Color _colorIdle = Color.white;
    [SerializeField] private Color _colorPrepare = new Color(1f, 0.9f, 0.2f);
    [SerializeField] private Color _colorActive = new Color(0.2f, 1f, 0.4f);
    [SerializeField] private Color _colorCooldown = new Color(0.4f, 0.4f, 0.4f);

    private AbilityInstance ability;

    public void Initialize(AbilityInstance ability)
    {
        this.ability = ability;
        var definition = ability.Definition;

        if (_iconImage != null && definition.Icon != null)
            _iconImage.sprite = definition.Icon;

        if (_nameLabel != null)
            _nameLabel.text = definition.DisplayName;

        if (_keyLabel != null)
            _keyLabel.text = definition.ShortcutKeyName;

        if (_cooldownOverlay != null)
            _cooldownOverlay.fillAmount = 0f;

        SetBackground(_colorIdle);
    }

    public void OnStateChanged(SkillState state)
    {
        switch (state)
        {
            case SkillState.Idle:
                SetBackground(_colorIdle);
                if (_cooldownOverlay) _cooldownOverlay.fillAmount = 0f;
                break;
            case SkillState.Preparing:
                SetBackground(_colorPrepare);
                break;
            case SkillState.Active:
                SetBackground(_colorActive);
                break;
            case SkillState.Cooldown:
                SetBackground(_colorCooldown);
                break;
            case SkillState.Cancelled:
                SetBackground(_colorIdle);
                break;
        }
    }

    public void UpdateCooldown(float remaining, float total)
    {
        if (_cooldownOverlay == null || total <= 0f) return;
        _cooldownOverlay.fillAmount = remaining / total;
    }

    private void SetBackground(Color c)
    {
        if (_background != null) _background.color = c;
    }
}
