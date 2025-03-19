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
        public List<AvatarSelection> listAvatarSelections = new();
        
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
            currentPlayerAvatarImage.sprite = Config.playerAvatarConfig.GetAvatarSprite(PlayerData.playerAvatarSpriteName);
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void Init()
        {

            if (String.IsNullOrEmpty(PlayerData.playerAvatarSpriteName))
            {
                PlayerData.playerAvatarSpriteName = Config.playerAvatarConfig.GetAvatarSprite("").name;
            }
            
            for (int i = 0; i < Config.playerAvatarConfig.listAvatarSprites.Count; i++)
            {
                if (i >= listAvatarSelections.Count)
                {
                    listAvatarSelections.Add(ObjectPool.Spawn(avatarSelectionPrefab, avatarSelectionContainer));
                }

                listAvatarSelections[i].transform.localScale = Vector3.one;
                listAvatarSelections[i].Init(Config.playerAvatarConfig.listAvatarSprites[i]);
                
                if(PlayerData.playerAvatarSpriteName.Equals(Config.playerAvatarConfig.listAvatarSprites[i].name))
                {
                    listAvatarSelections[i].SetSelectedDisplay(true);
                    CurrentSelectedAvatar = listAvatarSelections[i];
                }
                else
                {
                    listAvatarSelections[i].SetSelectedDisplay(false);
                }
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
