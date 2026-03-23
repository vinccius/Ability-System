using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyHighlightBehaviour : MonoBehaviour, IDetectable
{
    public Material highlightMaterial;

    private MaterialSwitch materialSwitch;
    private Coroutine hightlightCoroutine;
    private MaterialPropertyBlock propertyBlock;
    private List<SkinnedMeshRenderer> enemyRenderers = new();

    private readonly string materialProperty = "_Highlight_Progress";

    private void Awake()
    {
        materialSwitch = GetComponent<MaterialSwitch>();
        propertyBlock = new MaterialPropertyBlock();

        if (materialSwitch != null)
            enemyRenderers = materialSwitch.GetRenderers().ToList();
    }

    public void SetMaterialHighlight(Material target)
    {
        highlightMaterial = target;
    }

    public void TriggerHightlight()
    {
        if (hightlightCoroutine != null)
            StopCoroutine(hightlightCoroutine);

        hightlightCoroutine = StartCoroutine(EnableEnemyHighlight());
    }

    private IEnumerator EnableEnemyHighlight()
    {
        if (highlightMaterial == null)
        {
            Debug.LogError("Enemy highlight material is not assigned. Returning");
            yield break;
        }

        float duration = 2f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float lerpValue = Mathf.Lerp(1f, 0f, time / duration);

            ApplyHighlightValue(lerpValue);

            yield return null;
        }

        ApplyHighlightValue(0f);
    }

    private void ApplyHighlightValue(float value)
    {
        foreach (var renderer in enemyRenderers)
        {
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(materialProperty, value);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }
}
