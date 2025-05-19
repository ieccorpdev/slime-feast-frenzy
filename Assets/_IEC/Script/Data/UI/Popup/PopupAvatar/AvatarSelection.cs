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

        public void Init(Sprite sprite)
        {
            avatarSelectedGroup.SetActive(false);
            frameSelectedGroup.SetActive(false);
            selectedIcon.SetActive(false);
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
    }
}
