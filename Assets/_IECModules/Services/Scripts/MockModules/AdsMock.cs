using System;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class AdsMock : IAdsService
    {
        private bool m_IsTestLoaded;

        public AdsMock(bool isTestLoaded)
        {
            m_IsTestLoaded = isTestLoaded;
        }

        string IAdsService.GetInfo(AdsEnum adsType)
        {
            return "AdsMock";
        }

        bool IAdsService.IsAcceptGDPR()
        {
            return true;
        }

        void IAdsService.Load(AdsEnum adsType, Action<AdsEventArgs> loadCallback)
        {
            AdsStateData.SafeInvoke(loadCallback, m_IsTestLoaded ? AdsEventArgs.DONE : AdsEventArgs.FAIL);
        }

        void IAdsService.Show(AdsEnum adsType, Action<AdsEventArgs> showCallback)
        {
            AdsStateData.SafeInvoke(showCallback, AdsEventArgs.DONE_NO_SKIP);
        }

        void IAdsService.SetOnClick(AdsEnum adsType, Action clickCallback)
        {
        }

        bool IAdsService.IsLoaded(AdsEnum adsType)
        {
            return m_IsTestLoaded;
        }

        void IAdsService.Hide(AdsEnum adsType)
        {
        }

        double IAdsService.GetEcpm(AdsEnum adsType)
        {
            return 0;
        }

        public float GetBannerHeight()
        {
            float f = Screen.dpi / 160f;
            return 50f * f;
        }
    }
}

