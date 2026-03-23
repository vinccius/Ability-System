using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;

/// <summary>
/// Example: Shield Skill
/// - OnSetup: caches the pre-instantiated shield effect
/// - OnActivate: enables the shield mesh/VFX
/// - OnDeactivate: disables it
/// 
/// Create via: Assets > Create > SkillSystem > Behaviours > ShieldBehaviour
/// Assign to SkillDefinition.behaviourOverride
/// </summary>
[CreateAssetMenu(fileName = "ShieldBehaviour", menuName = "SkillSystem/Behaviours/Shield")]
public class ShieldBehaviour : AbilityBehaviour
{
    [Tooltip("Optional damage reduction while shield is active (0-1)")]
    [Range(0f, 1f)]
    public float damageReduction = 0.5f;

    [SerializeField] private float _dissolveSpeed;

    private Renderer rendererComponent;
    private readonly string dissolveValuePropertyName = "_dissolve";
    private CancellationTokenSource cts;

    public override void OnSetup(AbilityInstance skill)
    {
        // Effect is already pre-instantiated by SkillController.
        // Just make sure it starts hidden.
        if (skill.EffectInstance != null)
        {
            rendererComponent = skill.EffectInstance.GetComponent<Renderer>();
            rendererComponent.material.SetFloat(dissolveValuePropertyName, 1f);
            skill.EffectInstance.SetActive(false);
        }

        Debug.Log($"[Shield] Setup complete. Effect ready: {skill.EffectInstance != null}");
    }

    public override void OnPrepare(AbilityInstance skill)
    {
        Debug.Log("[Shield] Raising shield...");
    }

    public async override void OnActivate(AbilityInstance skill)
    {
        if (skill.EffectInstance != null)
        {
            skill.EffectInstance.SetActive(true);
            await EnableShield();
        }

        // Example: notify health/damage system via interface (decoupled)
        var damageable = skill.OwnerObject.GetComponent<IDamageable>();
        damageable?.SetDamageReduction(damageReduction);

        Debug.Log("[Shield] Active!");
    }

    public async override void OnDeactivate(AbilityInstance skill)
    {
        if (skill.EffectInstance != null)
        {
            await DisableShield();
            skill.EffectInstance.SetActive(false);
        }
        var damageable = skill.OwnerObject.GetComponent<IDamageable>();
        damageable?.SetDamageReduction(0f);

        Debug.Log("[Shield] Deactivated.");
    }

    public override void OnCancel(AbilityInstance skill)
    {
        OnDeactivate(skill);
        Debug.Log("[Shield] Cancelled.");
    }

    private async Task EnableShield()
    {
        if (cts is not null)
            cts.Cancel();

        cts = new CancellationTokenSource();

        await Task.Delay(500);

        float startValue = rendererComponent.material.GetFloat(dissolveValuePropertyName);

        for (float index = 0f; index < 1f; index += (_dissolveSpeed / 100f))
        {
            if (cts.IsCancellationRequested) return;

            rendererComponent.material.SetFloat(dissolveValuePropertyName, Mathf.Lerp(startValue, 0, index));
            await Task.Yield();
        }

        rendererComponent.material.SetFloat(dissolveValuePropertyName, 0f);
    }

    private async Task DisableShield()
    {
        if (cts is not null)
            cts.Cancel();

        cts = new CancellationTokenSource();

        float startValue = rendererComponent.material.GetFloat(dissolveValuePropertyName);

        for (float index = 0f; index < 1f; index += (_dissolveSpeed / 100f))
        {
            if (cts.IsCancellationRequested) return;

            rendererComponent.material.SetFloat(dissolveValuePropertyName, Mathf.Lerp(startValue, 1, index));
            await Task.Yield();
        }

        rendererComponent.material.SetFloat(dissolveValuePropertyName, 1f);
    }
}

// Decoupled interface — your health system implements this, no direct dependency
public interface IDamageable
{
    void SetDamageReduction(float amount);
}
