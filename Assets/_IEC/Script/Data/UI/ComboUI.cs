using DG.Tweening;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboUI : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text comboTxt;
    [SerializeField] private TMP_Text subComboTxt;
    private int currentComboCount;
    [SerializeField] private float comboTimeRange = 1.2f;
    [SerializeField] private int minComboCount = 3;
    [SerializeField] private Transform [] comboTxtPos;
    [SerializeField] private Transform [] subComboTxtPos;
    [SerializeField] private List<Color> comboColorsTop = new List<Color>();
    [SerializeField] private List<Color> comboColorsBottom = new List<Color>();
    [SerializeField] private List<ParticleSystem> fxFireCombos = new List<ParticleSystem>();
    [SerializeField] private List<Material> comboMaterials= new List<Material>();
    [SerializeField] private List<Material> comboTxtMaterials= new List<Material>();
    [SerializeField] private List<int> thresholds = new List<int>();
    private int currentComboIndex = 0;
    [SerializeField] private int minComboIndexVibrate = 2;
    private bool canVibrate = true;
    [NonSerialized] public int bonusCombo = 1;

    private void Start()
    {
        ChangeComboColor();
    }

    private void Show()
    {
        animator.Play("Show");
    }

    private void Hide()
    {
        animator.Play("Hide");
    }

    public void HandleEatCombo()
    {
        StopAllCoroutines();
        StartCoroutine(CheckInCombo());
        currentComboCount += bonusCombo;
        SetCombo(currentComboCount);
    }

    IEnumerator CheckInCombo()
    {
        yield return new WaitForSeconds(comboTimeRange);
        currentComboCount = 0;
        currentComboIndex = 0;
        canVibrate = true;
        subComboTxt.transform.parent.DOKill();
        Hide();
    }

    private void ChangeComboColor()
    {
        // Corrected the VertexGradient initialization to match the constructor's expected parameters
        VertexGradient gradient = new VertexGradient(
            comboColorsTop[currentComboIndex],  // topLeft
            comboColorsTop[currentComboIndex],  // topRight
            comboColorsBottom[currentComboIndex], // bottomLeft
            comboColorsBottom[currentComboIndex]  // bottomRight
        );

        // Assign the gradient to the TMP_Text component
        comboTxt.colorGradient = gradient;
        subComboTxt.colorGradient = gradient;
        comboTxt.fontMaterial = comboMaterials[currentComboIndex];
        subComboTxt.fontMaterial = comboTxtMaterials[currentComboIndex];
    }

    private void VibrateComboTxt()
    {
        if (!canVibrate) return;
        canVibrate = false;
        subComboTxt.transform.parent.DOKill();

        float duration = 0.3f; 
        float strength = 20f; 
        int vibrato = 50;    
        float randomness = 90f;

        subComboTxt.transform.parent.DOShakePosition(duration, strength, vibrato, randomness, false, true).onComplete += () => canVibrate = true;
    }

    private void SetCombo(int combo)
    {
        if (combo == 1)
        {
            ChangeComboColor();
            return;
        }
        comboTxt.text = $"{combo}";

        if (combo >= thresholds[currentComboIndex])
        {
            currentComboIndex++;
            if (currentComboIndex >= thresholds.Count)
            {
                currentComboIndex = thresholds.Count - 1;
            }
            else
            {
                ChangeComboColor();
                //fxFireCombos[currentComboIndex].Play();
            }
        }
        if (currentComboIndex > minComboIndexVibrate)
        {
            VibrateComboTxt();
        }
        if (combo < 10000)
        {
            comboTxt.transform.localPosition = comboTxtPos[comboTxt.text.Length - 1].localPosition;
            subComboTxt.transform.localPosition = subComboTxtPos[comboTxt.text.Length - 1].localPosition;
        }
        else
        {
            comboTxt.transform.position = comboTxtPos[comboTxtPos.Length - 1].position;
            subComboTxt.transform.position = subComboTxtPos[comboTxtPos.Length - 1].position;
        }
        Show();
    }
}
