using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using IEC.Helper;
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

    public void UpdateUI()
    {
        Config.eventLiveOpsConfig.GetTimeEndEvent(EventType.RelicHunt,Mathf.FloorToInt(_eventConfig.eventDurationInHours/24), PlayerData.GetDaysSinceDownload());
        completedGroup.SetActive(false);
        lockedGroup.SetActive(false);
        joinGroup.SetActive(false);
        claimRewardGroup.SetActive(false);
        timerGroup.SetActive(false);
        if (PlayerData.relicData.hasJoinEvent == false)
        {
            joinGroup.SetActive(true);
            return;
        }
        if (PlayerData.level < Config.relicConfig.levelUnlockFeature)
        {
            levelUnlockText.text = "Lvl. " + Config.relicConfig.levelUnlockFeature;
            lockedGroup.SetActive(true);
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
                countdownTimeSpan = PlayerData.relicData.eventTimeEnd - DateTime.Now;
                bool isEndEvent = countdownTimeSpan.TotalSeconds <= 0;
                if (isMaxLevel && !isEndEvent)
                {
                    completedGroup.SetActive(true);
                }
                else
                {
                    timerGroup.gameObject.SetActive(true);
                    timerText.text = DateTimeHelper.GetFormattedTimeHMS(
                        countdownTimeSpan);
                    if (isEndEvent)
                    {
                        OnTimeEventEnd();
                    }
                }
            }
        }
    }

    private void OnTimeEventEnd()
    {
        //gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateUI();
    }
}