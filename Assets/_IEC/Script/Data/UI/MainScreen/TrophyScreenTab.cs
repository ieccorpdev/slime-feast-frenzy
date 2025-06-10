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
    public override void Show()
    {
        LoadData();
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
        if (timesCollected == 0)
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
}