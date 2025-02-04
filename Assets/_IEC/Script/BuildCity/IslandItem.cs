using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Animations;

public class IslandItem : MonoBehaviour
{
    public RequierdScrewBuildCity panelInfoItem;
    public List<MeshRenderer> itemMaterial;
    public Animator animItem;
    public List<ParticleSystem> listEffectComplete;
    public static string FillAmount = "_FillAmount";
    public void BuildingCompleteEffect()
    {
        panelInfoItem.EffectComplete();
        foreach (var particle in listEffectComplete)
        {
            particle.Play();
        }
    }
    public void SetFloatMaterialItemIsland(float percentage)
    {
        foreach (var mat in itemMaterial)
        {
            mat.material.SetFloat(FillAmount,percentage);
        }
    }
    public Vector3 GetCenterObject()
    {
        return itemMaterial[0].bounds.center;
    }
    public void StopAnimBuilding(bool stt)
    {
       if(animItem) animItem.enabled = stt;
    }
    
}
