using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _IEC.Script.Config;
using _IEC.Script.Data.UI.Popup.RelicHunt;
using DG.Tweening;
using IEC;
using IEC.Helper;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupRelicHunt : BasePopup
{
    public GameObject infoGroup;
    private Sequence showInfoSequence = null;
    public Button infoButton;
    public Button closeInfoButton;
    public Button playMainButton;
    public Button playInfoButton;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI relicToClaimText;
    public Sprite goodJobSprite;
    public Sprite wellDoneSprite;
    public Sprite amazingSprite;
    public bool isMaxLevel;
    [SerializeField] private RelicLevelVisual levelVisual;
    [SerializeField] VitrualProgressBar vitrualProgressBar;
    private List<Reward> rewards = new();

    [Header("Info ui")] public List<Transform> listInfoUI = new();


    private void Start()
    {
        SetEvent(infoButton, OnShowInfo);
        SetEvent(closeInfoButton, OnShowInfo);
        SetEvent(playInfoButton, OnPlayBtnClick);
        SetEvent(playMainButton, OnPlayBtnClick);
    }

    // Start is called before the first frame update
    private void OnPlayBtnClick()
    {
    }

    public override void Show()
    {
        var curLevelIndex = EvaluateRelicStatus(out var aboveNode);
        SetRelicToClaim(isMaxLevel ? -1 : aboveNode.tokenCost - PlayerData.relicData.currentRelicTokenCount);
        vitrualProgressBar.SetVitrualProgress(PlayerData.relicData.currentRelicTokenCount, curLevelIndex);
        levelVisual.DoShowLevelVisual(isMaxLevel ? levelVisual.listLevelGO.Length : PlayerData.relicData.curRelicLevel);
        base.Show();
    }

    private int EvaluateRelicStatus(out ArtifactNodeModel aboveNode)
    {
        var curToken = PlayerData.relicData.currentRelicTokenCount;
        int curLevelIndex = PlayerData.relicData.curRelicLevel - 1;
        aboveNode = null;
        
        if (curLevelIndex >= Config.artifactCollectorConfig.ArtifactLevels.Count)
        {
            isMaxLevel = true;
            return curLevelIndex;
        }

        var relicReward = Config.artifactCollectorConfig.ArtifactLevels[curLevelIndex].artifactLevelRewards;
        for (int i = 0; i < relicReward.Count; i++)
        {
            if (relicReward[i].tokenCost > curToken)
            {
                aboveNode = relicReward[i];
                break;
            }
        }

        int maxLevel = Config.artifactCollectorConfig.ArtifactLevels.Count;
        if (aboveNode == null)
        {
            if (curLevelIndex < maxLevel)
            {
                var tokenToLevelUp = relicReward.Max(x => x.tokenCost);
                PlayerData.relicData.LevelUp(tokenToLevelUp);
                aboveNode = Config.artifactCollectorConfig.ArtifactLevels[++curLevelIndex].artifactLevelRewards.First();
            }
            else
            {
                isMaxLevel = true;
            }
        }
        else
        {
            var aboveNodeIndex = relicReward.IndexOf(aboveNode);
            for (int i = 0; i < aboveNodeIndex; i++)
            {
                if (PlayerData.relicData.levelClaimDict[i] != false) continue;
                foreach (var rwd in relicReward[i].artifactRewards)
                {
                    rewards.Add(ConvertToReward(rwd));
                }

                PlayerData.relicData.levelClaimDict[i] = true;
            }
        }

        return curLevelIndex;
    }

    /// <param name="relicLeft">Set negative to disactive text.</param>
    private void SetRelicToClaim(int relicLeft)
    {
        if (relicLeft < 0)
        {
            relicToClaimText.gameObject.SetActive(false);
        }
        else
        {
            relicToClaimText.gameObject.SetActive(true);
            relicToClaimText.text = $"Collect {relicLeft.ToString()} more <sprite=0> to get a reward!";
        }
    }

    public void CheckRelicProgress()
    {
    }

    [Button]
    private void ShowReward()
    {
        List<Reward> rewards = new();
        foreach (var artifactLevelReward in Config.artifactCollectorConfig.ArtifactLevels[0].artifactLevelRewards[0]
                     .artifactRewards)
        {
            rewards.Add(ConvertToReward(artifactLevelReward));
        }

        //Locator.Main.ClaimBonusRewardPanel.SetupTempTitle();
        Locator.Main.ClaimBonusRewardPanel.SetupRewardPanel(rewards, null, true);
        Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel(ClaimBonusRewardSkin.Gift_Box_2);
    }

    private Reward ConvertToReward(ArtifactRewardData relicData)
    {
        Reward rs = new();
        switch (relicData.artifactRewardType)
        {
            case ArtifactRewardType.Gold:
                rs.typeReward = ResourceType.Gold;
                break;
            case ArtifactRewardType.LivesInMinutes:
                rs.typeReward = ResourceType.UnlimitedLive;
                break;
            case ArtifactRewardType.Booster_EvolvePillInMinutes:
                rs.typeReward = ResourceType.BonusExp;
                break;
            case ArtifactRewardType.Booster_WindsurfInMinutes:
                rs.typeReward = ResourceType.WindSurf;
                break;
            case ArtifactRewardType.Helper_CompassInAmount:
                rs.typeReward = ResourceType.Compass;
                break;
            case ArtifactRewardType.Helper_EnlargeInAmount:
                rs.typeReward = ResourceType.Enlarge;
                break;
            case ArtifactRewardType.Helper_MagnetInAmount:
                rs.typeReward = ResourceType.Compass;
                break;
            case ArtifactRewardType.Helper_TimeStopInAmount:
                rs.typeReward = ResourceType.BonusTime;
                break;
        }

        rs.rewardAmount = relicData.value;
        return rs;
    }

    private void OnShowInfo()
    {
        if (infoGroup != null)
        {
            infoGroup.gameObject.SetActive(!infoGroup.gameObject.activeInHierarchy);
        }

        if (infoGroup != null && infoGroup.gameObject.activeInHierarchy)
        {
            if (showInfoSequence != null) DOTween.Kill(showInfoSequence);
            showInfoSequence = DOTween.Sequence();

            foreach (var ui in listInfoUI)
            {
                ui.localScale = Vector3.zero;
            }

            //closeInfoButton.transform.localScale = new Vector3(0, 1, 1);

            float delay = 0f;
            foreach (var ui in listInfoUI)
            {
                showInfoSequence.Insert(delay, ui.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
                delay += 0.1f;
            }

            //showInfoSequence.Insert(delay, closeInfoButton.transform.DOScaleX(1, 0.2f).SetEase(Ease.OutBack));

            showInfoSequence.Play();
        }
    }

    private void Update()
    {
        UpdateTimer();
    }

    private bool IsEndEvent;

    private void UpdateTimer()
    {
        if (IsEndEvent)
        {
            timerText.text = "Finished";
        }
        else
        {
            timerText.text =
                DateTimeHelper.GetFormattedTimeHMS(
                    PlayerData.relicData.eventTimeEnd - DateTime.Now);
        }
    }

    public bool CheckShowPopup()
    {
        return true;
    }
}