using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

namespace IEC
{
    public class SagaPath : EnhancedScrollerCellView
    {
        public int id;
        public List<ItemMap> listItem;
        public List<RectTransform> listPathExtend;

        public void SetData(int index, Action<int> nodeClick = null, Action<int> nodeChildClick = null)
        {
            id = index;
            for (int i = 0; i < listItem.Count; i++)
            {
                listItem[i].SetData(index * listItem.Count + i, nodeClick, nodeChildClick);
            }

            var ratio = 1f * Screen.height / Screen.width;
            var size_16_9 = 280f;
            var size = ratio / (16f / 9) * size_16_9;
            foreach (var t in listPathExtend)
            {
                var sizeDelta = t.sizeDelta;
                sizeDelta.y = size;
                t.sizeDelta = sizeDelta;
            }
        }
    }

}

