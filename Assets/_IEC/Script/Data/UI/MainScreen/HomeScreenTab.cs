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
    // Start is called before the first frame update
    void Start()
    {
        SetEvent(avatarButton,
            () =>
            {
                //Services.PushEvent(new TrackingEvent.ClickBtnTracking("open_avatar"));
                Locator.Main.PopupAvatar.Show();
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        UpdatePlayerAvatar();
    }

    public void UpdatePlayerAvatar()
    {
        avatarImage.sprite = Config.playerAvatarConfig.GetAvatarSprite(PlayerData.playerAvatarSpriteName);
    }
}
