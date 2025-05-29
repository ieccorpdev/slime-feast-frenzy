using System;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class UIScrollIndices : _BaseBehaviour
    {
        public int totalIndex = 4;
        public ScrollImageIndices[] scrollImageIndices;

        public void OnTotalsItemUpdate(int totalIndices)
        {
            totalIndex = totalIndices;
            for (int i = 0; i < scrollImageIndices.Length; i++)
            {
                scrollImageIndices[i].parentObj.SetActive((5 - i) % totalIndices == 0);
            }
        }
        
        public void SetIndex(RectTransform go, int index)
        {
            scrollImageIndices[5 - totalIndex].SetIndex(go, index);
        }
    }

    [Serializable]
    public class ScrollImageIndices
    {
        public GameObject parentObj;
        public List<Transform> imgOn;
        public List<Transform> imgOff;
        
        public void SetIndex(RectTransform go, int index)
        {
            for (int i = 0; i < imgOn.Count; i++)
            {
                imgOn[i].gameObject.SetActive(i == index);
            }
            
            for (int i = 0; i < imgOff.Count; i++)
            {
                imgOff[i].gameObject.SetActive(i != index);
            }
        }
    }
}