using EnhancedUI.EnhancedScroller;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;

namespace IEC.MapScroller
{
    public class BaseMapScroller : _BaseBehaviour, IEnhancedScrollerDelegate
    {
        public EnhancedScroller Scroller => SingleGet<EnhancedScroller>();

        public BaseMapBigItem prefabBigItem;

        public bool isAutoInit = true;

        public int numberBigItem = DEFAULT_NUMBER_BIGITEM;
        public int IndexLastData => numberBigItem - 1;

        public RectTransform RectTransform => SingleCreateIfNotExist("RectTransform", () => gameObject.GetComponent<RectTransform>());

        public const int DEFAULT_NUMBER_BIGITEM = 5;

        protected virtual void Start()
        {
            if (isAutoInit)
            {
                InitScroller();
            }
        }

        public virtual void InitScroller()
        {
            InitScroller(DEFAULT_NUMBER_BIGITEM, 0);
        }

        public virtual void InitScroller(int numberBigItem, int jumpIndex)
        {
            if (numberBigItem == 0)
            {
                Debug.LogError("InitScroller numberBigItem = 0");
                numberBigItem = DEFAULT_NUMBER_BIGITEM;
            }
            this.numberBigItem = numberBigItem;
            Scroller.Delegate = this;
            Scroller.ReloadData();

            JumpToIndex(jumpIndex);
        }

        public virtual void JumpToIndex(int trueIndex)
        {
            StartCoroutine(IEnumDelayJump(trueIndex));
        }

        protected IEnumerator IEnumDelayJump(int trueIndex)
        {
            yield return null;

            var index = IndexLastData - trueIndex;
            Scroller.JumpToDataIndex(index);
        }

        public virtual EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var cellView = scroller.GetCellView(prefabBigItem) as BaseMapBigItem;
            var trueIndex = IndexLastData - dataIndex;

            cellView.SetHeader(trueIndex == 0);
            cellView.SetFooter(trueIndex == IndexLastData);
            cellView.SetData(trueIndex);

            return cellView;
        }

        public virtual float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return RectTransform.rect.height;
        }

        public virtual int GetNumberOfCells(EnhancedScroller scroller)
        {
            return numberBigItem;
        }
    }
}