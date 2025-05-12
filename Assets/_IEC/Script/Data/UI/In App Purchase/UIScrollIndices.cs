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
            scrollImageIndices[0].parentObj.SetActive(totalIndices == 4);
            scrollImageIndices[1].parentObj.SetActive(totalIndices == 3);
            scrollImageIndices[2].parentObj.SetActive(totalIndices == 2);
        }
        
        public void SetIndex(RectTransform go, int index)
        {
            scrollImageIndices[totalIndex == 4 ? 0 : totalIndex == 3 ? 1 : 2].SetIndex(go, index);
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