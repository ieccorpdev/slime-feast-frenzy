using MoreMountains.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public enum JourneyItemStatus
    {
        LOCK,
        UNLOCK,
        CANCLAIM,
        CLAIMED
    }

    public class ItemPassJourney : _BaseBehaviour
    {
        private int currentID;
        public Image lockIcon;
        public Image chestIcon;
        public Image goldReward1;
        public Image goldReward2;
        public Image goldReward3;
        public Image goldReward4;
        public Image addHoleReward;
        public Image addHammerReward;
        public Image addToolsBoxReward;
        public Image unlimidtedLifeReward;
        public Image doneTickClaim;
        public TextMeshProUGUI valueReward;
        public Button infoItemBtn;
        public Button collectBtn;

        [Header("RewardDetailVipPass")]
        public GameObject detailReward;
        public GameObject detailGoldObj;
        public TextMeshProUGUI detailGoldValue;
        public GameObject detailHammerObj;
        public TextMeshProUGUI detailHammerValue;
        public GameObject detailAddSlotObj;
        public TextMeshProUGUI detailAddSlotValue;
        public GameObject detailToolsBoxObj;
        public TextMeshProUGUI detailToolsBoxValue;
        public GameObject detailInfinityLifeObj;
        public TextMeshProUGUI detailInfinityLifeValue;
        public Button selectedVipPass;
        [Header("Data")]
        public TypeItemPassJourney typeReward;
        public TypePassJourney typePass;
        public Rewards dataReward;
        public JourneyItemStatus statusItem;
        private void Start()
        {
            if (infoItemBtn) infoItemBtn.onClick.AddListener(OnClickShowInfo);
            collectBtn.onClick.AddListener(OnClaimItem);
            if (selectedVipPass) selectedVipPass.onClick.AddListener(OnClickVipPassAnim);
        }

        public void OnClickVipPassAnim()
        {
            //if (!PlayerData.purchasedJourneyPass)
            //{
            //    UI.popupJourney.PlayChangeCarSelectVipPass();
            //    PlayerData.Vibrate(HapticTypes.Failure);
            //}
        }
        public void OnClaimItem()
        {
            //switch (typeReward)
            //{
            //    case TypeItemPassJourney.NORMAL:
            //        foreach (var itr in dataReward.listReward)
            //        {
            //            switch (itr.typeReward)
            //            {
            //                case BonusDataReward.Gold:
            //                    PlayerData.AddGold(itr.rewardAmount);
            //                    break;
            //                case BonusDataReward.UnlimitedLives:
            //                    PlayerData.AddTimeUnlimitedLife(itr.rewardAmount);
            //                    break;
            //                case BonusDataReward.HAMMER:
            //                    PlayerData.numHammer += itr.rewardAmount;
            //                    break;
            //                case BonusDataReward.EXTRAHOLE:
            //                    PlayerData.numAddSlot += itr.rewardAmount;
            //                    break;
            //                case BonusDataReward.DOUBLETOOLSBOX:
            //                    PlayerData.numToolsBox += itr.rewardAmount;
            //                    break;
            //            }
            //        }
            //        EndClaim();
            //        break;
            //    case TypeItemPassJourney.CHEST:
            //        UI.popupJourney.ClaimBonusRewardPanel.gameObject.SetActive(true);
            //        UI.popupJourney.ClaimBonusRewardPanel.SetupRewardPanel(dataReward.listReward, EndClaim);
            //        UI.popupJourney.ClaimBonusRewardPanel.ShowingRewardPanel();
            //        break;
            //}
            PlayerData.Vibrate(HapticTypes.Failure);
        }

        public void MakeItemClaimed()
        {
            switch (typePass)
            {
                case TypePassJourney.REGULAR:
                    PlayerData.SetStatusRegularPass(currentID, JourneyItemStatus.CLAIMED);
                    break;
                case TypePassJourney.VIP:
                    PlayerData.SetStatusVipPass(currentID, JourneyItemStatus.CLAIMED);
                    break;
            }
        }
        void EndClaim()
        {
            MakeItemClaimed();
            //UI.groupGold.UpdateUI();
            //UI.screenHome.groupGold.UpdateUI();
            //UI.popupJourney.RefreshUI();
        }
        void OnClickShowInfo()
        {
            detailReward.gameObject.SetActive(!detailReward.gameObject.activeInHierarchy);
            PlayerData.Vibrate(HapticTypes.Failure);
        }
        private void ResetAllData()
        {
            lockIcon.gameObject.SetActive(false);
            goldReward1.gameObject.SetActive(false);
            goldReward2.gameObject.SetActive(false);
            goldReward3.gameObject.SetActive(false);
            goldReward4.gameObject.SetActive(false);
            addHoleReward.gameObject.SetActive(false);
            addHammerReward.gameObject.SetActive(false);
            addToolsBoxReward.gameObject.SetActive(false);
            unlimidtedLifeReward.gameObject.SetActive(false);
            doneTickClaim.gameObject.SetActive(false);
            collectBtn.gameObject.SetActive(false);
            valueReward.gameObject.SetActive(false);
            if (chestIcon) chestIcon.gameObject.SetActive(false);
            if (detailReward) detailReward.gameObject.SetActive(false);
            if (infoItemBtn) infoItemBtn.gameObject.SetActive(false);
        }

        public void BaseSetupDataItem(RewardByJourney data, Rewards dataItemPass, int id)
        {
            ResetAllData();
            ShowIconItem(data, dataItemPass);
            currentID = id;
        }
        public void SetupDataItemPass(RewardByJourney data, Rewards dataItemPass, int id)
        {
            BaseSetupDataItem(data, dataItemPass, id);
            statusItem = PlayerData.GetStatusRegularPass(currentID);
            typePass = TypePassJourney.REGULAR;
            switch (statusItem)
            {
                case JourneyItemStatus.LOCK:
                    lockIcon.gameObject.SetActive(true);
                    //Show nothing
                    break;
                case JourneyItemStatus.UNLOCK:
                    //may be unlock but can't claim, for vip pass
                    collectBtn.gameObject.SetActive(true);
                    break;
                case JourneyItemStatus.CLAIMED:
                    doneTickClaim.gameObject.SetActive(true);
                    break;
                case JourneyItemStatus.CANCLAIM:
                    collectBtn.gameObject.SetActive(true);
                    break;
            }
        }
        public void SetupDataItemVipPass(RewardByJourney data, Rewards dataItemPass, int id)
        {
            BaseSetupDataItem(data, dataItemPass, id);
            statusItem = PlayerData.GetStatusVipPass(currentID);
            typePass = TypePassJourney.VIP;
            switch (statusItem)
            {
                case JourneyItemStatus.LOCK:
                    //Show nothing
                    lockIcon.gameObject.SetActive(true);
                    break;
                case JourneyItemStatus.UNLOCK:
                    //may be unlock but can't claim, for vip pass
                    break;
                case JourneyItemStatus.CLAIMED:
                    doneTickClaim.gameObject.SetActive(true);
                    break;
                case JourneyItemStatus.CANCLAIM:
                    collectBtn.gameObject.SetActive(true);
                    break;
            }
        }
        public void ShowIconItem(RewardByJourney data, Rewards dataItemPass)
        {
            dataReward = dataItemPass;
            if (dataItemPass.listRewards.Count > 1)
            {
                //Chest
                if (chestIcon)
                {
                    typeReward = TypeItemPassJourney.CHEST;
                    chestIcon.gameObject.SetActive(true);
                    SetupInfoButton(dataItemPass);
                    //chestIcon.sprite = Config.JourneyTripConfig.lstSprChest[data.chestModel - 1];
                }

            }
            else
            {
                var type = dataItemPass.listRewards[0];
                valueReward.gameObject.SetActive(true);
                valueReward.text = type.typeReward == ResourceType.UnlimitedLive ? (type.rewardAmount + "m") : ("x" + type.rewardAmount);
                typeReward = TypeItemPassJourney.NORMAL;
                switch (type.typeReward)
                {
                    case ResourceType.Gold:

                        if (type.rewardAmount > 80)
                        {
                            goldReward4.gameObject.SetActive(true);
                            break;
                        }
                        if (type.rewardAmount > 40)
                        {
                            goldReward3.gameObject.SetActive(true);
                            break;
                        }
                        if (type.rewardAmount > 20)
                        {
                            goldReward2.gameObject.SetActive(true);
                            break;
                        }
                        goldReward1.gameObject.SetActive(true);
                        break;
                    case ResourceType.UnlimitedLive:
                        unlimidtedLifeReward.gameObject.SetActive(true);
                        break;
                    //case ResourceType.HAMMER:
                    //    addHammerReward.gameObject.SetActive(true);
                    //    break;
                    //case ResourceType.EXTRAHOLE:
                    //    addHoleReward.gameObject.SetActive(true);
                    //    break;
                    //case ResourceType.DOUBLETOOLSBOX:
                    //    addToolsBoxReward.gameObject.SetActive(true);
                        break;
                }
            }
        }
        public void SetupInfoButton(Rewards dataItemPass)
        {
            if (infoItemBtn) infoItemBtn.gameObject.SetActive(true);
            detailGoldObj.gameObject.SetActive(false);
            detailHammerObj.gameObject.SetActive(false);
            detailAddSlotObj.gameObject.SetActive(false);
            detailToolsBoxObj.gameObject.SetActive(false);
            detailInfinityLifeObj.gameObject.SetActive(false);
            foreach (var type in dataItemPass.listRewards)
            {
                switch (type.typeReward)
                {
                    case ResourceType.Gold:
                        detailGoldObj.gameObject.SetActive(true);
                        detailGoldValue.text = "x" + type.rewardAmount;
                        break;
                    case ResourceType.UnlimitedLive:
                        detailInfinityLifeObj.gameObject.SetActive(true);
                        detailInfinityLifeValue.text = type.rewardAmount + "m";
                        break;
                    //case ResourceType.HAMMER:
                    //    detailHammerObj.gameObject.SetActive(true);
                    //    detailHammerValue.text = "x" + type.rewardAmount;
                    //    break;
                    //case ResourceType.EXTRAHOLE:
                    //    detailAddSlotObj.gameObject.SetActive(true);
                    //    detailAddSlotValue.text = "x" + type.rewardAmount;
                    //    break;
                    //case ResourceType.DOUBLETOOLSBOX:
                    //    detailToolsBoxObj.gameObject.SetActive(true);
                    //    detailToolsBoxValue.text = "x" + type.rewardAmount;
                        break;
                }
            }
        }
    }
    public enum TypeItemPassJourney
    {
        NORMAL,
        CHEST
    }
    public enum TypePassJourney
    {
        REGULAR,
        VIP
    }
}
