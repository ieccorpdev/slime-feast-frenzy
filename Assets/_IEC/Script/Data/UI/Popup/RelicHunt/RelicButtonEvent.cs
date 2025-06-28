using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using IEC.Helper;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventType = IEC.EventType;

public class RelicButtonEvent : _BaseBehaviour
{
    public GameObject lockedGroup;
    public GameObject joinGroup;
    public GameObject claimRewardGroup;
    public GameObject completedGroup;
    public GameObject timerGroup;
    public TextMeshProUGUI levelUnlockText;
    public TextMeshProUGUI timerText;
    public Button button;
    private TimeSpan countdownTimeSpan;
    [SerializeField] private GameEventConfig _eventConfig;
    private SkeletonGraphic spine;

    private void Start()
    {
        spine = GetComponentInChildren<SkeletonGraphic>();
    }
    public bool IsEventCanShow()
    {   
        return Config.eventLiveOpsConfig.IsEventOnDay(EventType.RelicHunt, Config.relicConfig.eventDurationInHours/24, PlayerData.GetDaysSinceDownload()) &&
               Locator.Main.ScreenMain.trophyTab.IsEventCanShow(EventType.RelicHunt);
    }

    public void UpdateUI()
    {
        completedGroup.SetActive(false);
        lockedGroup.SetActive(false);
        joinGroup.SetActive(false);
        claimRewardGroup.SetActive(false);
        timerGroup.SetActive(false);
        button.interactable = true;
        gameObject.SetActive(IsEventCanShow());
        if (!IsEventCanShow())
        {
            return;
        }

        if (PlayerData.relicData.isNewEvent && PlayerData.relicData.lastEventRewards.Count > 0)
        {
            SetClaimLastEventState();
            return;
        }


        if (PlayerData.level < Config.relicConfig.levelUnlockFeature)
        {
            levelUnlockText.text = "Lvl. " + Config.relicConfig.levelUnlockFeature;
            lockedGroup.SetActive(true);
            spine?.gameObject.SetActive(false);
            button.interactable = false;
        }
        else
        {
            var relicLevel = PlayerData.relicData.curRelicLevel;
            var config = Config.relicConfig;
            var isMaxLevel = relicLevel == config.ArtifactLevels.Count;

            if (PlayerData.relicData.CheckIfHasReward(config))
            {
                claimRewardGroup.SetActive(true);
            }
            else
            {
                if (isMaxLevel && !PlayerData.relicData.CheckIfHasLockReward(config, true))
                {
                    completedGroup.SetActive(true);
                    spine?.gameObject.SetActive(false);
                    button.interactable = false;
                }
                else
                {
                    countdownTimeSpan = PlayerData.relicData.eventTimeEnd - DateTime.Now;
                    timerText.text = GetTimeFormat(countdownTimeSpan);
                    timerGroup.gameObject.SetActive(true);
                    bool isEndEvent = countdownTimeSpan.TotalSeconds <= 0;
                    if (isEndEvent)
                    {
                        OnTimeEventEnd();
                    }
                }
            }

            if (PlayerData.relicData.hasJoinEvent == false)
            {
                timerGroup.gameObject.SetActive(false);
                joinGroup.SetActive(true);
            }
            
        }
    }

    private string GetTimeFormat(TimeSpan time)
    {
        string outputString = "";

        var day = time.Days;
        var hour = time.Hours;
        var minute = time.Minutes;
        var seconds = time.Seconds;
        string days = day < 10 ? $"0{day}" : day.ToString();
        string hours = hour < 10 ? $"0{hour}" : hour.ToString();
        string minutes = minute < 10 ? $"0{minute}" : minute.ToString();
        string second = seconds.ToString();
        if (day > 0)
        {
            outputString = $"{days}d {hours}h";
        }
        else if (hour > 0)
        {
            outputString = $"{hours}h {minutes}m";
        }
        else
        {
            outputString = $"{minutes}m {second}s";
        }

        return outputString;
    }

    private void OnTimeEventEnd()
    {
        bool hasLastEventReward = PlayerData.relicData.lastEventRewards.Count > 0;
        if (hasLastEventReward)
        {
            SetClaimLastEventState();
        }
        else
        {
            gameObject.SetActive(false);
            UpdateTimeEnd();
        }
    }

    private static void UpdateTimeEnd()
    {
        DateTime timeEnd = Config.eventLiveOpsConfig.GetTimeEndEvent(EventType.RelicHunt,
            Mathf.FloorToInt(Config.relicConfig.eventDurationInHours / 24), PlayerData.GetDaysSinceDownload());
        PlayerData.relicData.eventTimeEnd = timeEnd;
    }

    private void SetClaimLastEventState()
    {
        timerGroup.gameObject.SetActive(false);
        claimRewardGroup.gameObject.SetActive(true);
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            Locator.Main.ClaimBonusRewardPanel.SetupRewardPanel(PlayerData.relicData.lastEventRewards,
                () =>
                {
                    if (IsEventCanShow())
                    {
                        Locator.Main.PopupRelicHunt.Show();
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }

                    PlayerData.relicData.lastEventRewards.Clear();
                    PlayerData.relicData.isNewEvent = false;
                },
                true);
            Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel(ClaimBonusRewardSkin.Gift_Box_3);
        });
    }


}