using System.Collections;
using System.Collections.Generic;
using IEC;
using IEC.Common.Event;
using TMPro;
using UnityEngine;

public class UICurrency : _BaseBehaviour
{
    //Inherit this class if you want custom
    public ResourceType typeCurrency;
    public TextMeshProUGUI currencyTMP;
    private void Awake()
    {
        gameObject.OnEvent(EventList.GamePlay.UPDATE_RESOURCE_UI, UpdateCurrency);
    }
    
    public virtual void UpdateCurrency()
    {
        int value = 0;
        switch (typeCurrency)
        {
            case  ResourceType.Gold:
                value = PlayerData.gold;
                break;
            case  ResourceType.Currency2:
                break;
            case  ResourceType.Live:
                break;
        }
        currencyTMP.text = value.ToString();
    }
}
