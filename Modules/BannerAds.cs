using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BannerAds : MonoBehaviour
{
    RectTransform m_RectTrans;

    public void SetBannerHeight(float height)
    {
        if (m_RectTrans == null)
        {
            m_RectTrans = GetComponent<RectTransform>();
        }
        var bannerSize = m_RectTrans.sizeDelta;
        bannerSize.y = height;
        m_RectTrans.sizeDelta = bannerSize;
    }
}
