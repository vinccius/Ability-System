using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BuildingHightlightBehaviour : MonoBehaviour, IDetectable
{
    [SerializeField] private VisualEffect _outlineVFX;

    public void SetMaterialHighlight(Material target)
    {
        
    }

    public void TriggerHightlight()
    {
        _outlineVFX.SendEvent("ShowOutline");
    }
}
