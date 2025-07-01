using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using IEC.Common.Event;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace IEC
{
    public class BottomBar : ScreenBehaviour
    {
        [SerializeField] private BigHomeTab m_BigTab;
        [SerializeField] private RectTransform m_ChooseTab;
        [SerializeField] private RectTransform m_Layout;
        private List<HomeTab> m_ListTab = new ();

        private bool m_IsStart = true;
        private int m_TabCount;
        private bool isClick = false;

        public BigHomeTab BigHomeTab => m_BigTab;
        
        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            m_ListTab[m_TabCount / 2].onClick.Invoke();
            m_IsStart = false;
        }

        public TabType lastTabType;

        private void Init()
        {
            Game.hybrid.currentTab = TabType.Shop;
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
            m_BigTab.transform.SetAsLastSibling();
            m_TabCount = m_ListTab.Count;
            for (int i = 0; i < m_TabCount; i++)
            {
                var tab = m_ListTab[i];
                tab.type = HybridConfig.listTabConfig[i].type;
                

                
                tab.Init();
                tab.onClick = () =>
                {
                    if (isClick) return;
                    //lock leaderboard temporary
                    if(tab.type == TabType.LeaderBoard) return;

                    if (m_BigTab.type == tab.type) return;

                    if (tab.type != Game.hybrid.currentTab)
                    {
                        isClick = true;
                        DOVirtual.DelayedCall(0.5f, () =>
                        {
                            isClick = false;
                        });
                    }
                    lastTabType = Game.hybrid.currentTab;
                    Game.hybrid.currentTab = tab.type;
                    var index = tab.transform.GetSiblingIndex();
                    m_BigTab.transform.SetSiblingIndex(index);
                    m_ListTab.Remove(m_BigTab);
                    m_ListTab.Insert(index, m_BigTab);

                    SetTabSprite(lastTabType);
                    m_BigTab.SetTabName();
                    m_BigTab.SetAnimationSpineOn();
                    var isStart = m_IsStart;
                    EventController.RaiseEvent(EventList.GamePlay.ON_CHOOSE_TAB, new EventParam(this));
                    EventController.RaiseEvent(EventList.Audio.HOME_PAGE_NAV_SLIDE);
                    SetLockTabs();
                    StartCoroutine(IE_WaitRebuild(() => ChangeTabUI(!isStart)));
                };
            }
            m_BigTab.SetSize(m_TabCount);
            StartCoroutine(IE_WaitRebuild(() =>
            {
                // Debug.LogError(m_BigTab.GetSize().x);
                //m_ChooseTab.sizeDelta = new Vector2(m_BigTab.GetSize().x, m_ChooseTab.sizeDelta.y);
            }));
            
            //m_ListTab.ForEach(x => x.SetLock(false)); 
            StartCoroutine(IE_SetLockTabs());
        }
        [Sirenix.OdinInspector.Button]
        private void SetLockTabs()
        {
            //lock leaderboard and trophy for now
            m_ListTab.ForEach(x =>
            {
                x.SetLock(
                    x.type == TabType.LeaderBoard 
                    ||(x.type == TabType.Trophy && PlayerData.level <= Config.trophyRewardConfig.levelUnlock)
                    ||(x.type == TabType.Character && PlayerData.level < Config.characterShopConfig.levelUnlock)
                    );
            });
        }
        IEnumerator IE_SetLockTabs()
        {
            yield return new WaitForEndOfFrame();
            SetLockTabs();
        }
        IEnumerator IE_WaitRebuild(Action callback)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_Layout);
            yield return null;
            callback?.Invoke();
        }
        
        private void SetTabSprite(TabType lastTabType)
        {
            for (int i = 0; i < m_ListTab.Count; i++)
            {
                m_ListTab[i].SetData(HybridConfig.listTabConfig[i], m_ListTab[i] == m_BigTab,lastTabType);
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

        public HomeTab GetTab(TabType tabType)
        {
            return m_ListTab.Find(tab => tab.type == tabType);
        }

        public void SetOnlyTabShowNoti(TabType tabType)
        {
            foreach (var tab in m_ListTab)
            {
                tab.SetActiveNoti(tab.type == tabType);
            }
        }
    }
}
