using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class OguryController : MonoBehaviour, IController, IEventStart, IEventListener
    {
        OguryModule m_OguryModule;
        ServicesController m_Service => Controllers.Get<ServicesController>();
        ServiceData m_ServiceData => Pool.Get<ServiceData>();

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void BehaviourStart()
        {
            m_OguryModule = new OguryModule();
            m_OguryModule.Init(m_Service.config.OguryConfig, true, false);
            m_OguryModule.onShowThumbnail = () =>
            {
                m_Service.SetActiveBanner(AdsEnum.BANNER_BOTTOM, false);
            };
            m_OguryModule.onCloseThumbnail = () =>
            {
                m_Service.SetActiveBanner(AdsEnum.BANNER_BOTTOM, true);
            };

            StartCoroutine(IE_LoadThumbnail());
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.Service.INIT_AD_DONE:
                    if (m_OguryModule == null || !m_OguryModule.IsThumbnailShowing())
                    {
                        m_Service.SetActiveBanner(AdsEnum.BANNER_BOTTOM, true);
                    }
                    break;
            }
        }

        IEnumerator IE_LoadThumbnail()
        {
            var threeSecs = new WaitForSeconds(3f);
            while (true)
            {
                m_OguryModule.isNoAds = m_ServiceData.isNoAds;
                m_OguryModule.LoadThumbnail();
                yield return threeSecs;
            }
        }
    }
}
