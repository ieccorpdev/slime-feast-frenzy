using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IEC;
using UnityEngine;
using UnityEngine.UI;
using _IECModules;
using DG.Tweening;
using Sirenix.OdinInspector;
using IEC.Common.Event;
using TMPro;
using EventType = IEC.EventType;

public class TrophyScreenTab : ScreenNavigateUI
{
    public List<ItemTrophyTab> listAllTrophy;
    public Button tutorialTtrophyButton;
    public TutorialJourney tutorialTrophyPanel;

    public void Start()
    {
        tutorialTtrophyButton.onClick.AddListener(OnClickButtonTutorial);
    }

    public override void Show()
    {
        Locator.Service.GameResourceDisplay.HideDisplayUnderBG();
        tutorialTrophyPanel.gameObject.SetActive(false);
        if (!PlayerData.trophyData.isShowJourneyTutorial)
        {
            PlayerData.trophyData.isShowJourneyTutorial = true;
            OnClickButtonTutorial();
        }
        LoadData();
    }
    void OnClickButtonTutorial()
    {
        tutorialTrophyPanel.gameObject.SetActive(true);
        tutorialTrophyPanel.Show();
    }
    public void LoadData()
    {

        foreach (var item in listAllTrophy)
        {
            var crState = GetStateTrophy(item.type);
            item.UpdateStateAquarium(crState);
            //int timesCollected = PlayerData.trophyData.GetCurrentTimeComplete(item.type);
            //Test
            if (crState is ItemTrophyState.Idle or ItemTrophyState.Missing )
            {
                var data = PlayerData.trophyData.GetMinMaxProgressBar(item.type);
                item.SetProgressBar((int)data.x,(int)data.y);
                item.dataEventTime = GetStatusEventConfig(item.type);
            }
            item.RegisterEventButton();
        }
    }
    
    public ItemTrophyState GetStateTrophy(TrophyType type)
    {
        if (PlayerData.level < GetLevelUnlockTrophy(type))
        {
            return ItemTrophyState.Locked;
        }

        int timesCollected = PlayerData.trophyData.GetCurrentTimeComplete(type);
        int trophyCollected = PlayerData.trophyData.GetCurrentCollectedTrophy(type);
        if (trophyCollected >=4 )
        {
            return ItemTrophyState.Complete;
        }
        if (timesCollected == 0 && trophyCollected == 0)
        {
            return ItemTrophyState.Missing;
        }
        if(PlayerData.trophyData.IsTrophyCanCollect(type)){
        {
            return ItemTrophyState.New;
        }}
        return ItemTrophyState.Idle;
    }
    public int GetLevelUnlockTrophy(TrophyType type)
    {
        switch ( type)
        {
            case TrophyType.GrandVoyage:
                return 1;
                break;
            case TrophyType.SharkFrenzyForce:
                return 1;
                break;
            case TrophyType.UnderseaVoyage:
                return Config.specialObjectConfig.kingdomSpecialObjectSpawnData.levelStartSpawn;
                break;
            case TrophyType.SharkRace:
                return Config.AllGameEventConfig.GetLevelUnlockEvent(EventType.SharkRace);
                break;
            case TrophyType.ChompOff:
                return  Config.AllGameEventConfig.GetLevelUnlockEvent(EventType.ChompOff);
                break;
            case TrophyType.OceanGift:
                return 1;
                break;
            case TrophyType.StarRace:
                return Config.AllGameEventConfig.GetLevelUnlockEvent(EventType.StarChase);
                break;
            case TrophyType.RelicHunt:
                return Config.AllGameEventConfig.GetLevelUnlockEvent(EventType.RelicHunt);
                break;
            default:
                return 999;
        }
        return 999;
    }
    public string GetRealNameTrophy(TrophyType type)
    {
        switch (type)
        {
            case TrophyType.GrandVoyage:
                return "Grand Voyage";
                break;
            case TrophyType.SharkFrenzyForce:
                return "Shark Frenzy Force";
                break;
            case TrophyType.UnderseaVoyage:
                return "Undersea Voyage";
                break;
            case TrophyType.SharkRace:
                return "Shark Race";
                break;
            case TrophyType.ChompOff:
                return "Chomp-off";
                break;
            case TrophyType.OceanGift:
                return "Ocean Gift";
                break;
            case TrophyType.StarRace:
                return "Star Race";
                break;
            case TrophyType.RelicHunt:
                return "Relic Hunt";
                break;
        }

        return "Error";
    }

