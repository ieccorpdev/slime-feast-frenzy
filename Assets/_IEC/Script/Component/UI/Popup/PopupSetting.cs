using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupSetting : _BaseBehaviour, IUIGroup
    {
        public Button btnClose, btnSound, btnVibrate, btnFeedBack, btnRestorePurchase;

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

            m_RectGroup.sizeDelta = new Vector2(640f, isHaveRestoreBtn ? 620f : 440f);
            m_RectPannel.sizeDelta = new Vector2(800f, isHaveRestoreBtn ? 900f : 750f);
        }
    }
}
