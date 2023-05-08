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

        public bool isAutoTestInit = false;
        public bool isReverseScroll = true;

        public int numberBigItem = DEFAULT_NUMBER_BIGITEM;
        public int IndexLastData => numberBigItem - 1;

        public RectTransform RectTransform => SingleCreateIfNotExist("RectTransform", () => gameObject.GetComponent<RectTransform>());

        public const int DEFAULT_NUMBER_BIGITEM = 5;

        protected virtual void Start()
        {
            if (isAutoTestInit)
            {
                TestInit();
            }
        }

        public virtual void TestInit()
        {
            InitScroller();
        }

        public virtual void InitScroller()
        {
            InitScroller(DEFAULT_NUMBER_BIGITEM, 0);
        }

        public virtual void InitScroller(int numberBigItem)
        {
            if (numberBigItem == 0)
            {
                Debug.LogError("InitScroller numberBigItem = 0");
                numberBigItem = DEFAULT_NUMBER_BIGITEM;
            }
            this.numberBigItem = numberBigItem;
            Scroller.Delegate = this;
            Scroller.ReloadData();
        }

        public virtual void InitScroller(int numberBigItem, int jumpIndex, float cellOffset = 0f)
        {
            if (numberBigItem == 0)
            {
                Debug.LogError("InitScroller numberBigItem = 0");
                numberBigItem = DEFAULT_NUMBER_BIGITEM;
            }
            this.numberBigItem = numberBigItem;
            Scroller.Delegate = this;
            Scroller.ReloadData();

            JumpToIndex(jumpIndex, cellOffset);
        }

        public virtual void JumpToIndex(int trueIndex, float cellOffset = 0f)
        {
            StartCoroutine(IEnumDelayJump(trueIndex, cellOffset));
        }

        int GetTrueIndex(int scrollIndex)
        {
            return ConvertIndex(scrollIndex);
        }

        int GetScrollIndex(int trueIndex)
        {
            return ConvertIndex(trueIndex);
        }

        int ConvertIndex(int index)
        {
            if (isReverseScroll)
                return IndexLastData - index;
            return index;
        }

        protected virtual IEnumerator IEnumDelayJump(int trueIndex, float cellOffset = 0f)
        {
            yield return null;

            Scroller.JumpToDataIndex(GetScrollIndex(trueIndex), 0f, cellOffset);
        }

        public virtual EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            var cellView = scroller.GetCellView(prefabBigItem) as BaseMapBigItem;
            var trueIndex = GetTrueIndex(dataIndex);

            cellView.SetScroller(Scroller);
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