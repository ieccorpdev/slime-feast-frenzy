using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace IEC
{
    public class ItemShop : MonoBehaviour
    {
        [SerializeField]
        private Button m_BtnSelect;

        [SerializeField]
        private TextMeshProUGUI m_TxtItemName;

        [SerializeField]
        private Image m_ImgIcon;

        [SerializeField]
        private GameObject m_SelectedObj, m_BlockObj;

        private ItemShopType m_ItemShopType;

        private int m_Index;

        public void OnInit(int index, ItemShopConfig itemConfig)
        {
            m_Index = index;
            m_ItemShopType = itemConfig.itemShopType;

            m_ImgIcon.sprite = itemConfig.spriteIcon;
            m_TxtItemName.text = itemConfig.name;

            m_BtnSelect.onClick.RemoveAllListeners();
            m_BtnSelect.onClick.AddListener(OnSelected);
        }

        private void OnSelected()
        {
            EventController.RaiseEvent(EventList.GamePlay.SHOP_SELECT_ITEM, new EventParam(this, m_ItemShopType, m_Index));
        }

        public void OnUpdateData(bool isUnlocked, bool isSelected)
        {
            m_SelectedObj.SetActive(isSelected);
            m_BlockObj.SetActive(!isUnlocked);
        }
    }
}
