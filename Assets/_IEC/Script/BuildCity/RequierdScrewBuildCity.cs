using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using TMPro;
using UnityEngine;

public class RequierdScrewBuildCity : _BaseBehaviour
{
    public Camera targetCamera;

    public TextMeshPro numberScrewNeed;
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    private void Start()
    {
        var bcty = GameObject.Find("BuildCity").GetComponent<BuildCity>();
        targetCamera = bcty.buildCityCamera;
        Locator.Main.HomeScreenTab.buildCityUI = bcty;
    }

    void Update()
    {
        // Make the object look at the camera
        if (targetCamera)
        {
            transform.LookAt(targetCamera.transform);
        }
    }

    public void UpdateNumberScrewNeed(int valueNeed)
    {
        numberScrewNeed.text = valueNeed.ToString();
    }

    public void EffectComplete()
    {
        numberScrewNeed.gameObject.SetActive(false);
    }
}
