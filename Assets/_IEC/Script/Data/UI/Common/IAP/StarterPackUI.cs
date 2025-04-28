using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace IEC
{
    public class StarterPackUI : PackUI
    {
        [SerializeField]
        private TextMeshProUGUI m_goldAmount; //, m_helperShuffleAmount, m_helperUndoAmount, m_helperClearAmount;

        protected override void Start()
        {
            base.Start();
            UpdateUI();
        }

        public override void UpdateUI()
        {
            m_goldAmount.text = m_Config.GetResourceValueByType(ResourceType.Gold).ToString();
            //_helperShuffleAmount.text = "x" + m_Config.GetResourceValueByType(ResourceType.HelperShuffle);
            //_helperUndoAmount.text = "x" + m_Config.GetResourceValueByType(ResourceType.HelperUndo);
            //_helperClearAmount.text = "x" + m_Config.GetResourceValueByType(ResourceType.HelperClear);
        }
    }
}
