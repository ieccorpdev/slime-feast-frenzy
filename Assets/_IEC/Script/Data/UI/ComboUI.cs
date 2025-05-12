using DG.Tweening;
using Newtonsoft.Json.Linq;
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
    [SerializeField] private List<float> comboLocalPos = new List<float>();
    [SerializeField] private Transform [] comboTxtPos;
    [SerializeField] private List<Color> comboColorsTop = new List<Color>();
    [SerializeField] private List<Color> comboColorsBottom = new List<Color>();
    [SerializeField] private List<ParticleSystem> fxFireCombos = new List<ParticleSystem>();
    [SerializeField] private List<Material> comboMaterials= new List<Material>();
    [SerializeField] private List<Material> comboTxtMaterials= new List<Material>();
    [SerializeField] private List<int> thresholds = new List<int>();
    private int currentComboIndex = 0;
    [SerializeField] private int minComboIndexVibrate = 2;
    private bool canVibrate = true;

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
        currentComboCount++;
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
        if (combo < minComboCount)
        {
            ChangeComboColor();
            return;
        }
        comboTxt.text = $"{combo}";
        Show();

        if (combo >= thresholds[currentComboIndex])
        {
            currentComboIndex++;
            if (currentComboIndex >= thresholds.Count)
            {
                currentComboIndex = thresholds.Count - 1;
                return;
            }
            ChangeComboColor();
            //fxFireCombos[currentComboIndex].Play();
        }
        if (currentComboIndex > minComboIndexVibrate)
        {
            VibrateComboTxt();
        }
        if (combo < 1000)
        {
            comboTxt.transform.position = comboTxtPos[combo.ToString().Length - 1].position;
            this.transform.localPosition = new Vector3(comboLocalPos[combo.ToString().Length - 1], this.transform.localPosition.y, this.transform.localPosition.z);
        }
        else
        {
            comboTxt.transform.position = comboTxtPos[comboTxtPos.Length - 1].position;
            this.transform.localPosition = new Vector3(comboLocalPos[comboLocalPos.Count - 1], this.transform.localPosition.y, this.transform.localPosition.z);
        }
    }
}
