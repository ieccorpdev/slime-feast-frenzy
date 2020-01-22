using System.Collections;
using UnityEngine;

namespace IEC
{
    public class AdsController : MonoBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
        ServicesController m_Service;
        Game m_Game;

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
                //case EventList.GamePlay.PASS_LEVEL:
                //    Debug.Log("PASS_LEVEL");
                //    m_Service.ShowFullAdsWithTime(m_Game.timeDelayFullAds);
                //    break;
                case EventList.Service.INIT_AD_DONE:
                    m_Service.Ads().Load(AdsEnum.BANNER_BOTTOM, null);
                    m_Service.Ads().Show(AdsEnum.BANNER_BOTTOM, null);
                    StartCoroutine(IE_LoadAd());
                    break;
            }
        }

        public void BehaviourAwake()
        {
            m_Service = Controllers.Get<ServicesController>();
            m_Game = Pool.Get<Game>();
        }

        public void BehaviourStart()
        {
        }

        IEnumerator IE_LoadAd()
        {
            var oneSec = new WaitForSecondsRealtime(1f);
            while (true)
            {
                m_Service.Ads().Load(AdsEnum.INTERSTITIAL, null);
                m_Service.Ads().Load(AdsEnum.REWARD, null);
                yield return oneSec;
            }
        }
    }
}