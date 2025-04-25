using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupAddHelper : BasePopup
    {
        public Image helperIcon;
        public TextMeshProUGUI titleText;

        public Button buyGoldButton;

        public TextMeshProUGUI helperDescription;
        public TextMeshProUGUI goldAddAmountText;
        public TextMeshProUGUI goldAmountText;

        [Header("Highlight text")] 
        public float textHighlightSize;
        public Color textHighlightColor;
        public Material materialPreset;
        
        public ResourceType HelperType { get; private set; }
        
        public AddHelperData AddHelperData { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            
            SetEvent(buyGoldButton,OnBuyGold);
        }

        public void SetData(ResourceType helperType)
        {
            HelperType = helperType;

            AddHelperData = PlayConfig.popupAddHelperConfig.GetAddHelperData(HelperType);

            titleText.text = AddHelperData.title;

            helperIcon.sprite = AddHelperData.helperIcon;
            helperIcon.SetNativeSize();
            
            helperDescription.text = AddHelperData.helperDescription;
            HighlightText(helperDescription, AddHelperData.highlightText);
            goldAmountText.text = $"{AddHelperData.goldCost}";
            goldAddAmountText.text = $"x{AddHelperData.addGoldAmount}";
        }

        public override void Show()
        {
            // if (!Game.isUsePopupRefillHelper)
            //     return;
            
            base.Show();
        }
 
        private void OnBuyGold()
        {
            if (PlayerData.CanSpendCurrency(ResourceType.Gold, AddHelperData.goldCost))
            {
                PlayerData.AddCurrency(HelperType, AddHelperData.addGoldAmount);
                PlayerData.SubtractCurrency(ResourceType.Gold, AddHelperData.goldCost);
                Hide();
            }
            else
            {
                Locator.Main.PopupGoldOffer.Show();
            }
        }
        
        void HighlightText(TextMeshProUGUI tmpText, string word)
        {
            if (tmpText == null || string.IsNullOrEmpty(word) || materialPreset == null) return;

            string materialName = materialPreset.name;
            string taggedWord = $"<size={textHighlightSize}><color={ColorToHex(textHighlightColor)}><material={materialName}>{word}</material></color></size>";

            string modifiedText = tmpText.text.Replace(word, taggedWord);
            tmpText.text = modifiedText;
        }
        
        public string ColorToHex(Color color)
        {
            Color32 color32 = color; // Converts to 0–255 range
            return $"#{color32.r:X2}{color32.g:X2}{color32.b:X2}";
        }
        
    }
}
