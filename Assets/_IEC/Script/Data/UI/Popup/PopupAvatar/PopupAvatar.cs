using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupAvatar : BasePopup
    {
        public Button allBGButton;
        public Image currentPlayerAvatarImage;
        public TMP_InputField playerName;
        public Button saveButton;

        public RectTransform avatarSelectionContainer;
        public AvatarSelection avatarSelectionPrefab;
        public List<AvatarSelection> listAvatarSelections = new List<AvatarSelection>();
        
        public AvatarSelection CurrentSelectedAvatar { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            
            SetEvent(saveButton,
                () =>
                {
                    SaveProfile();
                });
            
            SetEvent(allBGButton, OnHideClick);

            Init();
        }

        public override void Show()
        {
            base.Show();
            
            playerName.text = PlayerData.playerName;
            currentPlayerAvatarImage.sprite = PlayConfig.playerAvatarConfig.GetAvatarSprite(PlayerData.playerAvatarSpriteName);
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void Init()
        {

            if (String.IsNullOrEmpty(PlayerData.playerAvatarSpriteName))
            {
                PlayerData.playerAvatarSpriteName = PlayConfig.playerAvatarConfig.GetAvatarSprite("").name;
            }
        }
        

        public void SelectAvatar(AvatarSelection avatar)
        {
            if (CurrentSelectedAvatar != null)
            {
                CurrentSelectedAvatar.SetSelectedDisplay(false);
            }
            
            CurrentSelectedAvatar = avatar;
            
            currentPlayerAvatarImage.sprite = CurrentSelectedAvatar.avatarImage.sprite;
        }

        private void SaveProfile()
        {
            PlayerData.playerName = playerName.text;
            PlayerData.playerAvatarSpriteName = CurrentSelectedAvatar.avatarImage.sprite.name;
            //Meomew
            Locator.Main.HomeScreenTab.UpdateMainHome();
            OnHideClick();
        }
    }
}
