using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class EcpmAdsModule : IAdsService
    {
        List<IAdsService> m_ListEcpmAds;
        IAdsService m_DefaultAds;

        MonoBehaviour m_LoadRunner;
        Coroutine m_LoadCoroutine;

        Dictionary<AdsEnum, bool> m_IsUseEcpm = new Dictionary<AdsEnum, bool>();

        public EcpmAdsModule(MonoBehaviour coroutineRunner, IAdsService defaultAds, List<IAdsService> listAds)
        {
            m_DefaultAds = defaultAds;
            m_ListEcpmAds = listAds;
            m_LoadRunner = coroutineRunner;
            m_LoadCoroutine = null;
        }

        public void SetUseEcpm(AdsEnum adsType, bool isUseEcpm)
        {
            m_IsUseEcpm[adsType] = isUseEcpm;
        }

        private bool IsUseEcpm(AdsEnum adsType)
        {
            if (m_IsUseEcpm.ContainsKey(adsType))
            {
                return m_IsUseEcpm[adsType];
            }
            return false;
        }

        IEnumerator IE_LoadAdsOptions(AdsEnum adsType)
        {
            for (int i = 0; i < m_ListEcpmAds.Count; i++)
            {
                var ecpmAds = m_ListEcpmAds[i];
                if (string.IsNullOrEmpty(ecpmAds.GetInfo(adsType)))
                {
                    continue;
                }

                if (m_DefaultAds.IsLoaded(adsType) && m_DefaultAds.GetEcpm(adsType) > ecpmAds.GetEcpm(adsType))
                {
                    AdsStateData.DebugLog(adsType + " Already Loaded default ads better ecpm: " + m_DefaultAds.GetEcpm(adsType));
                    break;
                }

                bool isLoadedBetterEcpm = false;
                for (int k = 0; k < i; k++)
                {
                    if (string.IsNullOrEmpty(m_ListEcpmAds[k].GetInfo(adsType)))
                    {
                        continue;
                    }
                    if (m_ListEcpmAds[k].IsLoaded(adsType))
                    {
                        AdsStateData.DebugLog(adsType + " Already Loaded ads ecpm: " + m_ListEcpmAds[k].GetEcpm(adsType));
                        isLoadedBetterEcpm = true;
                        break;
                    }
                }
                if (isLoadedBetterEcpm)
                {
                    break;
                }
                
                if (ecpmAds.IsLoaded(adsType))
                {
                    AdsStateData.DebugLog(adsType + " Already Loaded ads ecpm: " + ecpmAds.GetEcpm(adsType));
                    break;
                }
                else
                {
                    AdsStateData.DebugLog(adsType + " Load ads ecpm:" + ecpmAds.GetEcpm(adsType));
                    ecpmAds.Load(adsType, null);
                }

                yield return new WaitForSecondsRealtime(2.5f);
            }

            m_LoadCoroutine = null;
        }

        public string GetInfo(AdsEnum adsType)
        {
            return m_DefaultAds.GetInfo(adsType);
        }

        public bool IsAcceptGDPR()
        {
            return m_DefaultAds.IsAcceptGDPR();
        }

        public void Load(AdsEnum adsType, Action<AdsEventArgs> loadCallback)
        {
            if (IsUseEcpm(adsType))
            {
                if (m_LoadCoroutine != null)
                {
                    m_LoadRunner.StopCoroutine(m_LoadCoroutine);
                }

                m_LoadCoroutine = m_LoadRunner.StartCoroutine(IE_LoadAdsOptions(adsType));
            }
            else
            {
                m_DefaultAds.Load(adsType, loadCallback);
            }
        }

        public void Show(AdsEnum adsType, Action<AdsEventArgs> showCallback)
        {
            if (IsUseEcpm(adsType))
            {
                var best = m_DefaultAds;
                if (best.IsLoaded(adsType) == false)
                {
                    foreach (var option in m_ListEcpmAds)
                    {
                        if (option.IsLoaded(adsType))
                        {
                            best = option;
                            break;
                        }
                    }
                }

                foreach (var option in m_ListEcpmAds)
                {
                    if (option.GetEcpm(adsType) > best.GetEcpm(adsType) && option.IsLoaded(adsType))
                    {
                        best = option;
                    }
                }

                AdsStateData.DebugLog(adsType + " Show Best Ecpm: " + best.GetEcpm(adsType));
                best.Show(adsType, showCallback);
            }
            else
            {
                m_DefaultAds.Show(adsType, showCallback);
            }
        }

        public void SetOnClick(AdsEnum adsType, Action clickCallback)
        {
            if (IsUseEcpm(adsType))
            {
                foreach (var ads in m_ListEcpmAds)
                {
                    ads.SetOnClick(adsType, clickCallback);
                }
            }
            m_DefaultAds.SetOnClick(adsType, clickCallback);
        }

        public bool IsLoaded(AdsEnum adsType)
        {
            if (IsUseEcpm(adsType))
            {
                foreach (var ads in m_ListEcpmAds)
                {
                    if (ads.IsLoaded(adsType)) return true;
                }
            }
            return m_DefaultAds.IsLoaded(adsType);
        }

        public void Hide(AdsEnum adsType)
        {
            if (IsUseEcpm(adsType))
            {
                foreach (var ads in m_ListEcpmAds)
                {
                    ads.Hide(adsType);
                }
            }
            m_DefaultAds.Hide(adsType);
        }

        public double GetEcpm(AdsEnum adsType)
        {
            return m_DefaultAds.GetEcpm(adsType);
        }

        public float GetBannerHeight()
        {
            return m_DefaultAds.GetBannerHeight();
        }

        public void OnApplicationPause(bool isPause)
        {
        }

        public void OnApplicationQuit()
        {
        }
    }
}
