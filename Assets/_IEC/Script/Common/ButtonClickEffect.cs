using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IEC
{
    public class ButtonClickEffect : _BaseBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public Image buttonDisplayImage;
        [Header("Button Sprite")] 
        public bool setNative = true;
        public bool isChangeButtonSpriteOnClick = false;
        public Sprite pressSprite;
        public Sprite releaseSprite;
        public bool isChangeButtonSpriteOnInactive = false;
        public Sprite activeSprite;
        public Sprite inactiveSprite;
        public GameObject activeGroup, inactiveGroup;
        [Header("Button scale")] 
        private bool isScaleButtonOnClick = true;
        [Range(0,1)]private float buttonScaleOnPressDown = 0.96f;
        
        // [Header("Button sound")] 
        // public bool isPlayDefaultSoundOnClick = true;
        
        private Button m_Button;
        
        public ButtonState ButtonState { get; private set; } = ButtonState.Active;
        
        private void Start()
        {
            m_Button = GetComponent<Button>();
        }

        private void OnDisable()
        {
            ChangeSprite(false);
        }

        public void SetState(ButtonState state)
        {
            ButtonState = state;
            
            if(activeGroup != null)
                activeGroup.SetActive(state == ButtonState.Active);
            
            if(inactiveGroup != null)
                inactiveGroup.SetActive(state == ButtonState.Inactive);
            
            ChangeSpriteOnInactive(ButtonState == ButtonState.Inactive);
        }

        private void ChangeSprite(bool isPress)
        {
            if (!isChangeButtonSpriteOnClick)
                return;

            if (ButtonState == ButtonState.Inactive)
                return;
            if (pressSprite == null || releaseSprite == null)
                return;
            if (isPress)
                buttonDisplayImage.sprite = pressSprite;
            else
                buttonDisplayImage.sprite = releaseSprite;
            
            if(setNative) buttonDisplayImage.SetNativeSize();
        }

        private void ChangeSpriteOnInactive(bool isInactive)
        {
            if (!isChangeButtonSpriteOnInactive)
                return;
            if (pressSprite == null || releaseSprite == null)
                return;
            if (isInactive)
                buttonDisplayImage.sprite = inactiveSprite;
            else
                buttonDisplayImage.sprite = activeSprite;
            
            //if(setNative) buttonDisplayImage.SetNativeSize();
        }

        private void ChangeScale(bool isPress)
        {
            if (!isScaleButtonOnClick)
                return;
            
            if (m_Button ==null || !m_Button.interactable || ButtonState == ButtonState.Inactive)
                return;
            GameObject go = buttonDisplayImage == null ? gameObject : buttonDisplayImage.gameObject;
            if (isPress)
                go.transform.DOScale(Vector3.one * buttonScaleOnPressDown, 0.1f).SetEase(Ease.InQuad);
            else
                go.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutQuad);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            switch (ButtonState)
            {
                case ButtonState.Active:
                    EventController.RaiseEvent(EventList.Audio.ACTIVE_BUTTON_CLICK);
                    break;
                case ButtonState.Inactive:
                    EventController.RaiseEvent(EventList.Audio.INACTIVE_BUTTON_CLICK);
                    break;
            }
            
            ChangeSprite(true);
            ChangeScale(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ChangeSprite(false);
            ChangeScale(false);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ChangeSprite(false);
        }
    }

    public enum ButtonState
    {
        Active,
        Inactive
    }
    
}
