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

        [Button]
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
    }

}
