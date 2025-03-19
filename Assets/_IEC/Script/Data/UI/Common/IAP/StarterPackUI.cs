using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace IEC
{
    public class StarterPackUI : PackUI
    {
        [SerializeField]
        private TextMeshProUGUI m_goldAmount, m_helperShuffleAmount, m_helperUndoAmount, m_helperClearAmount;

        protected override void Start()
        {
            base.Start();
            UpdateUI();
        }

        public override void UpdateUI()
        {
            m_goldAmount.text = "x" + m_Config.GetResourceValueByType(ResourceType.Gold);
            m_helperShuffleAmount.text = "x" + m_Config.GetResourceValueByType(ResourceType.HelperShuffle);
            m_helperUndoAmount.text = "x" + m_Config.GetResourceValueByType(ResourceType.HelperUndo);
            m_helperClearAmount.text = "x" + m_Config.GetResourceValueByType(ResourceType.HelperClear);
        }
    }
}
