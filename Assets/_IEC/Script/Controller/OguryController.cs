using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class OguryController : _BaseBehaviour, IController, IEventListener
    {
        OguryModule m_OguryModule;
        ServiceData m_ServiceData => Pool.Get<ServiceData>();

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
                case EventList.Service.REMOTE_CONFIG_LOADED:
                    Game.isShowThumbnail = Services.DataRemote().GetBool("OguryThumbnail", false);
                    CheckShowBanner(Game.isShowThumbnail);
                    break;
            }
        }

        private void CheckShowBanner(bool isShowThumbnail)
        {
            if (isShowThumbnail)
            {
                m_OguryModule = new OguryModule();
                m_OguryModule.Init(Services.config.OguryConfig, true, false);
                m_OguryModule.onShowThumbnail = () =>
                {
                    Services.SetActiveBanner(AdsEnum.BANNER_BOTTOM, false);
                };
                m_OguryModule.onCloseThumbnail = () =>
                {
                    Services.SetActiveBanner(AdsEnum.BANNER_BOTTOM, true);
                };

                StartCoroutine(IE_LoadThumbnail());
            }

            if (!isShowThumbnail || m_OguryModule == null || !m_OguryModule.IsThumbnailShowing())
            {
                Services.SetActiveBanner(AdsEnum.BANNER_BOTTOM, true);
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
