using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using IEC.Common.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICurrency : _BaseBehaviour
{
    //Inherit this class if you want custom
    public ResourceType typeCurrency;
    public TextMeshProUGUI currencyTMP;
    public Button addButton;
    public Image targetIcon;
    public GameObject addButtonDisplay;
    public Animator clickEffectAnimator;
    private bool m_isForcedCurrencyText = false;

    private void Awake()
    {
        gameObject.OnEvent(EventList.GamePlay.UPDATE_RESOURCE_UI, UpdateCurrency);
        gameObject.OnEvent<EventParam>(EventList.GamePlay.UPDATE_RESOURCE_UI, param =>
        {
            if ((ResourceType?)param.p0 == ResourceType.Gold && param.p1 is int amount && amount > 0)
            {
                UpdateCustomCurrency(amount);
            }
            else
            {
                UpdateCurrency();
            }
        });
        if (addButton) addButton.onClick.AddListener(OnAddButtonClick);
    }

    public void TriggredPressButton()
    {
       if(clickEffectAnimator !=null) clickEffectAnimator.Play("PunchScale");
    }
    public void OnEnable()
    {
        UpdateCurrency();
    }

    private void OnAddButtonClick()
    {
        switch (typeCurrency)
        {
            case ResourceType.Gold:
                if (Locator.Main.PopupGoldOffer.IsShow)
                    return;
                Locator.Main.PopupGoldOffer.Show();
                break;
            case ResourceType.Live:
                if (Locator.Main.PopupOutOfLive.IsShow)
                    return;
                if (!PlayerData.IsInUnlimitedLife() && PlayerData.playerLife < Config.maxHealth)
                {
                    Locator.Main.PopupOutOfLive.Show();
                }
                break;
        }
    }
    
    
    public virtual void UpdateCurrency()
    {
        if (m_isForcedCurrencyText)
            return;
        
        int value = 0;
        switch (typeCurrency)
        {
            case  ResourceType.Gold:
                value = PlayerData.GetPlayerCurrencyCount(ResourceType.Gold);
                break;
            case  ResourceType.Currency2:
                break;
            case  ResourceType.Live:
                break;
        }
        currencyTMP.text = value.ToString();
    }
    public virtual void UpdateCustomCurrency(int cv)
    {
        if (m_isForcedCurrencyText)
            return;
        
        int value = 0;
        switch (typeCurrency)
        {
            case  ResourceType.Gold:
                value = cv;
                break;
            case  ResourceType.Currency2:
                break;
            case  ResourceType.Live:
                break;
        }
        currencyTMP.text = value.ToString();
    }
    public void SetIsForcedCurrencyDisplay(bool isForced)
    {
        m_isForcedCurrencyText = isForced;
    }

    //For increasing currency effect
    public void ForceUpdateCurrencyText(int amount)
    {
        currencyTMP.text = amount.ToString();
    }

    public int GetCurrentCurrencyTextAmount()
    {
        return int.Parse(currencyTMP.text);
    }
}
