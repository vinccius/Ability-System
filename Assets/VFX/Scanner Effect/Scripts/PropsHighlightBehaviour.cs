using System.Collections;
using UnityEditor;
using UnityEngine;

public class PropsHighlightBehaviour : MonoBehaviour, IDetectable
{
    [SerializeField] private Material _highlightMaterial;
    [SerializeField] private Renderer[] _allObjectRenderers;

    private MaterialPropertyBlock propertyBlock;
    private Coroutine hightlightCoroutine;

    private readonly string materialProperty = "_Highlight_Progress";

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    public void SetMaterialHighlight(Material target)
    {
        
    }

    public void TriggerHightlight()
    {
        if (hightlightCoroutine != null)
            StopCoroutine(hightlightCoroutine);

        hightlightCoroutine = StartCoroutine(EnableHighlight());
    }

    private IEnumerator EnableHighlight()
    {
        if (_highlightMaterial == null)
        {
            Debug.LogError("Highlight material is not assigned. Returning");
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
        foreach (var renderer in _allObjectRenderers)
        {
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(materialProperty, value);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }
}
