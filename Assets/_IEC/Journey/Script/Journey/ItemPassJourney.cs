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
        public List<GameEventRewardInfoUI> listRewardInfoUI = new();
        public List<GameEventRewardInfoUI> listRewardIMain = new();
        public GameObject chest;
        public GameObject rootIcon;
        public Button infor;
        public Button claim;
        public GameObject info;
        private static GameObject currentInfo;
        public List<GameObject> lockGroups, unlockGroups;

        private void Awake()
        {
            infor.onClick.AddListener(OnInfoItem);
            claim.onClick.AddListener(OnClaimItem);
        }

        public void OnInfoItem()
        {
            if (currentInfo)
            {
                currentInfo.transform.DOKill();
                currentInfo.SetActive(false);
            }
            currentInfo = info;
            info.transform.parent = transform.parent.parent.parent;
            info.transform.SetAsLastSibling();
            info.SetActive(true);
            info.transform.localScale = Vector3.zero;
            info.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

            DOVirtual.DelayedCall(3f, () =>
            {
                if (currentInfo == info)
                {
                    info.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
                    {
                        info.SetActive(false);
                    });
                }
            }, false);
        }

        public void OnClaimItem()
        {
            //Locator.Main.ClaimBonusRewardPanel.SetupRewardPanel(dataReward, null, true);
            //Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel(
            //  typePass == TypePassJourney.REGULAR ? ClaimBonusRewardSkin.Chest_UnderseaVoyageFree
            //  : ClaimBonusRewardSkin.Chest_UnderseaVoyageVip);
            //PlayerData.Vibrate(HapticTypes.Failure);
            Locator.Main.PopupJourney.OnCollectAllButtonClick();
            Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel(
              typePass == TypePassJourney.REGULAR ? ClaimBonusRewardSkin.Chest_UnderseaVoyageFree
              : ClaimBonusRewardSkin.Chest_UnderseaVoyageVip);
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

        public void BaseSetupDataItem(List<Reward> dataItemPass, int id)
        {
            ShowIconItem();
            currentID = id;
            foreach (var rewardUI in listRewardInfoUI)
            {
                Reward rewardData;
                if ((rewardData = dataItemPass.Find(data => data.typeReward == rewardUI.resourceType)) != null)
                {
                    rewardUI.gameObject.SetActive(true);
                    rewardUI.UpdateAmountText(rewardData.rewardAmount.ToString());
                }
                else
                {
                    rewardUI.gameObject.SetActive(false);
                }
            }
        }

        public void SetupDataItemPass(Rewards dataItemPass, int id)
        {
            dataReward = dataItemPass.listRewards.Select(x => x.reward).ToList();
            BaseSetupDataItem(dataReward, id);
            if (typePass == TypePassJourney.VIP)
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
            bool canInteracInfo = dataReward.Count > 2;
            infor.interactable = canInteracInfo;
            chest.SetActive(canInteracInfo);
            rootIcon.SetActive(!canInteracInfo);
            if (!canInteracInfo)
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
