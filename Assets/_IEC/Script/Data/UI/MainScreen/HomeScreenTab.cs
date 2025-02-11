using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;
using UnityEngine.UI;
using _IECModules;


public class HomeScreenTab : ScreenNavigateUI
{
    [Header("Avatar Button")] 
    public Button avatarButton;
    public Image avatarImage;
    [Header("LuckyWheel")] 
    public Button luckyWheel;
    // Start is called before the first frame update
    void Start()
    {
        SetEvent(avatarButton,
            () =>
            {
                //Services.PushEvent(new TrackingEvent.ClickBtnTracking("open_avatar"));
                Locator.Main.PopupAvatar.Show();
            });      
        SetEvent(luckyWheel,
            () =>
            {
                //Services.PushEvent(new TrackingEvent.ClickBtnTracking("open_avatar"));
                Locator.Main.PopupLuckyWheel.Show();
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        UpdateMainHome();
    }

    public void UpdateMainHome()
    {
        avatarImage.sprite = Config.playerAvatarConfig.GetAvatarSprite(PlayerData.playerAvatarSpriteName);
        luckyWheel.gameObject.SetActive(PlayerData.level >= Config.dataRewardLuckyWheelConfig.levelUnlock);
    }
    
}
