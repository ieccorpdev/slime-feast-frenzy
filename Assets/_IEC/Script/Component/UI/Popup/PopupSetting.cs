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
                SendEmail();
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
            btnRestorePurchase.gameObject.SetActive(Game.isHaveNoAdsInMainScreen);

            //m_RectGroup.sizeDelta = new Vector2(640f, Game.isHaveNoAdsInMainScreen ? 620f : 440f);
            //m_RectPannel.sizeDelta = new Vector2(800f, Game.isHaveNoAdsInMainScreen ? 900f : 750f);
        }

        void SendEmail()
        {
            string email = "support@iecglobal.com.au";
            string title = string.Empty;
            if (Application.platform == RuntimePlatform.Android)
            {
                title = "Water Connect Puzzle Android Support";
            }
            else
            {
                title = "Water Connect Puzzle IOS Support";
            }
            title = MyEscapeURL(title);
            //var userIDFA = Pool.Get<PlayerSecret>().userIDFA;
            var newLine = Application.isMobilePlatform ? "<br>" : System.Environment.NewLine;
            string message = "Water Connect Puzzle version: " + Application.version + newLine;
            //if (!string.IsNullOrWhiteSpace(userIDFA))
            //{
            //    message += "ID: " + newLine;
            //}
            message += newLine + "Hello, " + newLine;
            Debug.Log("message SendEmail: " + message);
            message = MyEscapeURL(message);
            Debug.Log("message SendEmail EscapeURL: " + message);
            Application.OpenURL("mailto:" + email + "?subject=" + title + "&body=" + message);
        }

        string MyEscapeURL(string url)
        {
            return WWW.EscapeURL(url).Replace("+", "%20");
        }

    }
}
