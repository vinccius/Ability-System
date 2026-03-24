using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;

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

    public override void OnSetup(AbilityInstance ability)
    {
        if (ability.EffectInstance != null)
        {
            rendererComponent = ability.EffectInstance.GetComponent<Renderer>();
            rendererComponent.material.SetFloat(dissolveValuePropertyName, 1f);
            ability.EffectInstance.SetActive(false);
        }

        Debug.Log($"[Shield] Setup complete. Effect ready: {ability.EffectInstance != null}");
    }

    public override void OnPrepare(AbilityInstance ability)
    {
        Debug.Log("[Shield] Raising shield...");
    }

    public async override void OnActivate(AbilityInstance ability)
    {
        if (ability.EffectInstance != null)
        {
            ability.EffectInstance.SetActive(true);
            await EnableShield();
        }

        var damageable = ability.OwnerObject.GetComponent<IDamageable>();
        damageable?.SetDamageReduction(damageReduction);

        Debug.Log("[Shield] Active!");
    }

    public async override void OnDeactivate(AbilityInstance ability)
    {
        if (ability.EffectInstance != null)
        {
            await DisableShield();
            ability.EffectInstance.SetActive(false);
        }
        var damageable = ability.OwnerObject.GetComponent<IDamageable>();
        damageable?.SetDamageReduction(0f);

        Debug.Log("[Shield] Deactivated.");
    }

    public override void OnCancel(AbilityInstance ability)
    {
        OnDeactivate(ability);
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

public interface IDamageable
{    void SetDamageReduction(float amount);
}
