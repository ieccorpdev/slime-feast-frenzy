using IEC.MapScroller;
using System.Collections.Generic;
using System.Linq;

namespace IEC
{
    public class BaseSagaPath : BaseMapBigItem
    {
        public int scrollIndex;
        public List<BaseSagaPathItem> listItem;

        public override void SetData(int scrollIndex)
        {
            gameObject.name = $"SagaPath-{scrollIndex}";

            this.scrollIndex = scrollIndex;
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
    }

}

