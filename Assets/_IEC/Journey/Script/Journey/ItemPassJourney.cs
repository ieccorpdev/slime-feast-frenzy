using DG.Tweening;
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
        public List<Reward> dataReward;
        public JourneyItemStatus statusItem;
        public int currentID;
        [Header("Reference")]
        public List<GameEventRewardInfoUI> listRewardIMain = new();
        public GameObject chest;
        public GameObject rootIcon;
        public Button infor;
        public Button claim;
        public List<GameObject> lockGroups, unlockGroups;
        private bool isUseChest;
        public RewardInfoGroup rewardInfoGroup;
        public static GameObject currentInfo;

        private void Awake()
        {
            infor.onClick.AddListener(OnInfoItem);
            claim.onClick.AddListener(OnClaimItem);
        }

        public void OnInfoItem()
        {
            rewardInfoGroup.Show();
        }

        IEnumerator WaitEndTut()
        {
            yield return new WaitUntil(() => PlayerData.journeyData.isShowTutorialInPopup);
            HandleHandleTut();
        }

        public void OnClaimItem()
        {
            if (!PlayerData.journeyData.isShowHandtut && typePass == TypePassJourney.REGULAR && currentID == 0)
            {
                PlayerData.journeyData.isShowHandtut = true;
                Locator.Main.HighLightCanvas.RemoveCurrentHighLight();
                Locator.Main.PopupJourney.bgdark.SetActive(false);
            }
            if (currentID == Config.journeyTripConfig.listRewardsByJourneys[0].idJourney && typePass == TypePassJourney.VIP)
            {
                PlayerData.journeyData.isEnableMaxHeart = true;
                PlayerData.RaiseToMaxHealth();
            }
            if (currentID == Config.journeyTripConfig.bonusReward.idJourney)
            {
                Locator.Main.PopupJourney.OnCollectAllButtonClick(dataReward, this);
                Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel(ClaimBonusRewardSkin.Chest_UnderseaVoyageVip);
            }
            else
            {
                Locator.Main.PopupJourney.OnCollectAllButtonClick();
                Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel(
                  PlayerData.journeyData.isBuyIAP ?
                  ClaimBonusRewardSkin.Chest_UnderseaVoyageFree
                  : ClaimBonusRewardSkin.Chest_2);
            }
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

        private void OnEnable()
        {
            StartCoroutine(WaitEndTut());
        }

        private void HandleHandleTut()
        {
            if (!PlayerData.journeyData.isShowHandtut && typePass == TypePassJourney.REGULAR && currentID == 0)
            {
                Locator.Main.HighLightCanvas.HighLightUI(claim.GetComponent<RectTransform>(), true, true);
                Locator.Main.PopupJourney.bgdark.SetActive(true);
            }
        }

        public void BaseSetupDataItem(List<Reward> dataItemPass, int id)
        {
            ShowIconItem();
            currentID = id;
            rewardInfoGroup.SetupDataItem(dataItemPass);
        }

        public void SetupDataItemPass(Rewards dataItemPass, int id)
        {
            dataReward = dataItemPass.listRewards.Select(x => x.reward).ToList();
            BaseSetupDataItem(dataReward, id);
            if (id == Config.journeyTripConfig.bonusReward.idJourney)
            {
                if (PlayerData.journeyData.GetStatusVipPass(currentID) == JourneyItemStatus.CLAIMED)
                {
                    statusItem = JourneyItemStatus.CLAIMED;
                }
                else if (PlayerData.journeyData.currentTargetJourney == Config.journeyTripConfig.listRewardsByJourneys.Count + 1 && PlayerData.journeyData.isBuyIAP)
                {
                    statusItem = JourneyItemStatus.CANCLAIM;
                    PlayerData.journeyData.SetStatusVipPass(id, JourneyItemStatus.CANCLAIM);
                }
                else
                {
                    statusItem = JourneyItemStatus.LOCK;
                }
            }
            else if (typePass == TypePassJourney.VIP)
            {
                statusItem = PlayerData.journeyData.GetStatusVipPass(currentID);
            }
            else
            {
                statusItem = PlayerData.journeyData.GetStatusRegularPass(currentID);
            }
            claim.gameObject.SetActive(false);
            switch (statusItem)
            {
                case JourneyItemStatus.LOCK:
                    foreach (var go in lockGroups)
                    {
                        go.SetActive(true);
                    }
                    foreach (var go in unlockGroups)
                    {
                        go.SetActive(false);
                    }
                    break;
                case JourneyItemStatus.UNLOCK:
                    foreach (var go in lockGroups)
                    {
                        go.SetActive(false);
                    }
                    foreach (var go in unlockGroups)
                    {
                        go.SetActive(false);
                    }
                    break;
                case JourneyItemStatus.CLAIMED:

                    foreach (var go in lockGroups)
                    {
                        go.SetActive(false);
                    }
                    foreach (var go in unlockGroups)
                    {
                        go.SetActive(true);
                    }
                    break;
                case JourneyItemStatus.CANCLAIM:
                    claim.gameObject.SetActive(true);
                    foreach (var go in lockGroups)
                    {
                        go.SetActive(false);
                    }
                    foreach (var go in unlockGroups)
                    {
                        go.SetActive(false);
                    }
                    break;
            }
        }

        public void ShowIconItem()
        {
            isUseChest = dataReward.Count > 2;
            chest.SetActive(isUseChest);
            rootIcon.SetActive(!isUseChest);
            if (!isUseChest)
            {
                foreach (var rewardUI in listRewardIMain)
                {
                    Reward rewardData;
                    if ((rewardData = dataReward.Find(data => data.typeReward == rewardUI.resourceType)) != null)
                    {
                        rewardUI.gameObject.SetActive(true);
                        string result = rewardData.rewardAmount.ToString();
                        switch (rewardUI.resourceType)
                        {
                            case ResourceType.UnlimitedLive:
                            case ResourceType.TimeUnlimitedWindSurf:
                            case ResourceType.TimeUnlimitedEvolveStone:
                                result = "<sprite name=\"icon_timer\"> " + result;
                                break;

                        }
                        rewardUI.UpdateAmountText(result);
                    }
                    else
                    {
                        rewardUI.gameObject.SetActive(false);
                    }
                }
                rootIcon.transform.localScale = Vector3.one * (dataReward.Count == 1 ? 1.3f : 1f);
            }
        }
    }

    public enum TypePassJourney
    {
        REGULAR,
        VIP
    }
}
