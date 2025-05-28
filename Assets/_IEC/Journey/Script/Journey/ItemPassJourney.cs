using MoreMountains.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ItemPassJourney : _BaseBehaviour
    {
        [Header("Data")]
        public TypePassJourney typePass;
        public Rewards dataReward;
        public JourneyItemStatus statusItem;
        public int currentID;
        
        public void OnClaimItem()
        {
            Locator.Main.ClaimBonusRewardPanel.SetupRewardPanel(dataReward.listRewards.Select(x => x.reward).ToList(), null, true);
            Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel(ClaimBonusRewardSkin.Chest_UnderseaVoyageFree);
            PlayerData.Vibrate(HapticTypes.Failure);
        }

        public void MakeItemClaimed()
        {
            switch (typePass)
            {
                case TypePassJourney.REGULAR:
                    PlayerData.journeyData.SetStatusRegularPass(currentID, JourneyItemStatus.CLAIMED);
                    break;
                case TypePassJourney.VIP:
                    PlayerData.journeyData.SetStatusVipPass(currentID, JourneyItemStatus.CLAIMED);
                    break;
            }
        }

        public void BaseSetupDataItem(RewardByJourney data, Rewards dataItemPass, int id)
        {
            ShowIconItem(data, dataItemPass);
            currentID = id;
        }

        public void SetupDataItemPass(RewardByJourney data, Rewards dataItemPass, int id)
        {
            BaseSetupDataItem(data, dataItemPass, id);
            statusItem = PlayerData.journeyData.GetStatusRegularPass(currentID);
            switch (statusItem)
            {
                case JourneyItemStatus.LOCK:
                    break;
                case JourneyItemStatus.UNLOCK:
                    break;
                case JourneyItemStatus.CLAIMED:
                    break;
                case JourneyItemStatus.CANCLAIM:
                    break;
            }
        }
        
        public void ShowIconItem(RewardByJourney data, Rewards dataItemPass)
        {
            dataReward = dataItemPass;
        }
    }
    
    public enum TypePassJourney
    {
        REGULAR,
        VIP
    }
}
