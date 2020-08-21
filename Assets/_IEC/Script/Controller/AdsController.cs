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
                //case EventList.GamePlay.END_LEVEL_ADS:
                //    Services.ShowFullAdsWithTime(Game.timeDelayFullAds, () =>
                //    {
                //        EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
                //    });
                //    break;
                case EventList.Service.INIT_AD_DONE:
                    Services.Ads().Load(AdsEnum.BANNER_BOTTOM, null);
                    Services.Ads().Show(AdsEnum.BANNER_BOTTOM, null);
                    StartCoroutine(IE_LoadAd());
                    break;
            }
        }

        public void BehaviourAwake()
        {
        }

        public void BehaviourStart()
        {
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
    }
}