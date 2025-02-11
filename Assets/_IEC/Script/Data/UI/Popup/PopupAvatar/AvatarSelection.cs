using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IEC
{
    public class AvatarSelection : _BaseBehaviour, IPointerDownHandler
    {
        public Image avatarImage;
        public GameObject selectedGroup;

        public void Init(Sprite avatarSprite)
        {
            avatarImage.sprite = avatarSprite;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            OnSelectAvatar();
        }

        private void OnSelectAvatar()
        {
            Locator.Main.PopupAvatar.SelectAvatar(this);
            SetSelectedDisplay(true);
        }

        public void SetSelectedDisplay(bool isSelected)
        {
            selectedGroup.gameObject.SetActive(isSelected);
        }
    }
}
