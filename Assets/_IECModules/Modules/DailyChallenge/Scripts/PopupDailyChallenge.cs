using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupDailyChallenge : BasePopup
    {
        public int choosingMonth = -1;
        public int choosingDay = -1;
        public bool isInDailyMode;

        public bool IsChooseChallenge => choosingMonth > 0 && choosingDay > 0;

        List<DayChallenge> ListDay =>
            SingleCreateIfNotExist(() => GetComponentsInChildren<DayChallenge>().ToList());

        public PlayerSecretDailyChallenge DataDailyChallenge => Pool.Get<PlayerSecretDailyChallenge>();

        [SerializeField] private TMP_Text m_MonthTxt;
        [SerializeField] private TMP_Text m_StarProgressTxt;
        [SerializeField] private Image m_CrownProgress;
        [SerializeField] private Button m_BtnNext;
        [SerializeField] private Button m_BtnPrev;
        [SerializeField] private Button m_BtnPlay;
        [SerializeField] private Button m_BtnClose;
        [SerializeField] private Image m_EffectStar;

        private List<string> m_DateStr = new List<string> { "M", "T", "W", "T", "F", "S", "S" };

        private int m_FirstDayOfWeek;
        private int m_DayOfMonth;
        private PlayerSecretDailyChallenge.MonthChallenge m_MonthData;
        private int m_MonthIndex = -1;
        private DateTime m_NowDay = DateTime.Now;

        private DayChallenge m_DayChoose;

        private bool m_IsLockClick;

        private Vector2 m_EffectStartPos;

        private float m_StartAmount = 0.364f;
        private float m_EndAmount = 0.71f;

        private void Start()
        {
            m_BtnNext.onClick.AddListener(() =>
            {
                if (m_IsLockClick) return;
                m_MonthIndex++;
                choosingDay = 1;
                choosingMonth = m_MonthIndex;
                if (m_MonthIndex == DateTime.Now.Month)
                {
                    choosingDay = DateTime.Now.Day;
                }

                UpdateChallenge();
            });

            m_BtnPrev.onClick.AddListener(() =>
            {
                if (m_IsLockClick) return;
                m_MonthIndex--;
                choosingMonth = m_MonthIndex;
                choosingDay = 1;
                UpdateChallenge();
            });

            m_BtnPlay.onClick.AddListener(() =>
            {
                if (m_IsLockClick) return;
                choosingMonth = m_MonthIndex;
                isInDailyMode = true;
                Hide();

                // Controllers.Get<ServicesController>().PushEvent(new ClickBtnPlayDaily());
                // Controllers.Get<GameController>().OnClickPlayDailyChallenge();
            });

            m_BtnClose.onClick.AddListener(Hide);

            ListDay.ForEach(x =>
            {
                x.SetClick(() =>
                {
                    if (m_IsLockClick) return;
                    choosingDay = x.day;
                    choosingMonth = m_MonthIndex;
                    UpdateChallenge();
                });
            });

            m_EffectStartPos = m_EffectStar.rectTransform.anchoredPosition;
        }

        public override void Show()
        {
            DataDailyChallenge.SetupNewMonthChallenge();
            UpdateChallenge();
            m_EffectStar.gameObject.SetActive(false);
            base.Show();
        }

        public override void Hide()
        {
            // Game.isLockClick = false;
            base.Hide();
        }

        public DateTime GetLevelDate()
        {
            var date = DateTime.Now;
            if (IsChooseChallenge)
            {
                date = new DateTime(DataDailyChallenge.currentYear, choosingMonth, choosingDay);
            }
            return date;
        }

        void UpdateChallenge()
        {
            m_NowDay = DateTime.Now;
            if (choosingMonth < 0)
                choosingMonth = DateTime.Now.Month;
            if (choosingDay < 0)
                choosingDay = DateTime.Now.Day;
            if (m_MonthIndex < 0) m_MonthIndex = choosingMonth;
            m_MonthData = DataDailyChallenge.months[m_MonthIndex];
            m_FirstDayOfWeek = GetDayOfWeekIndex(1);
            m_DayOfMonth = DateTime.DaysInMonth(DataDailyChallenge.currentYear, m_MonthIndex);

            UpdateCalendar();

            m_MonthTxt.text = new DateTime(DataDailyChallenge.currentYear, m_MonthIndex, 1)
                .ToString("MMMM yyyy", new CultureInfo("en-US"));
            var countCompleted = DataDailyChallenge.GetCountCompletedOfMonth(m_MonthIndex);
            m_StarProgressTxt.text = $"{countCompleted}/{m_DayOfMonth}";
            var fillAmount = 1f * countCompleted / m_DayOfMonth;
            var amount = fillAmount * (m_EndAmount - m_StartAmount) + m_StartAmount;
            if (fillAmount >= 1) amount = 1;
            m_CrownProgress.fillAmount = amount;

            m_BtnNext.gameObject.SetActive(m_MonthIndex < m_NowDay.Month);
            m_BtnPrev.gameObject.SetActive(m_MonthIndex > 1);
        }

        private void UpdateCalendar()
        {
            for (int i = 0; i < ListDay.Count; i++)
            {
                var dayChallenge = ListDay[i];
                if (i < 7)
                {
                    dayChallenge.SetDayOfWeek(m_DateStr[i]);
                }
                else if (i < 7 + m_FirstDayOfWeek)
                {
                    dayChallenge.SetDayOfWeek("");
                }
                else if (i < 7 + m_FirstDayOfWeek + m_DayOfMonth)
                {
                    var day = i - m_FirstDayOfWeek - 6;
                    var state = m_MonthData.days[day];
                    var isChoose = day == choosingDay &&
                                   choosingMonth == m_MonthIndex;
                    dayChallenge.SetCompleted(state == 2);
                    dayChallenge.SetDay(day);
                    dayChallenge.SetChoose(isChoose);
                    dayChallenge.SetProgress(state);
                    dayChallenge.SetUnlock(m_MonthIndex < m_NowDay.Month
                                           || (m_MonthIndex == m_NowDay.Month && day <= m_NowDay.Day));
                    if (isChoose) m_DayChoose = dayChallenge;
                }
                else
                {
                    dayChallenge.SetDayOfWeek("");
                }
            }
        }

        int GetDayOfWeekIndex(int day)
        {
            var dayOfWeek = (int)(new DateTime(DataDailyChallenge.currentYear, m_MonthIndex, day).DayOfWeek);
            dayOfWeek = (dayOfWeek + 6) % 7;
            return dayOfWeek;
        }

        public void ShowEffectStar()
        {
            EnableAllBtn(false);
            m_EffectStar.rectTransform.anchoredPosition = m_EffectStartPos;
            m_EffectStar.gameObject.SetActive(true);
            m_DayChoose.SetCompleted(false);

            var timePlay = 1f;
            var scale = 1.5f;
            var delay = 0.1f;

            var seq = DOTween.Sequence();
            seq.Insert(delay, m_EffectStar.transform.DOMove(m_DayChoose.transform.position, timePlay)
                    .SetEase(Ease.InQuart)
                    .OnUpdate(() =>
                    {
                        var pos3D = m_EffectStar.rectTransform.anchoredPosition3D;
                        pos3D.z = 0;
                        m_EffectStar.rectTransform.anchoredPosition3D = pos3D;
                    }))
                .Insert(delay, m_EffectStar.transform.DOScale(scale, timePlay / 2).SetEase(Ease.Linear))
                .Insert(delay + timePlay / 2, m_EffectStar.transform.DOScale(1f, timePlay / 2).SetEase(Ease.Linear))
                .OnComplete(() =>
                {
                    m_DayChoose.SetCompleted(true);
                    m_EffectStar.gameObject.SetActive(false);
                    EnableAllBtn(true);
                });
        }

        void EnableAllBtn(bool isEnable)
        {
            m_IsLockClick = !isEnable;
            m_BtnClose.interactable = isEnable;
            m_BtnPlay.interactable = isEnable;
            m_BtnPrev.interactable = isEnable;
            m_BtnNext.interactable = isEnable;
        }
    }
}