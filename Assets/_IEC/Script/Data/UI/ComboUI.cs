using DG.Tweening;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboUI : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private TMP_Text comboTxt;
    private int currentComboCount;
    [SerializeField] private float comboTimeRange = 1.2f;
    [SerializeField] private int minComboCount = 3;
    [SerializeField] private List<float> comboLocalPos = new List<float>();
    [SerializeField] private Transform [] comboTxtPos;

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
        Hide();
    }

    private void SetCombo(int combo)
    {
        if (combo < minComboCount)
        {
            return;
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
        comboTxt.text = $"{combo}";
        Show();
    }
}
