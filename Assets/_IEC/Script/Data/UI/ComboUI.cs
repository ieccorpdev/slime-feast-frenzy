using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboUI : MonoBehaviour
{
    [Header("0 -> 9")]
    [SerializeField] private Sprite[] comboNumberSprites;
    [Header("Text")]
    [SerializeField] private Image[] comboTxt;
    [SerializeField] private Animator animator;
    private int currentComboCount;
    [SerializeField] private float comboTimeRange = 0.3f;
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
        if (combo < minComboCount || combo >= comboTxt.Length * 10)
        {
            return;
        }
        Debug.Log(combo);

        //for (int i = 0; i < comboTxt.Length; i++)
        //{
        //    comboTxt[i].gameObject.SetActive(false);
        //}
        //int j = 0;
        //while (combo > 0)
        //{
        //    int index = combo % 10;
        //    comboTxt[j].gameObject.SetActive(true);
        //    comboTxt[j].sprite = comboNumberSprites[index - 1];
        //    combo /= 10;
        //    j++;
        //}
    }


}
