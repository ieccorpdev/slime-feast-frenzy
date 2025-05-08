using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HelperIntro : MonoBehaviour
{
    [SerializeField] private Animator helperIntroAnim;
    [SerializeField] private ParticleSystem helperIntroFx;
    [SerializeField] private TMP_Text helperIntroTxt;

    public void ShowHelperIntroAnimation(string messenger)
    {
        helperIntroTxt.text = messenger;
        if (helperIntroAnim)
            helperIntroAnim.Play("Show");
        if (helperIntroFx)
            helperIntroFx.Play();
    }
}
