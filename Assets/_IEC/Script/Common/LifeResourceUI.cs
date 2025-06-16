using System.Collections;
using System.Collections.Generic;
using IEC;
using IEC.Common.Event;
using TMPro;
using UnityEngine;

public class LifeResourceUI : ResourceUI
{
    [SerializeField] private TMP_Text m_LifeCount;
    [SerializeField] private GameObject m_Unlimited;

    protected override void Init()
    {
        type = ResourceType.Live;
        onClick = () =>
        {
            //TODO: Show popup 
        };
        gameObject.OnEvent<EventParam>(EventList.GamePlay.UPDATE_RESOURCE_UI, param =>
        {
            if ((ResourceType)param.p0 == ResourceType.Live||((ResourceType)param.p0 == ResourceType.UnlimitedLive))
            {
                UpdateUI();
            }
        });

        UpdateUI();
        base.Init();
    }

    protected override void UpdateUI()
    {
        var timeUnlimited = PlayerData.hybridData.GetResourceCount(ResourceType.UnlimitedLive);
        if (timeUnlimited > 0)
        {
            m_Unlimited.SetActive(true);
            m_LifeCount.gameObject.SetActive(false);
            SetText(CurrencyFormat.FormatTime(timeUnlimited));
        }
        else
        {
            var lifeCount = PlayerData.hybridData.GetResourceCount(ResourceType.Live);
            m_Unlimited.SetActive(false);
            m_LifeCount.text = lifeCount.ToString();
            m_LifeCount.gameObject.SetActive(true);
            SetText(lifeCount == 5 ? "Full" : CurrencyFormat.FormatTime(PlayerData.hybridData.timeRefillLeft));
        }

        base.UpdateUI();
    }
}