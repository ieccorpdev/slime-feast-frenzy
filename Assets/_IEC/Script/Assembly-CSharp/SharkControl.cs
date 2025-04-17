using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;

public class SharkControl : MonoBehaviour
{
    private Animator m_Animator;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void DriveInWater()
    {
      //Call by animator triggerd event 
      Pool.Get<GamePlayGroup>().levelController.GetHolePlayer().PlayEffectSharkDriveIn();
    }
    public void EndAnimation()
    {
        //Call by animator triggerd event 
        Pool.Get<GamePlayGroup>().levelController.GetHolePlayer().EndAnimationCharacter();
    }
    public void EndBite()
    {
        //Call by animator triggerd event 
        //m_Animator.SetTrigger("Bite");
        //Pool.Get<GamePlayGroup>().levelController.GetHolePlayer().EndAnimationCharacter();
    }
}
