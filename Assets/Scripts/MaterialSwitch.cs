using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MaterialSwitch : MonoBehaviour
{
    [SerializeField] private Material[] _targetMaterial;

    [SerializeField] private SkinnedMeshRenderer[] _meshRenderers;

    private void Start()
    {
        _meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void Update()
    {
        if (_targetMaterial is not null)
        {
            foreach (var renderer in _meshRenderers)
            {
                renderer.materials = _targetMaterial;
            }
        }
    }

    public void SetMaterial(Material material)
    {
        _targetMaterial[0] = material;
    }

    public Material GetMaterial()
    {
        return _targetMaterial[0];
    }

    public SkinnedMeshRenderer[] GetRenderers()
    {
        return _meshRenderers;
    }
}
