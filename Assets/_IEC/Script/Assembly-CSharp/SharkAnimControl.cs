using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;
public class SharkAnimControl : MonoBehaviour
{
    private Animator m_Animator;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void DriveInWater()
    {
    }
    public void EndAnimation()
    {
    }
    public void EndBite()
    {
        //Call by animator triggerd event 
    }
    public void EndBiteRotate()
    {
    }
}
