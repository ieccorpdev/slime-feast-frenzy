using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace IEC
{
    public class HomeTab : _BaseBehaviour
    {
        public TabType type;
        public Action onClick;
        [SerializeField] private Image m_Icon;
        [SerializeField] private Image m_IconNoti;
        [SerializeField] private GameObject m_Lock;
        private Button m_Btn => SingleGet<Button>();
        private bool m_IsLock;
        public SpineAnimOT iconSpine;
        public bool isBigTabOn;
        public RectTransform IconTransform => iconSpine != null? iconSpine.GetComponent<RectTransform>() : m_Icon.GetComponent<RectTransform>();
        public SpineAnimOT shopIconSpine;
        public SpineAnimOT homeIconSpine;
        public SpineAnimOT leaderBoardIconSpine;
        public SpineAnimOT characterIconSpine;
        private float localPosIconOff = -72.48f;
        private void Start()
        {
            m_Btn.onClick.AddListener(()=>
            {
                if(m_IsLock) return;
                onClick?.Invoke();
            });
            
            SetActiveNoti(false);
        }

        public void Init()
        {
            SetSpineIcon(); 
            iconSpine.gameObject.SetActive(true);
            var state = iconSpine.ske.AnimationState;
            state.ClearTrack(0);
            state.SetAnimation(0, "off", false);
        }

        private void SetSpineIcon()
        {
            shopIconSpine.gameObject.SetActive(false);
            homeIconSpine.gameObject.SetActive(false);
            leaderBoardIconSpine.gameObject.SetActive(false);
            characterIconSpine.gameObject.SetActive(false);
            m_Icon.gameObject.SetActive(false);
            switch (type)
            {
                case TabType.Home:
                    iconSpine = homeIconSpine;
                    break;
                case TabType.Shop:
                    iconSpine = shopIconSpine;
                    break;
                case TabType.LeaderBoard:
                case TabType.Trophy:
                    iconSpine = leaderBoardIconSpine;
                    break;                
                case TabType.Character:
                    iconSpine = characterIconSpine;
                    break;
            }
        }

        public void SetData(TabConfig config, bool isOn,TabType lastTabType)
        {
            type = config.type;
            //m_Icon.sprite = isOn ? config.spriteOn : config.spriteOff;
            //m_Icon.SetNativeSize();
            if (!isOn)
            {
                SetAnimationSpineOff(lastTabType);
            }
        }
        
        public void SetLock(bool isLock)
        {
            m_IsLock = isLock;
            //m_Icon.gameObject.SetActive(!isLock);

            if (iconSpine)
            {
                iconSpine.gameObject.SetActive(!isLock);
            }
            if (type == Locator.Main.ScreenMain.bottomBar.lastTabType)
            {
                SetAnimationSpineOff(type);
            }
            m_Lock.SetActive(isLock);
            m_Lock.transform.localPosition = Vector3.up * -8;
            //if(isLock) m_Lock.transform.SetAsLastSibling();
        }

        public void SetActiveNoti(bool isShowNoti)
        {
            if (m_IconNoti == null)
                return;
            
            m_IconNoti.gameObject.SetActive(isShowNoti);
        }
        public void SetAnimationSpineOn()
        {
            SetSpineIcon();
            if (iconSpine && !m_IsLock)
            {
                iconSpine.gameObject.SetActive(true);
                iconSpine.SetAnimation( "on" ,false,null);
            }
        }
        public void SetAnimationSpineOff(TabType lastTabType)
        {
            SetSpineIcon();
            if(m_IsLock) return;
            iconSpine.gameObject.SetActive(true);
            var skeleton = iconSpine.ske.Skeleton;
            var state = iconSpine.ske.AnimationState;

            state.ClearTrack(0);

            var isSameTab = lastTabType == type;
            var targetAnimation = isSameTab ? skeleton.Data.FindAnimation("on") : skeleton.Data.FindAnimation("off");

            if (targetAnimation != null)
            {
                targetAnimation.Apply(
                    skeleton,
                    0f,
                    targetAnimation.Duration,
                    false,
                    null,
                    1f,
                    MixBlend.Replace,
                    MixDirection.In
                );
            }
            iconSpine.transform.localPosition = Vector3.up * localPosIconOff;
            iconSpine.ske.Update(0);
            iconSpine.ske.LateUpdate();

            if (isSameTab)
            {
                state.SetAnimation(0, "off", false);
            }
        }
    }
}