    //HardCode
    public string GetColorTrophy(TrophyType type)
    {
        //    Bronze, Silver, Gold, Diamond
        if (PlayerData.trophyData.GetCurrentTimeComplete(type) == 0)
        {
            return "#ff9727";
        }
        switch (PlayerData.trophyData.GetCurrentCollectedTrophy(type))
        {
            case 1:
                return "#85cdff";
            case 2:
                return "#ffde00";
            case 3:
                return "#ff7ff1";
            default:
                return "#ff7ff1";
        }
    }
    public string GetTextInfoTrophyMerged(TrophyType type, int targetToTrophy, bool isMiniPopup = false)
    {
        targetToTrophy = targetToTrophy < 0 ? 0 : targetToTrophy;
        string textRS = "";
        string colorTrophy = GetColorTrophy(type);
        string trophy = $"<color={colorTrophy}>{GetTrophyTarget(type)}</color>";
        string targetReachColor = "#ffd400"; // Màu vàng nổi bật cho targetReach
        string targetReach = $"<color={targetReachColor}>{targetToTrophy}</color>";
        switch (type)
        {
            case TrophyType.GrandVoyage:
                textRS += $"Clear {targetReach} levels to get the {trophy}!";
                break;
            case TrophyType.SharkFrenzyForce:
                textRS += $"Collect {targetReach} characters to get the {trophy}!";
                break;
            case TrophyType.UnderseaVoyage:
                textRS += $"Complete the pass {targetReach} times to get the {trophy}!";
                break;
            case TrophyType.SharkRace:
            case TrophyType.StarRace:
                if (isMiniPopup)
                    textRS += $"Complete the event in <color={targetReachColor}>3rd place</color> or higher to get the {trophy}!";
                else
                    textRS += $"Complete the event in <color={targetReachColor}>3rd place</color> or higher {targetReach} times to get the {trophy}!";
                break;
            case TrophyType.ChompOff:
            case TrophyType.RelicHunt:
            case TrophyType.OceanGift:
                textRS += $"Complete the event {targetReach} times to get the {trophy}!";
                break;
        }
        return textRS;
    }

    public TrophyEventData GetStatusEventConfig(TrophyType type)
    {
        TrophyEventData dataEvent = new TrophyEventData();
        dataEvent.isEvent = false;
        dataEvent.isEventActive = true;
        switch (type)
        {
            case TrophyType.GrandVoyage:
                dataEvent.isEvent = false;
                break;
            case TrophyType.SharkFrenzyForce:
                dataEvent.isEvent = false;
                break;
            case TrophyType.UnderseaVoyage:
                dataEvent.isEvent = true;
                //Run another calculate 
                dataEvent.isEvent = true;
                bool active = PlayerData.journeyData.wasOpenJourney;
                dataEvent.isEventActive = PlayerData.journeyData.wasOpenJourney;
                if (!active)
                {
                    dataEvent.timeStartEvent = DateTime.Now.Add(PlayerData.journeyData.GetTimeRemaining());
                }
                break;
            case TrophyType.SharkRace:
                SetEventStatus(dataEvent, EventType.SharkRace);
                break;
            case TrophyType.ChompOff:
                SetEventStatus(dataEvent, EventType.ChompOff);
                break;
            case TrophyType.OceanGift:
                dataEvent.isEvent = true;
                dataEvent.isEventActive = true;
                break;
            case TrophyType.StarRace:
                SetEventStatus(dataEvent, EventType.StarChase);
                break;
            case TrophyType.RelicHunt:
                SetEventStatus(dataEvent, EventType.RelicHunt);
                break;
        }
        
        return dataEvent;
    }

    private void SetEventStatus(TrophyEventData dataEvent, EventType eventType)
    {
        dataEvent.isEvent = true;
        bool active = IsEventCanShow(eventType);
        dataEvent.isEventActive = active;
        if (!active)
        {
            dataEvent.timeStartEvent = Config.eventLiveOpsConfig.GetNextTimeStartEvent(eventType, PlayerData.GetDaysSinceDownload());
        }
    }
    public bool IsEventCanShow(EventType eventType)
    {
        int level = PlayerData.level;
        int levelUnlock = Config.AllGameEventConfig.GetGameEventConfig(eventType).levelUnlock;
        return (level + Config.AllGameEventConfig.diffLevelShow >= levelUnlock);
    }
    public string GetTrophyTarget(TrophyType type)
    {
        //    Bronze, Silver, Gold, Diamond
        if (PlayerData.trophyData.GetCurrentTimeComplete(type) == 0)
        {
            return "Bronze Trophy";
        }
        switch (PlayerData.trophyData.GetCurrentCollectedTrophy(type))
        {
            case 1:
                return "Silver Trophy";
            case 2:
                return "Gold Trophy";
            case 3:
                return "Diamond Trophy";
            default:
                return "Diamond Trophy";
        }
    }
}

public class TrophyEventData
{
    public bool isEvent = false;
    public bool isEventActive = false;
    public DateTime timeStartEvent;
}