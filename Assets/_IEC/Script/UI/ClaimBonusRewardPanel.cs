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
    private bool showAimChest = true;

    [Header("Fixed position")] 
    public PositionGroupsRewardDataPanel groupsOne;
    public PositionGroupsRewardDataPanel groupsTwo;
    public PositionGroupsRewardDataPanel groupsThree;
    public PositionGroupsRewardDataPanel groupsFour;
    public PositionGroupsRewardDataPanel groupsFive;
    public PositionGroupsRewardDataPanel groupsSix;
    public PositionGroupsRewardDataPanel groupsSeven;
    // Start is called before the first frame update
    void Start()
    {
        tapToClaim.onClick.AddListener(OnClaimReward);
    }

    public void SetupRewardPanel(List<Reward> listDataReward, Action actionClose, bool showAimChest = false)
    {
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
                listRewardUI[i].targetAnchorPos = groups.GetRectTransformByIndex(index).transform.position;
            }
        }
    }
    
    public void ShowingRewardPanel(string skinChest = "")
    {
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
        tapToClaim.gameObject.SetActive(false);
        if (Aura) Aura.gameObject.SetActive(false);

        float timeDelayEffect = showAimChest ? 2f : 0f;
        if (showAimChest)
        {
            chestAnim.SetSkin(skinChest);
            chestAnim.SetAnimation("animation", false);
            //soundController.PlaySound(new AudioInfo(Config.soundConfig.unlockNewFeatureGP, false));
            TweenControl.GetInstance().DelayCall(tapToClaim.gameObject.transform, 1.5f, () =>
            {
                //soundController.PlaySound(new AudioInfo(Config.soundConfig.openChest, false));
            });
        }
        else
        {
            chestAnim.gameObject.SetActive(false);
        }

        TweenControl.GetInstance().DelayCall(tapToClaim.gameObject.transform, timeDelayEffect, () =>
        {
            int inx = 0;
            foreach (var itm in listRewardUI)
            {
                if (itm.haveData)
                {
                    TweenControl.GetInstance().DelayCall(itm.transform,inx*0.07f, ()=>
                    {
                        itm.GetComponent<RectTransform>().transform.position = chestAnim.GetComponent<RectTransform>().transform.position;
                    }, () =>
                    {
                        TweenControl.GetInstance().ScaleFromZero(itm.gameObject, 0.3f, () =>
                        {
                            itm.transform.DOPunchScale(Vector3.one*0.2f, 0.4f,1,0.3f);
                        });

                        TweenControl.GetInstance().Move(itm.GetComponent<RectTransform>(),itm.targetAnchorPos ,0.5f,null,EaseType.OutQuint);
                    });
                    inx++;
                }
            }

            if (Aura)
            {
                Aura.gameObject.SetActive(true);
                TweenControl.GetInstance().ScaleFromZero(Aura.gameObject, 0.5f);
                TweenControl.GetInstance().DelayCall(tapToClaim.gameObject.transform, 0.5f, () =>
                {
                    tapToClaim.gameObject.SetActive(true);
                    canClaim = true;
                    TweenControl.GetInstance().ScaleFromZero(textTapToClaim.gameObject, 0.5f);
                });
            }
        });
    }
    public void OnClaimReward()
    {
        if (canClaim)
        {
            canClaim = false;
            foreach (var itr in listDataReward)
            {
                PlayerData.AddCurrency(itr.typeReward,itr.rewardAmount);
            }
            this.gameObject.SetActive(false);
            actionClose?.Invoke();
            PlayerData.Vibrate(HapticTypes.Failure);
            Pool.Get<ConfettiGroup>().StopConfetti();
        }
        Locator.Service.GameResourceDisplay.ToggleDisplayAllResources(true);
    }

    public override void Hide()
    {
        Locator.Service.GameResourceDisplay.ToggleDisplayAllResources(true);
        base.Hide();
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
                return Pos6;
        }
        return Pos1;
    }
}