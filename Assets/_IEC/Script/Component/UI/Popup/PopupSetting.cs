using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupSetting : _BaseBehaviour, IUIGroup
    {
        public Button btnClose, btnSound, btnVibrate, btnFeedBack, btnRestorePurchase, btnEditConsent;

        public GameObject soundOffObj, vibrateOffObj;

        [SerializeField]
        private RectTransform m_RectPannel, m_RectGroup;

        public void OnInit()
        {
            btnClose.onClick.AddListener(() => {
                UI.popupSettings.gameObject.SetActive(false);
                Game.isShowPopup = false;
            });

            btnSound.onClick.AddListener(() => {
                PlayerData.isSoundOn = !PlayerData.isSoundOn;
                SetStatus(PlayerData.isSoundOn, PlayerData.isVibrateOn);
            });

            btnVibrate.onClick.AddListener(() => {
                PlayerData.isVibrateOn = !PlayerData.isVibrateOn;
                SetStatus(PlayerData.isSoundOn, PlayerData.isVibrateOn);
            });

            btnFeedBack.onClick.AddListener(() => {
                Services.SendEmail();
            });

            btnRestorePurchase.onClick.AddListener(() => {
                EventController.RaiseEvent(EventList.GamePlay.IAP_RESTORE_NO_ADS);
            });

            btnEditConsent.onClick.AddListener(() =>
            {
                Controllers.Get<ServicesController>().EditOCM();
            });

        }

        public void SetStatus(bool isSoundOn, bool isVibrateOn)
        {
            soundOffObj.SetActive(!isSoundOn);
            vibrateOffObj.SetActive(!isVibrateOn);

            Sound.SetupAudio();
        }

        public void SetupAbUI()
        {
            var isHaveRestoreBtn = Game.isHaveNoAdsV3 || Game.isHaveNoAdsInShopTab;
            btnRestorePurchase.gameObject.SetActive(isHaveRestoreBtn);
        }

        public void SetActiveConsent(bool isActive)
        {
            btnEditConsent.gameObject.SetActive(isActive);

            var isHaveRestoreBtn = Game.isHaveNoAdsV3 || Game.isHaveNoAdsInShopTab;

            var isScaleUp = isActive || isHaveRestoreBtn;
            var isHaveBoth = isActive && isHaveRestoreBtn;

            var scaleYGroup = isHaveBoth ? 800f : (isScaleUp ? 620f : 440f);
            var scaleYPanel = isHaveBoth ? 1290f : (isScaleUp ? 1020f : 750f);

            m_RectGroup.sizeDelta = new Vector2(620f, scaleYGroup);
            m_RectPannel.sizeDelta = new Vector2(800f, scaleYPanel);
        }
    }
}
