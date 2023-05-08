using EnhancedUI.EnhancedScroller;
using IEC.SagaMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC.LevelCollection
{
    public class BaseLevelCollectionScroller : BaseSagaMapScroller
    {
        public float bigItemHeight;

        public override void TestInit()
        {
            SetLevelProgress(12, 21, 30);
            InitScroller();
        }

        public override float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            var scale = AspectRatioScaler.GetScale(RectTransform.rect.size, standardFullSize);
            return bigItemHeight * scale;
        }

        public override void JumpToLevelProgress()
        {
            var numberLevelPerPath = NumberLevelPerPath;
            var indexJumpTo = (MaxUnlockLevel - 1) / numberLevelPerPath;

            var pathMinLevel = indexJumpTo * numberLevelPerPath + 1;

            var prefabSagaPath = prefabBigItem as BaseSagaPath;
            var offset = prefabSagaPath.GetJumpOffset(MaxUnlockLevel - pathMinLevel);

            // jump 1 index before the true index
            if (indexJumpTo >= 1)
            {
                indexJumpTo -= 1;
            }

            //Debug.Log($"{indexJumpTo} {offset}");

            JumpToIndex(indexJumpTo, offset);
        }
    }
}