using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace IEC
{
    public class BottomBar : _BaseBehaviour
    {
        [SerializeField] private BigHomeTab m_BigTab;
        private List<HomeTab> m_ListTab = new ();
        
        private void Start()
        {
            var tabShow = HybridConfig.listTabConfig.Count - 1;
            var listHomeTab = GetComponentsInChildren<HomeTab>(true).ToList();
            listHomeTab.Remove(m_BigTab);
            for (int i = 0; i < listHomeTab.Count; i++)
            {
                if (i < tabShow)
                {
                    m_ListTab.Add(listHomeTab[i]);
                    listHomeTab[i].SetActive(true);
                }
                else
                {
                    listHomeTab[i].SetActive(false);
                }
            }
            m_ListTab.Add(m_BigTab);
            
            for (int i = 0; i < m_ListTab.Count; i++)
            {
                var tab = m_ListTab[i];
                tab.type = (Tab) i;
                tab.onClick = () =>
                {
                    Game.hybrid.currentTab = tab.type;
                    var index = (int) tab.type;
                    m_BigTab.transform.SetSiblingIndex((int)tab.type);
                    m_ListTab.Remove(m_BigTab);
                    m_ListTab.Insert(index, m_BigTab);
                    SetTabSprite();
                    m_BigTab.SetTabName();
                };
            }
            
            m_ListTab[m_ListTab.Count / 2].onClick.Invoke();
        }

        private void SetTabSprite()
        {
            for (int i = 0; i < m_ListTab.Count; i++)
            {
                m_ListTab[i].SetData(HybridConfig.listTabConfig[i]);
            }
        }
    }
}