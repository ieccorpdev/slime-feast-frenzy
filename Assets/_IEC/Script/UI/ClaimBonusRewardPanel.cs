using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using IEC;
using MoreMountains.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;

public class ClaimBonusRewardPanel : BasePopup
{
    public List<UIItemReward> listRewardUI;
    public SpineAnimOT chestAnim;
    public Button tapToClaim;
    public GameObject textTapToClaim;
    public GameObject Aura;

    public List<Reward> listDataReward;
    public Action actionClose;
    private bool canClaim;
    private bool canOpen;
    private bool showAimChest = true;

    [Header("Fixed position")] public PositionGroupsRewardDataPanel groupsOne;
    public PositionGroupsRewardDataPanel groupsTwo;
    public PositionGroupsRewardDataPanel groupsThree;
    public PositionGroupsRewardDataPanel groupsFour;
    public PositionGroupsRewardDataPanel groupsFive;
    public PositionGroupsRewardDataPanel groupsSix;
    public PositionGroupsRewardDataPanel groupsSeven;
    public PositionGroupsRewardDataPanel groupsEight;
    public PositionGroupsRewardDataPanel groupsNine;

    private Queue<ShowClaimRewardData> queueShowClaimReward = new();

    public string AutoClaimTween => GetInstanceID() + "_AutoClaim";
    // Start is called before the first frame update
    void Start()
    {
        tapToClaim.onClick.AddListener(OnClaimReward);
    }

    public bool IsHaveShowClaimRewardAnim()
    {
        return queueShowClaimReward.Count > 0;
    }

    public void AddClaimRewardData(ShowClaimRewardData data)
    {
        data.actionClose += ShowNext;
        queueShowClaimReward.Enqueue(data);
    }

    public void ShowNext()
    {
        if (queueShowClaimReward.Count == 0)
            return;

        var data = queueShowClaimReward.Dequeue();
        
        SetupRewardPanel(data.listDataReward, data.actionClose, data.showAimChest);
        ShowingRewardPanel(data.chestType);
    }

    public void SetupRewardPanel(List<Reward> listDataReward, Action actionClose, bool showAimChest = false)
    {
        canOpen = false;
        canClaim = false;
        this.listDataReward = listDataReward;
        this.showAimChest = showAimChest;
        this.actionClose = null;
        this.actionClose = actionClose;
    }

    public void SettingRewardPosition()
    {
        int numberReward = listDataReward.Count;
        switch (numberReward)
        {
            case 1:
                AssignRewardByGroups(groupsOne);
                break;
            case 2:
                AssignRewardByGroups(groupsTwo);
                break;
            case 3:
                AssignRewardByGroups(groupsThree);
                break;
            case 4:
                AssignRewardByGroups(groupsFour);    
                break;
            case 5:
                AssignRewardByGroups(groupsFive);
                break;         
            case 6:
                AssignRewardByGroups(groupsSix);
                break;
            case 7:
                AssignRewardByGroups(groupsSeven);
                break;
            case 8:
                AssignRewardByGroups(groupsEight);
                break;
            case 9:
                AssignRewardByGroups(groupsNine);
                break;
        }
    }

    public void AssignRewardByGroups(PositionGroupsRewardDataPanel groups)
    {
        int index = 0;
        for (int i = 0; i < listRewardUI.Count; i++)
        {
            if (listRewardUI[i].haveData)
            {
                index++;
                var anchoTarget = groups.GetRectTransformByIndex(index).anchoredPosition;
                anchoTarget += Vector2.up * ( showAimChest ? 225: 0);
                listRewardUI[i].targetAnchorPos = anchoTarget;
            }
        }
    }
    
    public void ShowingRewardPanel(string skinChest = "")
    {
        DOTween.Kill(AutoClaimTween);
        Locator.Service.GameResourceDisplay.ToggleDisplayAllResources(false);
        EventController.RaiseEvent(EventList.Audio.GET_REWARD);
        Pool.Get<ConfettiGroup>().ShowConfetti();
        gameObject.SetActive(true);
        //Showing
        foreach (var itm in listRewardUI)
        {
            itm.haveData = false;
            itm.gameObject.SetActive(false);
        }
        foreach (var itr in listDataReward)
        {
            var itemReward = listRewardUI.Find(x => x.typeReward == itr.typeReward);
            if (itemReward != null)
            {
                itemReward.LoadValueReward(itr.typeReward,itr.rewardAmount);
                itemReward.haveData = true;
            }
        }

        SettingRewardPosition();
        if (Aura) Aura.gameObject.SetActive(false);
        float timeDelayEffect = showAimChest ? 2f : 0f;
        if (showAimChest)
        {
            tapToClaim.gameObject.SetActive(true);
            canOpen = true;
            chestAnim.gameObject.SetActive(true);
            chestAnim.SetSkin(skinChest);
            Pool.Get<ConfettiGroup>().StopConfetti();
            chestAnim.SetAnimation("1_Start", false, () =>
            {
                chestAnim.SetAnimation("2_Idle", false, () =>
                {
                    OpenChest();
                    AnimShowTextTapClaim();
                });
            });
            //soundController.PlaySound(new AudioInfo(Config.soundConfig.unlockNewFeatureGP, false));
            TweenControl.GetInstance().DelayCall(tapToClaim.gameObject.transform, 1.5f, () =>
            {
                //soundController.PlaySound(new AudioInfo(Config.soundConfig.openChest, false));
            });
        }
        else
        {
            chestAnim.gameObject.SetActive(false);
            canClaim = true;
            canOpen = true;
        }

        if (!showAimChest)
        {
            //Block 
            TweenControl.GetInstance().DelayCall(tapToClaim.gameObject.transform, timeDelayEffect, ShowRewardInChest);
        }

    }

