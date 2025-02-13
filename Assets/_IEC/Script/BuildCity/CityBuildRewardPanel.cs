using System.Collections;
using System.Collections.Generic;
using IEC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityBuildRewardPanel : _BaseBehaviour
{
    public Button tapToClaim;

    public Image rewardImage;
    public TextMeshProUGUI rewardAmountTMP;
    private BuildCityDataConfig _dataReward;
    void Start()
    {
        tapToClaim.onClick.AddListener(() =>
        {
            //Anim GetReward
            this.gameObject.SetActive(false);
            Locator.Main.HomeScreenTab.buildCityUI.OnClaimReward();
        });
    }

    public void ShowingRewardPanel()
    {
        GetReward();
        this.gameObject.SetActive(true);
    }
    public void SetupRewardPanel(BuildCityDataConfig data)
    {
        //SetupRewardHere
        //Add code to multiple reward here ( health, helper etc ) 
        //rewardImage = coinImage
        _dataReward = data;
        int totalGold = 0;
        foreach (var rw in _dataReward.listReward)
        {
            switch (rw.typeCityReward)
            {
                case BonusDataCityReward.Gold:
                    totalGold += rw.rewardAmount;
                    break;
            }
        }
        rewardAmountTMP.text =totalGold.ToString();
    }

    public void GetReward()
    {
        //Get Reward instance 
        foreach (var rw in _dataReward.listReward)
        {
            switch (rw.typeCityReward)
            {
                case BonusDataCityReward.Gold:
                    PlayerData.gold += rw.rewardAmount;
                    break;
            }
        }
    }
}
