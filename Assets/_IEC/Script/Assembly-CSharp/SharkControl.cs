using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;

public class SharkControl : MonoBehaviour
{
    private Animator m_Animator;
    [SerializeField]private Hole m_hole;
    private void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void DriveInWater()
    {
      //Call by animator triggerd event 
      m_hole.PlayEffectSharkDriveIn();
    }
    public void EndAnimation()
    {
        //Call by animator triggerd event 
        m_hole.EndAnimationCharacter();
    }
    public void EndBite()
    {
        //Call by animator triggerd event 
        //m_Animator.SetTrigger("Bite");
        //Pool.Get<GamePlayGroup>().levelController.GetHolePlayer().EndAnimationCharacter();
        m_hole.readyForSharkBite = true;
    }
    public void EndBiteRotate()
    {
        //Call by animator triggerd event 
        //m_Animator.SetTrigger("Bite");
        //Pool.Get<GamePlayGroup>().levelController.GetHolePlayer().EndAnimationCharacter();
        m_hole.readyForSharkBite = true;
    }
}
