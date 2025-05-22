using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;
using UnityEngine.UI;

public class MainBGController : _BaseBehaviour
{
    public Image staticImage;

    public SpineAnimOT spineImage;
    // Start is called before the first frame update
    void Start()
    {
        staticImage.gameObject.SetActive(Game.iosFeature);
        spineImage.gameObject.SetActive(!Game.iosFeature);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
