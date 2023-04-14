using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DG.Tweening;
using IEC.TrackingEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupDailyChallenge : BasePopup
    {
        DailyChallengeController dailyChallengeController => Controllers.Get<DailyChallengeController>();

        private List<DayChallenge> m_ListDay =>
            SingleCreateIfNotExist(() => GetComponentsInChildren<DayChallenge>().ToList());

        private PlayerSecretDailyChallenge playerDataDailyChallenge => Pool.Get<PlayerSecretDailyChallenge>();
        private Game Game => Pool.Get<Game>();

        [SerializeField] private TMP_Text m_MonthTxt;
        [SerializeField] private TMP_Text m_StarProgressTxt;
        [SerializeField] private Image m_CrownProgress;
        [SerializeField] private Button m_BtnNext;
        [SerializeField] private Button m_BtnPrev;
        [SerializeField] private Button m_BtnPlay;
        [SerializeField] private Button m_BtnClose;
        [SerializeField] private Image m_EffectStar;

        private List<string> m_DateStr = new List<string> {"M", "T", "W", "T", "F", "S", "S"};

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
                dailyChallengeController.dailyChallengeChoosingDay = 1;
                dailyChallengeController.dailyChallengeChoosingMonth = m_MonthIndex;
                if (m_MonthIndex == DateTime.Now.Month)
                {
                    dailyChallengeController.dailyChallengeChoosingDay = DateTime.Now.Day;
                }

                UpdateChallenge();
            });

            m_BtnPrev.onClick.AddListener(() =>
            {
                if (m_IsLockClick) return;
                m_MonthIndex--;
                dailyChallengeController.dailyChallengeChoosingMonth = m_MonthIndex;
                dailyChallengeController.dailyChallengeChoosingDay = 1;
                UpdateChallenge();
            });

            m_BtnPlay.onClick.AddListener(() =>
            {
                if (m_IsLockClick) return;
                dailyChallengeController.dailyChallengeChoosingMonth = m_MonthIndex;
                dailyChallengeController.PlayDailyChallenge();
                Hide();
                
                
             
                // Controllers.Get<ServicesController>().PushEvent(new ClickBtnPlayDaily());
                // Controllers.Get<GameController>().OnClickPlayDailyChallenge();
            });

            m_BtnClose.onClick.AddListener(Hide);

            m_ListDay.ForEach(x =>
            {
                x.SetClick(() =>
                {
                    if (m_IsLockClick) return;
                    dailyChallengeController.dailyChallengeChoosingDay = x.day;
                    dailyChallengeController.dailyChallengeChoosingMonth = m_MonthIndex;
                    UpdateChallenge();
                });
            });

            m_EffectStartPos = m_EffectStar.rectTransform.anchoredPosition;
        }

        public override void Show()
        {
            playerDataDailyChallenge.SetupNewMonthChallenge();
            UpdateChallenge();
            m_EffectStar.gameObject.SetActive(false);
            base.Show();
        }

        public override void Hide()
        {
            // Game.isLockClick = false;
            base.Hide();
        }

        void UpdateChallenge()
        {
            m_NowDay = DateTime.Now;
            if (dailyChallengeController.dailyChallengeChoosingMonth < 0)
                dailyChallengeController.dailyChallengeChoosingMonth = DateTime.Now.Month;
            if (dailyChallengeController.dailyChallengeChoosingDay < 0)
                dailyChallengeController.dailyChallengeChoosingDay = DateTime.Now.Day;
            if (m_MonthIndex < 0) m_MonthIndex = dailyChallengeController.dailyChallengeChoosingMonth;
            m_MonthData = playerDataDailyChallenge.monthChallenge[m_MonthIndex];
            m_FirstDayOfWeek = GetDayOfWeekIndex(1);
            m_DayOfMonth = DateTime.DaysInMonth(playerDataDailyChallenge.currentYear, m_MonthIndex);

            UpdateCalendar();

            m_MonthTxt.text = new DateTime(playerDataDailyChallenge.currentYear, m_MonthIndex, 1)
                .ToString("MMMM yyyy", new CultureInfo("en-US"));
            var countCompleted = playerDataDailyChallenge.GetCountCompletedOfMonth(m_MonthIndex);
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
            for (int i = 0; i < m_ListDay.Count; i++)
            {
                var dayChallenge = m_ListDay[i];
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
                    var state = m_MonthData.dayChallenge[day];
                    var isChoose = day == dailyChallengeController.dailyChallengeChoosingDay &&
                                   dailyChallengeController.dailyChallengeChoosingMonth == m_MonthIndex;
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
            var dayOfWeek = (int) (new DateTime(playerDataDailyChallenge.currentYear, m_MonthIndex, day).DayOfWeek);
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