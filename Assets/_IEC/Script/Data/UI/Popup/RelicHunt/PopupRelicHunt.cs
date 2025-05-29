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
    public bool isDoneLevel;
    [SerializeField] private RelicLevelVisual levelVisual;
    [SerializeField] RelicProgressBar relicProgressBar;
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
        if (Locator.Main.ScreenMain.listClaimRewardData.Count > 0 ||
            Locator.Main.ClaimBonusRewardPanel.IsHaveShowClaimRewardAnim()) return;
        if (PlayerData.playerLife <= 0 && !PlayerData.IsInUnlimitedLife())
        {
            Locator.Main.PopupOutOfLive.Show();
        }
        else
        {
            Locator.Main.PopupBooster.Show();
        }
        Locator.Main.PopupRelicHunt.Hide();
    }

    public override void Show()
    {
        isDoneLevel = IsAllRewardClaim();
        ResetVisual();
        //CaculateAndShowReward();
        CaculateReward();

        base.Show();
    }

    private bool IsAllRewardClaim()
    {
        var rewardPerlevel = 3;
        return PlayerData.relicData.claimedRewardIds.Count ==
               Config.artifactCollectorConfig.ArtifactLevels.Count * rewardPerlevel;
    }

    private void ResetVisual()
    {
        var aboveNode = FindNextRewardNode();

        if (aboveNode != null) SetRelicToClaimText(aboveNode.tokenCost - PlayerData.relicData.currentRelicTokenCount);
        else
        {
            SetRelicToClaimText(0, true);
        }

        relicProgressBar.SetVitrualProgress(PlayerData.relicData.currentRelicTokenCount, GetCurrentLevelIndex(),
            isDoneLevel);
        levelVisual.DoShowLevelVisual(PlayerData.relicData.curRelicLevel);
        UpdateButtonPlayState();
    }

    private void CaculateReward()
    {
        var curLevel = PlayerData.relicData.curRelicLevel;
        var config = Config.artifactCollectorConfig.ArtifactLevels[curLevel - 1];
        var relicReward = config.artifactLevelRewards;
        for (var index = 0; index < relicReward.Count; index++)
        {
            var rw = relicReward[index];
            var levelNode = relicProgressBar.levelProgressNodes[index];

            if (rw.tokenCost <= PlayerData.relicData.currentRelicTokenCount)
            {
                if (!PlayerData.relicData.HasRewardedLevel(rw.id))
                {
                    levelNode.ClearReward();
                    levelNode.SetLock(true);
                    foreach (var artifactRewardData in rw.artifactRewards)
                    {
                        levelNode.AddReward(ConvertToReward(artifactRewardData));
                        levelNode.OnClaimReward = () =>
                        {
                            PlayerData.relicData.SetClaimReward(rw.id);
                            UpdateState();
                        };
                    }
                }
                else
                {
                    levelNode.SetLock(false);
                }
            }
            else
            {
                levelNode.SetLock(true);
            }
        }
    }

    private void UpdateState()
    {
        bool isClaimAll = true;
        foreach (var levelProgressNode in relicProgressBar.levelProgressNodes)
        {
            isClaimAll = levelProgressNode.hasClaimReward;
        }

        if (isClaimAll)
        {
            ClaimAllLevelReward();
        }
    }

    private void CaculateAndShowReward()
    {
        bool isMaxLevel = PlayerData.relicData.curRelicLevel >= Config.artifactCollectorConfig.ArtifactLevels.Count;
        var rwd = PlayerData.relicData.GetAllUnClaimRelicLevelReward(Config.artifactCollectorConfig, isMaxLevel);
        foreach (var keyValuePair in rwd)
        {
            rewards.Add(ConvertToReward(new ArtifactRewardData
                { artifactRewardType = keyValuePair.Key, value = keyValuePair.Value }));
        }

        if (rewards.Count > 0)
        {
            ShowReward();
            rewards.Clear();
        }
    }

    private ArtifactNodeModel FindNextRewardNode()
    {
        int curLevelIndex = GetCurrentLevelIndex();
        var relicReward = Config.artifactCollectorConfig.ArtifactLevels[curLevelIndex].artifactLevelRewards;
        int maxLevel = Config.artifactCollectorConfig.ArtifactLevels.Count;
        var curToken = PlayerData.relicData.currentRelicTokenCount;
        var aboveNode = relicReward.FirstOrDefault(t => t.tokenCost > curToken);

        if (aboveNode != null)
        {
            isDoneLevel = false;
            return aboveNode;
        }

        if (curLevelIndex <= maxLevel)
        {
            isDoneLevel = true;
        }


        return aboveNode;
    }


    private void UpdateButtonPlayState()
    {
        if (isDoneLevel)
        {
            PlayerData.relicData.isAvaiableToSpawn = false;

            playMainButton.GetComponentInChildren<TextMeshProUGUI>().text = IsAllRewardClaim() ? "Finished" : "Claim";
            SetEvent(playMainButton, ClaimAllLevelReward);
        }
        else
        {
            PlayerData.relicData.isAvaiableToSpawn = true;
            playMainButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play";
            SetEvent(playMainButton, OnPlayBtnClick);
        }
    }

    private void ClaimAllLevelReward()
    {
        OnLevelUpRelicLevel();
        CaculateAndShowReward();
    }

    private void OnLevelUpRelicLevel()
    {
        int curLevelIndex = GetCurrentLevelIndex();
        var relicReward = Config.artifactCollectorConfig.ArtifactLevels[curLevelIndex].artifactLevelRewards;
        var tokenToLevelUp = relicReward.Max(x => x.tokenCost);
        if (PlayerData.relicData.LevelUp(tokenToLevelUp, Config.artifactCollectorConfig.ArtifactLevels.Count))
        {
            isDoneLevel = false;
            //reset visual
            relicProgressBar.OnNewLevel();
            ResetVisual();
            CaculateReward();
            UpdateButtonPlayState();
        }
    }

    private void OnAllLevelClaimState()
    {
        levelVisual.DoShowLevelVisual(Config.artifactCollectorConfig.ArtifactLevels.Count);
    }

    private int GetCurrentLevelIndex()
    {
        int curLevelIndex = PlayerData.relicData.curRelicLevel - 1;

        if (curLevelIndex >= Config.artifactCollectorConfig.ArtifactLevels.Count)
        {
            isDoneLevel = true;
        }

        return curLevelIndex;
    }

    private void SetRelicToClaimText(int relicLeft, bool isMaxLevel = false)
    {
        relicToClaimText.gameObject.SetActive(true);

        if (isMaxLevel)
        {
            relicToClaimText.text = $"Claim all reward to continue";
            return;
        }

        relicToClaimText.text = $"Collect {relicLeft.ToString()} more <sprite=0> to get a reward!";
    }


    [Button]
    private void ShowReward()
    {
        //Locator.Main.ClaimBonusRewardPanel.SetupTempTitle();
        Locator.Main.ClaimBonusRewardPanel.SetupRewardPanel(rewards, () => { Locator.Main.PopupRelicHunt.Show(); },
            true);
        Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel(ClaimBonusRewardSkin.Gift_Box_3);
        Locator.Main.PopupRelicHunt.Hide();
    }

    private Reward ConvertToReward(ArtifactRewardData relicData)
    {
        Reward rs = new(relicData.value,relicData.artifactRewardType);
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
        if (IsEndEvent || isDoneLevel)
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