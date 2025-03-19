using IEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupSetting : BasePopup
{
    [SerializeField] private Button soundBtn;
    [SerializeField] private Button vibrateBtn;
    [SerializeField] private Button restartBtn;


    protected override void Awake()
    {
        base.Awake();
        SetEvent(soundBtn, OnSoundBtnClick);
        SetEvent(vibrateBtn, OnVibrateBtnClick);
        SetEvent(restartBtn, () => {
            EventController.RaiseEvent(EventList.GamePlay.END_LEVEL_ADS, new EventParam(this, name));
            Services.PushEvent(new IEC.TrackingEvent.ClickBtnReplay());
        });
    }

    public void SetActiveRestartButton(bool isEnable)
    {
        restartBtn.gameObject.SetActive(isEnable);
    }

    private void OnVibrateBtnClick()
    {
    }

    private void OnSoundBtnClick()
    {
    }


    
}
