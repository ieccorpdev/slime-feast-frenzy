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
        completedGroup.SetActive(false);
        lockedGroup.SetActive(false);
        joinGroup.SetActive(false);
        claimRewardGroup.SetActive(false);
        timerGroup.SetActive(false);
        button.interactable = true;

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
                if (isMaxLevel)
                {
                    completedGroup.SetActive(true);
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
        //gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateUI();
    }
}