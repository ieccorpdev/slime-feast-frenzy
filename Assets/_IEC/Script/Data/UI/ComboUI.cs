using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboUI : MonoBehaviour
{
    [Header("0 -> 9")]
    public Sprite[] comboNumberSprites;
    [Header("Text")]
    public Image[] comboTxt;
    public Animator animator;

    public void Show()
    {
        gameObject.SetActive(true);
        animator.Play("Show");
    }

    public void Hide()
    {
        animator.Play("Hide");
        gameObject.SetActive(false);
    }

    public void SetCombo(int combo)
    {
        if (combo <= 0 || combo >= comboTxt.Length * 10)
        {
            Debug.LogError("Invalid combo number: " + combo);
            return;
        }
        for (int i = 0; i < comboTxt.Length; i++)
        {
            comboTxt[i].gameObject.SetActive(false);
        }
        int j = 0;
        while (combo > 0)
        {
            int index = combo % 10;
            comboTxt[j].gameObject.SetActive(true);
            comboTxt[j].sprite = comboNumberSprites[index - 1];
            combo /= 10;
            j++;
        }
    }


}