    private void ShowRewardInChest()
    {
        int inx = 0;
        foreach (var itm in listRewardUI)
        {
            if (itm.haveData)
            {
                TweenControl.GetInstance().DelayCall(itm.transform, inx * 0.07f,
                    () =>
                    {
                        itm.GetComponent<RectTransform>().transform.position =
                            chestAnim.GetComponent<RectTransform>().transform.position;
                    }, () =>
                    {
                        TweenControl.GetInstance().ScaleFromZero(itm.gameObject, 0.3f,
                            () => { itm.transform.DOPunchScale(Vector3.one * 0.2f, 0.4f, 1, 0.3f); });

                        TweenControl.GetInstance().MoveRect(itm.GetComponent<RectTransform>(), itm.targetAnchorPos, 0.5f, null,
                            EaseType.OutQuint);
                    });
                inx++;
            }
        }

        if (Aura)
        {
            Aura.gameObject.SetActive(true);
            TweenControl.GetInstance().ScaleFromZero(Aura.gameObject, 0.5f);
            if (!showAimChest)
            {
                AnimShowTextTapClaim();
            }
        }
    }

    private void AnimShowTextTapClaim()
    {
        TweenControl.GetInstance().DelayCall(tapToClaim.gameObject.transform, 0.5f, () =>
        {
            tapToClaim.gameObject.SetActive(true);
            TweenControl.GetInstance().ScaleFromZero(textTapToClaim.gameObject, 0.5f);
        });
    }

    public void OnClaimReward()
    {
        if (canOpen && showAimChest)
        {
            OpenChest(true);
        } else if (canClaim)
        {
            ClaimReward();
        }
    }

    private void ClaimReward()
    {
        DOTween.Kill(AutoClaimTween);
        tapToClaim.gameObject.SetActive(false);
        canClaim = false;
        canOpen = false;
        foreach (var itr in listDataReward)
        {
            PlayerData.AddCurrency(itr.typeReward, itr.rewardAmount);
        }

        if (!IsHaveShowClaimRewardAnim())
        {
            //this.gameObject.SetActive(false);
            Hide();
        }

        actionClose?.Invoke();
        PlayerData.Vibrate(HapticTypes.Failure);
        Pool.Get<ConfettiGroup>().StopConfetti();
    }

    private void OpenChest(bool fastOpen = false)
    {
        canClaim = true;
        canOpen = false;
        if (fastOpen)
        {
            EndingChestOpen();
        }
        else
        {
            chestAnim.SetAnimation("3_Open", false, () =>
            {
                EndingChestOpen();
            });
        }

    }

    private void EndingChestOpen()
    {
        ShowRewardInChest();
        chestAnim.SetAnimation("4_OpenIdle", false, () =>
        {
            //Autoclaim
            DOVirtual.DelayedCall(1.5f, () =>
            {
                if (canClaim)
                {
                    ClaimReward();
                }
            }).SetId(AutoClaimTween);
        });
    }

    public override void Hide()
    {
        Locator.Service.GameResourceDisplay.ToggleDisplayAllResources(true);
        base.Hide();
        CallTempOnHide();
    }
}

[Serializable]public class PositionGroupsRewardDataPanel
{
    public RectTransform Pos1;
    public RectTransform Pos2;
    public RectTransform Pos3;
    public RectTransform Pos4;
    public RectTransform Pos5;
    public RectTransform Pos6;
    public RectTransform Pos7;
    public RectTransform Pos8;
    public RectTransform Pos9;

    public RectTransform GetRectTransformByIndex(int index)
    {
        switch (index)
        {
            case 1:
                return Pos1;
            case 2:
                return Pos2;
            case 3:
                return Pos3;
            case 4:
                return Pos4;
            case 5:
                return Pos5;
            case 6:
                return Pos6;
            case 7:
                return Pos7;
            case 8:
                return Pos8;
            case 9:
                return Pos9;
        }
        return Pos1;
    }
}

[Serializable]
public class ShowClaimRewardData
{
    public List<Reward> listDataReward = new();
    public Action actionClose;
    public bool showAimChest;
    public string chestType = "";
}