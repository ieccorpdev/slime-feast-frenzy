using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using IEC;
using UnityEngine;
using UnityEngine.Animations;

public class IslandItem : MonoBehaviour
{
    public RequierdScrewBuildCity panelInfoItem;
    public List<MeshRenderer> itemMaterial;
    public Animator animItem;
    public List<ParticleSystem> listEffectComplete;
    public static string FillAmount = "_FillAmount";
    private string FILL_ID => $"fill_id_{gameObject.GetInstanceID()}"; 
    public void BuildingCompleteEffect()
    {
        panelInfoItem.EffectComplete();
        EventController.RaiseEvent(EventList.Audio.COMPLETE_BUILDING);
        foreach (var particle in listEffectComplete)
        {
            particle.Play();
        }
    }
    public void SetFloatMaterialItemIsland(float percentage)
    {
        DOTween.Kill(FILL_ID);
        foreach (var mat in itemMaterial)
        {
            mat.material.DOFloat(percentage,FillAmount,0.1f).SetId(FILL_ID);
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

    public void SelfUpdate()
    {
        panelInfoItem = null;
        itemMaterial = new List<MeshRenderer>();
        listEffectComplete = new List<ParticleSystem>();
        panelInfoItem = gameObject.transform.GetComponentInChildren<RequierdScrewBuildCity>();
        itemMaterial.Add(GetComponent<MeshRenderer>());
        foreach (Transform child in transform)
        {
            if (child.GetComponent<ParticleSystem>() != null)
            {
                listEffectComplete.Add(child.GetComponent<ParticleSystem>());
            }
        }
    }

    public void OnDestroy()
    {
        DOTween.Kill(FILL_ID);
    }
}
