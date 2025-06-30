using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayableTutorial : MonoBehaviour
{
    public Button buttonMagnet, buttonExpand;
    private bool canShow = true;
    private void Start()
    {
        buttonMagnet.onClick.AddListener(() =>
        {
            PlayableController.Instance.OnMagnetButtonClicked();
            this.gameObject.SetActive(false);
            Time.timeScale = 1;
        });
        buttonExpand.onClick.AddListener(() =>
        {
            PlayableController.Instance.OnExpandButtonClicked();
            this.gameObject.SetActive(false);
            Time.timeScale = 1;
        });
    }

    public void ShowTut()
    {
        if (canShow)
        {
            Time.timeScale = 0;
            canShow = false;
            this.gameObject.SetActive(true);
        }
    }
}
