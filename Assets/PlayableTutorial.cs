using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay;
using DG.Tweening;
using IEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayableTutorial : MonoBehaviour
{
    public Button buttonMagnet, buttonExpand, buttonSpeedUp;
    public bool canShow = true;
    public int enableTimeScale;

    private void Start()
    {
        if (buttonMagnet)
        {
            buttonMagnet.onClick.AddListener(() =>
            {
                PlayableController.Instance.OnMagnetButtonClicked();
                this.gameObject.SetActive(false);
                Time.timeScale = 1;
                EventController.RaiseEvent(EventList.Audio.HOLE_LEVELUP);
            });
        }
        if (buttonExpand != null)
        {
            buttonExpand.onClick.AddListener(() =>
            {
                PlayableController.Instance.OnExpandButtonClicked();
                this.gameObject.SetActive(false);
                Time.timeScale = 1;
                EventController.RaiseEvent(EventList.Audio.HOLE_LEVELUP);
            });
        }
        if (buttonSpeedUp != null)
        {
            buttonSpeedUp.onClick.AddListener(() =>
            {
                this.RaiseEvent(EventList.GamePlay.ON_BOOSTER_COLLECTED, new BoosterData()
                {
                    boosterType = ResourceType.WindSurf,
                    effectValue = 80,
                    effectTime = 20,
                });
                this.gameObject.SetActive(false);
                Time.timeScale = 1;
                EventController.RaiseEvent(EventList.Audio.HOLE_LEVELUP);
            });
        }

        if (PlayableController.Instance.isUseItem)
        {
            canShow = true;
            ShowTut();
        }
        else
        {
           HideTut();
        }
    }

    public void ShowTut()
    {
        if (canShow)
        {
            Time.timeScale = enableTimeScale;
            canShow = false;
            this.gameObject.SetActive(true);
        }
    }

    public void HideTut()
    {
        this.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
