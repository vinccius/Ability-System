using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scanner Behaviour", menuName = "SkillSystem/Behaviours/Scanner")]
public class ScannerBehaviour : AbilityBehaviour
{
    [Range(1f, 30f)]
    public float ScannerRange;

    private ScannerTrigger scanner;

    public override void OnSetup(AbilityInstance skill)
    {
        if (skill.EffectInstance != null)
        {
            scanner = skill.EffectInstance.GetComponent<ScannerTrigger>();
            scanner.SetupScanner(ScannerRange, skill.Definition.Duration);
        }

        Debug.Log($"[Scanner] Setup complete. Effect ready: {skill.EffectInstance != null}");
    }

    public override void OnPrepare(AbilityInstance skill)
    {
        Debug.Log($"[Scanner] Preparing Scanner");
    }

    public override void OnActivate(AbilityInstance skill)
    {
        if (scanner != null)
        {
            scanner.gameObject.SetActive(true);
            scanner.StartScan();
        }

        Debug.Log($"[Scanner] Scanner enabled!");
    }

    public override void OnDeactivate(AbilityInstance skill)
    {
        Debug.Log($"[Scanner] Scanner disabled");
    }

    public override void OnCancel(AbilityInstance skill)
    {
        Debug.Log($"[Scanner] Canceled.");
    }
}
