using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using IEC;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHomeTab : ScreenNavigateUI
{
    public Button characterListButton;
    public CharacterSelectionTab characterSelectionTab;
    public CharacterShardTab characterShardTab;

    private void OnEnable()
    {
        this.RegisterListener(EventList.GamePlay.CHANGE_TAB_CHARACTER_SHOP, OnChangeTabEvent);
    }
    
    private void OnDisable()
    {
        this.UnregisterListener(EventList.GamePlay.CHANGE_TAB_CHARACTER_SHOP, OnChangeTabEvent);
    }
    
    private void OnChangeTabEvent(object param)
    {
        var type = (CharacterTabType)param;
            
        characterSelectionTab.gameObject.SetActive(type == CharacterTabType.Character);
        characterShardTab.gameObject.SetActive(type == CharacterTabType.Shard);

        // if (characterSelectionTab.gameObject.activeInHierarchy)
        // {
        //     characterSelectionTab.Setup();
        // }
        // else if (characterShardTab.gameObject.activeInHierarchy)
        // {
        //     characterShardTab.Setup();
        // }
    }

    public override void Show()
    {
        this.RaiseEvent(EventList.GamePlay.CHANGE_TAB_CHARACTER_SHOP, CharacterTabType.Character);
        
        characterSelectionTab.Setup();
        
        if (PlayerData.CheckRefreshCharacterShardShop())
        {
            characterShardTab.Setup();
        }
        else
        {
            characterShardTab.LoadData();
        }

        base.Show();
    }
}
