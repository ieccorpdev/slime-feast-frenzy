using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace IEC
{
    public class AvatarSelection : _BaseBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public Image avatarFrameImage;
        public Image avatarImage;
        public GameObject avatarSelectedGroup;
        public GameObject frameSelectedGroup;
        public GameObject selectedIcon;
        public GameObject lockIcon;
        
        public AvatarSelectionUIState CurrentState { get; private set; }

        public void Init(Sprite sprite)
        {
            avatarSelectedGroup.SetActive(false);
            frameSelectedGroup.SetActive(false);
            selectedIcon.SetActive(false);
            if(lockIcon != null)
                lockIcon.SetActive(false);
            if (Locator.Main.PopupEditProfile.CurrentSelectedTab == ProfileSelectTab.Avatar)
            {
                avatarImage.gameObject.SetActive(true);
                avatarImage.sprite = sprite;
                avatarFrameImage.sprite = Config.playerAvatarConfig.listAvatarFrameSprites[0];
            }
            else if (Locator.Main.PopupEditProfile.CurrentSelectedTab == ProfileSelectTab.Frame)
            {
                avatarFrameImage.sprite = sprite;
                avatarImage.gameObject.SetActive(false);
            }
            
            CheckSetLockDisplay(Config.playerAvatarConfig.GetUnlockConfig(sprite.name));
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            //OnSelectAvatar();
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if (Locator.Main.PopupEditProfile.Dragging)
                return;
            
            OnSelectAvatar();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(Locator.Main.PopupEditProfile.avatarScrollGroup != null)
                Locator.Main.PopupEditProfile.avatarScrollGroup.OnDrag(eventData);
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (Locator.Main.PopupEditProfile.avatarScrollGroup != null)
            {
                Locator.Main.PopupEditProfile.avatarScrollGroup.OnBeginDrag(eventData);
                Locator.Main.PopupEditProfile.Dragging = true;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (Locator.Main.PopupEditProfile.avatarScrollGroup != null)
            {
                Locator.Main.PopupEditProfile.avatarScrollGroup.OnEndDrag(eventData);
                Locator.Main.PopupEditProfile.Dragging = false;
            }
        }

        private void OnSelectAvatar()
        {
            if (CurrentState == AvatarSelectionUIState.Lock)
                return;
            
            Locator.Main.PopupEditProfile.SelectAvatar(this);
            SetSelectedDisplay(true);
        }

        public void SetSelectedDisplay(bool isSelected)
        {
            selectedIcon.SetActive(isSelected);

            if (Locator.Main.PopupEditProfile.CurrentSelectedTab == ProfileSelectTab.Avatar)
            {
                avatarSelectedGroup.SetActive(isSelected);
            }
            else if (Locator.Main.PopupEditProfile.CurrentSelectedTab == ProfileSelectTab.Frame)
            {
                frameSelectedGroup.SetActive(isSelected);
            }
        }

        public void CheckSetLockDisplay(AvatarNFrameUnlockConfig unlockConfig)
        {
            if (unlockConfig == null)
            {
                SetSelectionState(AvatarSelectionUIState.Unlock);
                return;
            }

            switch (unlockConfig.conditionType)
            {
                case AvatarUnlockConditionType.Level:
                    break;
                case AvatarUnlockConditionType.KingdomPass:
                    SetSelectionState(PlayerData.journeyData.isEnableMaxHeart
                        ? AvatarSelectionUIState.Unlock
                        : AvatarSelectionUIState.Lock);
                    break;
            }
            
            if(CurrentState == AvatarSelectionUIState.Lock)
                SetLockDisplay(unlockConfig);
        }

        private void SetLockDisplay(AvatarNFrameUnlockConfig unlockConfig)
        {
            avatarFrameImage.sprite = unlockConfig.lockFrameSprite;

            if(lockIcon != null)
                lockIcon.SetActive(true);
        }

        public void SetSelectionState(AvatarSelectionUIState state)
        {
            CurrentState = state;
        }
    }

    public enum AvatarSelectionUIState
    {
        Lock,
        Unlock
    }

    public enum AvatarUnlockConditionType
    {
        Level,
        KingdomPass,
    }
}
