using IEC.MapScroller;
using System.Collections.Generic;
using System.Linq;

namespace IEC
{
    public class BaseSagaPath : BaseMapBigItem
    {
        public List<BaseSagaPathItem> listItem;

        public virtual int MinLevel => ScrollIndex * listItem.Count + 1;
        public virtual int MaxLevel => ScrollIndex * listItem.Count + listItem.Count;

        public override void SetData(int scrollIndex)
        {
            ScrollIndex = scrollIndex;
            gameObject.name = $"SagaPath-{scrollIndex}";

            for (int i = 0; i < listItem.Count; i++)
            {
                var level = scrollIndex * listItem.Count + i + 1;
                listItem[i].SetData(level);
            }
        }

        [NaughtyAttributes.Button]
        public void UpdateListItem()
        {
            listItem = GetComponentsInChildren<BaseSagaPathItem>().ToList();
        }

        public virtual void SetLevelProgress(int currentLevel, int maxUnlockLevel)
        {
            var scrollIndex = ScrollIndex;
            for (int i = 0; i < listItem.Count; i++)
            {
                var level = scrollIndex * listItem.Count + i + 1;
                listItem[i].SetLevelState(level > maxUnlockLevel, level == currentLevel);
            }
        }

        public virtual float GetJumpOffset(int itemIndex)
        {
            return listItem[itemIndex].GetJumpOffset();
        }
    }

}

