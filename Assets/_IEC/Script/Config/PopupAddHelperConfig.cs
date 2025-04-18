using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/PopupAddHelperConfig")]
    public class PopupAddHelperConfig : ScriptableObject
    {
        public List<AddHelperData> listAddHelperData = new();

        public AddHelperData GetAddHelperData(ResourceType helperType)
        {
            return listAddHelperData.FirstOrDefault(x => x.helperType == helperType);
        }
    }

    [System.Serializable]
    public class AddHelperData
    {
        public ResourceType helperType;
        public Sprite helperIcon;
        public string title;
        [TextArea]
        public string helperDescription;
        public string highlightText;
        public int addGoldAmount = 3;
        public int goldCost;
    }
}
