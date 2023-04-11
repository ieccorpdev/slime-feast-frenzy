using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class DayChallenge : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_DayTxt;
        [SerializeField] private Image m_Completed;
        [SerializeField] private Image m_Choosing;
        [SerializeField] private Button m_Btn;
        [SerializeField] private Image m_ProgressBG;
        [SerializeField] private Image m_Progress;

        [Header("State Config")] 
        [SerializeField] private GameObject m_State;
        [SerializeField] private Sprite m_ProgressBGNormal;
        [SerializeField] private Sprite m_ProgressBGChoosing;
        [SerializeField] private Sprite m_ProgressNormal;
        [SerializeField] private Sprite m_ProgressChoosing;
        [SerializeField] private Color m_ColorNormal;
        [SerializeField] private Color m_ColorChoosing;
        [SerializeField] private Color m_ColorLock;

        public int day;

        public void SetClick(Action action)
        {
            m_Btn.onClick.AddListener(action.Invoke);    
        }
        
        public void SetDay(int day)
        {
            this.day = day;
            m_DayTxt.text = day.ToString();
        }

        public void SetDayOfWeek(string date)
        {
            day = 0;
            m_DayTxt.text = date;
            SetUnlock(false);
            m_DayTxt.color = m_ColorChoosing;
        }

        public void SetChoose(bool isChoose)
        {
            m_State.SetActive(true);
            m_Choosing.gameObject.SetActive(isChoose);
            m_DayTxt.color = m_ColorNormal;
            m_Progress.sprite = isChoose ? m_ProgressChoosing : m_ProgressNormal;
            m_ProgressBG.sprite = isChoose ? m_ProgressBGChoosing : m_ProgressBGNormal;
        }

        public void SetCompleted(bool isCompleted)
        {
            m_State.SetActive(true);
            m_Completed.gameObject.SetActive(isCompleted);
            m_DayTxt.gameObject.SetActive(!isCompleted);
            m_ProgressBG.gameObject.SetActive(!isCompleted);
            m_Progress.gameObject.SetActive(!isCompleted);
        }

        public void SetUnlock(bool isUnlock)
        {
            m_Btn.enabled = isUnlock;
            if (!isUnlock)
            {
                m_DayTxt.color = m_ColorLock;
            }
            m_State.SetActive(isUnlock);
        }

        public void SetProgress(int step)
        {
            m_Progress.fillAmount = step * 1f / 2;
        }
    }
}
