using EnhancedUI.EnhancedScroller;
using IEC.MapScroller;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class BaseSagaMapScroller : BaseMapScroller
    {
        public Vector2 standardFullSize = new Vector2(1080, 1920);

        public bool IsHaveLevelProgress { get; protected set; }
        public int CurrentLevel { get; protected set; }
        public int MaxUnlockLevel { get; protected set; }

        public int NumberLevelPerPath => ((BaseSagaPath)prefabBigItem).listItem.Count;
        public int MaxLevelNeedToShow { get; protected set; }
        public int NumberOfPath => MaxLevelNeedToShow / NumberLevelPerPath + (MaxLevelNeedToShow % NumberLevelPerPath > 0 ? 1 : 0);
        public int NumberLevelOnMap => NumberOfPath * NumberLevelPerPath;

        public override void InitScroller()
        {
            if (IsHaveLevelProgress)
            {
                InitScroller(NumberOfPath);
                JumpToLevelProgress();
                return;
            }

            MaxLevelNeedToShow = Mathf.Max(MaxLevelNeedToShow, NumberLevelPerPath);
            InitScroller(NumberOfPath);
        }

        public override float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            var scale = AspectRatioScaler.GetScale(RectTransform.rect.size, standardFullSize);
            return base.GetCellViewSize(scroller, dataIndex) * scale;
        }

        public override EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var sagaPath = base.GetCellView(scroller, dataIndex, cellIndex) as BaseSagaPath;
            if (IsHaveLevelProgress)
            {
                sagaPath.SetLevelProgress(CurrentLevel, MaxUnlockLevel);
            }
            return sagaPath;
        }

        [Button]
        public void TestSetLevelProgress()
        {
            SetLevelProgress(8, 15, 16);
            InitScroller();
        }

        public virtual void SetLevelProgress(int currentLevel, int maxUnlockLevel, int maxLevelNeedToShow)
        {
            IsHaveLevelProgress = true;
            CurrentLevel = currentLevel;
            MaxUnlockLevel = maxUnlockLevel;
            MaxLevelNeedToShow = maxLevelNeedToShow;
        }

        public virtual void JumpToLevelProgress()
        {
            var numberLevelPerPath = NumberLevelPerPath;
            var indexJumpTo = (MaxUnlockLevel - 1) / numberLevelPerPath;

            var pathMinLevel = indexJumpTo * numberLevelPerPath + 1;
            var pathMaxLevel = indexJumpTo * numberLevelPerPath + numberLevelPerPath;

            var offset = MathHelper.Remap(pathMinLevel, pathMaxLevel, 0.2f, -0.4f, MaxUnlockLevel);

            //Debug.Log($"{indexJumpTo} {offset}");

            JumpToIndex(indexJumpTo, offset);
        }
    }

}
