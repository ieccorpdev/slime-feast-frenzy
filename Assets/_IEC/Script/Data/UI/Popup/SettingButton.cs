using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IEC;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
namespace IEC
{
    public class SettingButton : _BaseBehaviour
    {
        [Header("Background")]
        public Image background;
        public Sprite backgroundOnSprite;
        public Sprite backgroundOffSprite;

        [Header("Button")] 
        public SettingType settingType;
        public Button button;
        public RectTransform onHandle;
        public RectTransform offHandle;
        public TextMeshProUGUI onText;
        public TextMeshProUGUI offText;

        private string _tween_ID => gameObject.GetInstanceID().ToString();

        private Vector3 _onPos;
        private Vector3 _offPos;
        
        private void Start()
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(HandleClick);

            _onPos = onHandle.anchoredPosition;
            _offPos = offHandle.anchoredPosition;
            
            UpdateButtonUI(IsSettingOn());
        }

        public void UpdateUI()
        {
            UpdateButtonUI(IsSettingOn());
        }

        private void HandleClick()
        {
            Services.PushEvent(new TrackingEvent.ClickBtnTracking("btn_setting_" + settingType));
            SetButton(!IsSettingOn());
        }

        private void SetButton(bool isOn)
        {
            ChangeSetting(isOn);

            DOTween.Kill(_tween_ID);
            
            // onText.gameObject.SetActive(isOn);
            // offText.gameObject.SetActive(!isOn);

            offHandle.anchoredPosition = _offPos;
            onHandle.anchoredPosition = _onPos;

            var moveObj = isOn ? offHandle : onHandle;
            var targetObj =  isOn ? onHandle : offHandle;

            var initPos = moveObj.anchoredPosition;
            
            background.sprite = isOn ? backgroundOnSprite : backgroundOffSprite;
            // onText.gameObject.SetActive(isOn);
            // offText.gameObject.SetActive(!isOn);
            
            moveObj.DOAnchorPos(endValue: targetObj.anchoredPosition, duration: 0.15f)
                .SetId(_tween_ID)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    moveObj.anchoredPosition = initPos;
                    
                    UpdateButtonUI(isOn);
                });
        }
        
        public void UpdateButton()
        {
            UpdateButtonUI(IsSettingOn());
        }

        private void UpdateButtonUI(bool isOn)
        {
            // offHandle.GetComponent<Image>().enabled = !isOn;
            // onHandle.GetComponent<Image>().enabled = isOn;
            offHandle.gameObject.SetActive(!isOn);
            onHandle.gameObject.SetActive(isOn);
                    
            // onText.gameObject.SetActive(isOn);
            // offText.gameObject.SetActive(!isOn);

            background.sprite = isOn ? backgroundOnSprite : backgroundOffSprite;
        }
        
        private void ChangeSetting(bool isOn)
        {
            switch (settingType)
            {
                case SettingType.Music:
                    PlayerData.isMusicOn = isOn;
                    EventController.RaiseEvent(EventList.GamePlay.SETTING_MUSIC);
                    break;
                case SettingType.Sound:
                    PlayerData.isSoundOn = isOn;
                    EventController.RaiseEvent(EventList.GamePlay.SETTING_SOUND);
                    break;
                case SettingType.Vibrate:
                    PlayerData.isVibrateOn = isOn;
                    EventController.RaiseEvent(EventList.GamePlay.SETTING_VIBRATES);
                    break;
            }
        }

        bool IsSettingOn()
        {
            return settingType switch
            {
                SettingType.Music => PlayerData.isMusicOn,
                SettingType.Sound => PlayerData.isSoundOn,
                SettingType.Vibrate => PlayerData.isVibrateOn,
                _ => false
            };
        }
    }
    
    public enum SettingType
    {
        Music,
        Sound,
        Vibrate
    }
}
