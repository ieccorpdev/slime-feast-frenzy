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
        
        public override void InitScroller()
        {
            var numberLevelPerPath = 5;
            var numberLevelAhead = 50;
            var numberOfPath = (PlayerData.level + numberLevelAhead) / numberLevelPerPath;
            var indexJumpTo = PlayerData.level / numberLevelPerPath;

            InitScroller(numberOfPath, indexJumpTo);
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
            SetLevelProgress(8, 12);
            InitScroller();
        }

        public virtual void SetLevelProgress(int currentLevel, int maxUnlockLevel)
        {
            IsHaveLevelProgress = true;
            CurrentLevel = currentLevel;
            MaxUnlockLevel = maxUnlockLevel;
        }
    }

}
