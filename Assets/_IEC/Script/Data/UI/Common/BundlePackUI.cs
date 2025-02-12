using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace IEC
{
    public class BundlePackUI : PackUI
    {
        [Header("Custom Resource")]
        [SerializeField] private TMP_Text m_TxtResource1;
        [SerializeField] private TMP_Text m_TxtResource2, m_TxtResource3, m_TxtResource4, m_TxtResource5, m_TxtResource6;

        private Dictionary<ResourceType, TMP_Text> m_DictAmountResource = new Dictionary<ResourceType, TMP_Text>();

        protected override void Start()
        {
            base.Start();
            //TODO: Map ResourceType and text amount
            if (m_Config == null) return;
            foreach (var resourceType in m_DictAmountResource)
            {
                resourceType.Value.text = $"x{m_Config.GetResourceValueByType(resourceType.Key).ToString()}";
            }
        }
    }
}