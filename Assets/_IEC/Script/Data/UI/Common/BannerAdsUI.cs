using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class BannerAdsUI : BaseBehaviour
    {
        public RectTransform RectTransform => SingleGet<RectTransform>();
        public Image Image => SingleGet<Image>();

        // Start is called before the first frame update
        void Start()
        {
            
        }
    }
}