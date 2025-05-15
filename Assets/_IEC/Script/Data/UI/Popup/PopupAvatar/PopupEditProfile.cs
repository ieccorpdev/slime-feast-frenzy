using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupEditProfile : BasePopup
    {
        public Button allBGButton;
        public PlayerAvatar playerAvatar;
        public Button btnEditProfileName;
        public Button saveButton;

        public List<EditProfileTabSelection> listTabSelection = new();

        public RectTransform avatarSelectionContainer;
        public AvatarSelection avatarSelectionPrefab;
        public List<AvatarSelection> listAvatarSelections = new();

        public EditProfileTabSelection CurrentTabSelection { get; private set; }
        public AvatarSelection CurrentSelectedAvatar { get; private set; }
        
        public ProfileSelectTab CurrentSelectedTab { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            
            SetEvent(saveButton,
                () =>
                {
                    SaveProfile();
                });
            
            SetEvent(btnEditProfileName,
                () =>
                {
                    Locator.Main.PopupEditProfileName.Show();
                });

            foreach (var selection in listTabSelection)
            {
                SetEvent(selection.btn, ()=>{OnSelectTab(selection);});
            }
            
            SetEvent(allBGButton, OnHideClick);

            //InitAvatarTab();
        }

        public override void Show()
        {
            base.Show();
            EventController.RaiseEvent(EventList.GamePlay.UPDATE_PLAYER_PROFILE);
            
            //playerAvatar.playerAvatar.sprite = Config.playerAvatarConfig.GetAvatarSprite(PlayerData.playerAvatarSpriteName);
            
            OnSelectTab(listTabSelection.Find(selection => selection.profileSelectTab == ProfileSelectTab.Avatar));
        }

        public override void Hide()
        {
            EventController.RaiseEvent(EventList.GamePlay.UPDATE_PLAYER_PROFILE);
            base.Hide();
        }

        public void InitAvatarTab()
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

        public void InitFrameTab()
        {
            
        }

        public void OnSelectTab(EditProfileTabSelection tabSelection)
        {
            if (CurrentTabSelection != null)
            {
                CurrentTabSelection.SetSelected(false);
            }

            CurrentTabSelection = tabSelection;
            CurrentTabSelection.SetSelected(true);
            
            CurrentSelectedTab = tabSelection.profileSelectTab;

            if (tabSelection.profileSelectTab == ProfileSelectTab.Avatar)
            {
                InitAvatarTab();
            }
            else if (tabSelection.profileSelectTab == ProfileSelectTab.Frame)
            {
                InitFrameTab();
            }
        }

        public void SelectAvatar(AvatarSelection avatar)
        {
            if (CurrentSelectedAvatar != null)
            {
                CurrentSelectedAvatar.SetSelectedDisplay(false);
            }
            
            CurrentSelectedAvatar = avatar;

            if (CurrentSelectedTab == ProfileSelectTab.Avatar)
            {
                playerAvatar.playerAvatar.sprite = CurrentSelectedAvatar.avatarImage.sprite;
            }
            else if (CurrentSelectedTab == ProfileSelectTab.Frame)
            {
                playerAvatar.playerAvatarFrame.sprite = CurrentSelectedAvatar.avatarFrameImage.sprite;
            }
        }

        private void SaveProfile()
        {
            PlayerData.playerAvatarSpriteName = playerAvatar.playerAvatar.sprite.name;
            PlayerData.playerAvatarFrameName = playerAvatar.playerAvatarFrame.sprite.name;
            EventController.RaiseEvent(EventList.GamePlay.UPDATE_PLAYER_PROFILE);
            OnHideClick();
        }
    }

    public enum ProfileSelectTab
    {
        Avatar,
        Frame
    }
}
