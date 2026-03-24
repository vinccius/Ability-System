using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scanner Behaviour", menuName = "Ability System/Behaviours/Scanner")]
public class ScannerBehaviour : AbilityBehaviour
{
    [Range(1f, 30f)]
    public float ScannerRange;

    private ScannerTrigger scanner;

    public override void OnSetup(AbilityInstance ability)
    {
        if (ability.EffectInstance != null)
        {
            scanner = ability.EffectInstance.GetComponent<ScannerTrigger>();
            scanner.SetupScanner(ScannerRange, ability.Definition.Duration);
        }

        Debug.Log($"[Scanner] Setup complete. Effect ready: {ability.EffectInstance != null}");
    }

    public override void OnPrepare(AbilityInstance ability)
    {
        Debug.Log($"[Scanner] Preparing Scanner");
    }

    public override void OnActivate(AbilityInstance ability)
    {
        if (scanner != null)
        {
            scanner.gameObject.SetActive(true);
            scanner.StartScan();
        }

        Debug.Log($"[Scanner] Scanner enabled!");
    }

    public override void OnDeactivate(AbilityInstance ability)
    {
        Debug.Log($"[Scanner] Scanner disabled");
    }

    public override void OnCancel(AbilityInstance ability)
    {
        Debug.Log($"[Scanner] Canceled.");
    }
}
