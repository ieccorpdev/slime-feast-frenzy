using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class CharacterTabSelectionButton : _BaseBehaviour
    {
        public CharacterTabType tabType;
        public Button btn;
        public GameObject btnActiveGroup;
        // Start is called before the first frame update
        void Start()
        {
            btn.onClick.AddListener(OnButtonClick);
        }

        private void OnEnable()
        {
            this.RegisterListener(EventList.GamePlay.CHANGE_TAB_CHARACTER_SHOP, OnChangeTabEvent);
        }

        private void OnDisable()
        {
            this.UnregisterListener(EventList.GamePlay.CHANGE_TAB_CHARACTER_SHOP, OnChangeTabEvent);
        }

        private void OnButtonClick()
        {
            this.RaiseEvent(EventList.GamePlay.CHANGE_TAB_CHARACTER_SHOP, tabType);
        }

        private void OnChangeTabEvent(object param)
        {
            var type = (CharacterTabType)param;
            
            SetButtonActive(tabType == type);
        }

        private void SetButtonActive(bool isActive)
        {
            btnActiveGroup.SetActive(isActive);
        }
    }

    public enum CharacterTabType
    {
        Character,
        Shard,
    }
}
