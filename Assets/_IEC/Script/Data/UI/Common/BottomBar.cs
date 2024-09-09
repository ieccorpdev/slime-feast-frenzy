using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace IEC
{
    public class BottomBar : _BaseBehaviour
    {
        [SerializeField] private BigHomeTab m_BigTab;
        [SerializeField] private RectTransform m_ChooseTab;
        [SerializeField] private RectTransform m_Layout;
        private List<HomeTab> m_ListTab = new ();

        private bool m_IsStart = true;
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

            var tabCount = m_ListTab.Count;
            for (int i = 0; i < tabCount; i++)
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
                    var isStart = m_IsStart;
                    StartCoroutine(IE_WaitRebuild(() => ChangeTabUI(!isStart)));
                };
            }
            m_BigTab.SetSize(tabCount);
            StartCoroutine(IE_WaitRebuild(() =>
            {
                m_ChooseTab.sizeDelta = new Vector2(m_BigTab.GetSize().x, m_ChooseTab.sizeDelta.y);
            }));
            m_ListTab[tabCount / 2].onClick.Invoke();
            m_IsStart = false;
        }

        IEnumerator IE_WaitRebuild(Action callback)
        {
            yield return null;
            callback?.Invoke();
        }
        
        private void SetTabSprite()
        {
            for (int i = 0; i < m_ListTab.Count; i++)
            {
                m_ListTab[i].SetData(HybridConfig.listTabConfig[i]);
            }
        }

        private void ChangeTabUI(bool isAnim)
        {
            if (!isAnim)
            {
                m_ChooseTab.anchoredPosition = new Vector2(m_BigTab.GetPosX(), m_ChooseTab.anchoredPosition.y);
                return;
            }

            m_ChooseTab.DOAnchorPosX(m_BigTab.GetPosX(), 0.2f).SetEase(Ease.OutQuad);
        }
    }
}