using System.Collections;
using UnityEngine;

namespace IEC
{
    public class AdsController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.GAME_RESTART:
                    Services.ShowFullAdsWithTime(Game.DelayInterstitial, () =>
                    {
                        if (Game.isHaveDynamicInter)
                        {
                            PlayerData.IncreaseNumberInterShow();
                        }
                        EventController.RaiseEvent(EventList.GamePlay.GAME_SPAWN_LEVEL);
                    });
                    break;
                case EventList.Service.INIT_AD_DONE:
                    Services.Ads().Load(AdsEnum.BANNER_BOTTOM, null);
                    Services.Ads().Show(AdsEnum.BANNER_BOTTOM, null);
                    StartCoroutine(IE_LoadAd());
                    UpdateWhiteBannerSize();
                    break;
            }
        }

        public void BehaviourAwake()
        {
        }

        public void BehaviourStart()
        {
            if (Application.isEditor)
            {
                UpdateWhiteBannerSize();
            }
        }

        IEnumerator IE_LoadAd()
        {
            var delaySec = new WaitForSecondsRealtime(3f);
            while (true)
            {
                Services.Ads().Load(AdsEnum.INTERSTITIAL, null);
                Services.Ads().Load(AdsEnum.REWARD, null);
                yield return delaySec;
            }
        }

        void UpdateWhiteBannerSize()
        {
            Debug.LogWarning("UpdateWhiteBannerSize");
            var sizeDelta = UI.bottomAds.sizeDelta;
            var bannerHeight = Screen.safeArea.y + (Application.isEditor ? 150 : Services.Ads().GetBannerHeight());
            Debug.LogWarning("bannerHeight: " + bannerHeight);
            Debug.LogWarning("screen height: " + Screen.height);
            Debug.LogWarning("canvasScaler ref height: " + UI.canvasScaler.referenceResolution.y);
            var ratio = UI.canvasScaler.referenceResolution.y / Screen.height;
            bannerHeight *= ratio;
            Debug.LogWarning("scaleBannerHeight: " + bannerHeight);
            sizeDelta.y = bannerHeight;
            sizeDelta.y = Mathf.Clamp(sizeDelta.y, 100, 200);
            UI.bottomAds.sizeDelta = sizeDelta;
        }
    }
}