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
    }

    private void SetCombo(int combo)
    {
        if (combo < minComboCount)
        {
            return;
        }
        comboTxt.text = $"{combo} <size=68>COMBO!";
        Show();
    }
}
