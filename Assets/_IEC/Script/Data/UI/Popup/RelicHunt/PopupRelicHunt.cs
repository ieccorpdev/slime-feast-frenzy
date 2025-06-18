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
    public Button claimButton;
    public GameObject finishGroup;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI relicToClaimText;
    public Sprite goodJobSprite;
    public Sprite wellDoneSprite;
    public Sprite amazingSprite;
    [SerializeField] private RelicLevelVisual levelVisual;
    [SerializeField] RelicProgressBar relicProgressBar;
    private List<Reward> rewards = new();
    private ArtifactCollectorConfig relicConfig => Config.relicConfig;

    [Header("Info ui")] public List<Transform> listInfoUI = new();


    private void Start()
    {
        SetEvent(infoButton, OnShowInfo);
        SetEvent(closeInfoButton, OnShowInfo);
        SetEvent(playMainButton, OnPlayBtnClick);
        SetEvent(claimButton, ClaimAllLevelReward);
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
        if (PlayerData.relicData.hasJoinEvent == false)
        {
            DOVirtual.DelayedCall(0.5f, OnShowInfo);
            PlayerData.relicData.hasJoinEvent = true;
        }
        ResetVisual();
        //CaculateAndShowReward();
        UpdateRewardLevelNode();

        base.Show();
    }


    private void ResetVisual()
    {
        var isDoneLevel = PlayerData.relicData.IsDoneLevel(relicConfig);
        var aboveNode = FindNextRewardNode();

        if (aboveNode != null)
        {
            SetRelicToClaimText(relicConfig.GetStackTokenCost(aboveNode.id) - PlayerData.relicData.currentRelicTokenCount);
        }
        else
        {
            SetRelicToClaimText(0, true);
        }

        relicProgressBar.SetVitrualProgress(PlayerData.relicData.currentRelicTokenCount, GetCurrentLevelIndex(),
            isDoneLevel);
        levelVisual.DoShowLevelVisual(PlayerData.relicData.curRelicLevel);
        UpdateButtonState();
    }

    private void UpdateRewardLevelNode()
    {
        var curLevel = PlayerData.relicData.curRelicLevel;
        var config = Config.relicConfig.ArtifactLevels[curLevel - 1];
        var relicReward = config.artifactLevelRewards;
        for (var index = 0; index < relicReward.Count; index++)
        {
            var rw = relicReward[index];
            var levelNode = relicProgressBar.levelProgressNodes[index];

            if (relicConfig.GetStackTokenCost(rw.id) <= PlayerData.relicData.currentRelicTokenCount)
            {
                if (!PlayerData.relicData.HasRewardedLevel(rw.id))
                {
                    levelNode.ClearReward();
                    levelNode.SetState(LevelProgressNode.RewardState.Claimable);
                    foreach (var artifactRewardData in rw.artifactRewards)
                    {
                        levelNode.AddReward(ConvertToReward(artifactRewardData));
                    }
                    levelNode.OnClaimReward = () =>
                    {
                        PlayerData.relicData.SetClaimReward(rw.id);
                        if (PlayerData.relicData.AbleTolevelUp(Config.relicConfig)) OnLevelUpRelicLevel();
                        ResetVisual();
                    };
                }
                else
                {
                    levelNode.SetState(LevelProgressNode.RewardState.Claimed);
                }
            }
            else
            {
                levelNode.SetState(LevelProgressNode.RewardState.Lock);
            }
        }
    }
    

    private void CaculateAndShowReward()
    {
        var rwd = PlayerData.relicData.GetAllUnClaimRelicLevelReward(Config.relicConfig);
        rewards.Clear();
        foreach (var keyValuePair in rwd)
        {
            rewards.Add(ConvertToReward(new ArtifactRewardData
                { artifactRewardType = keyValuePair.Key, value = keyValuePair.Value }));
        }

        if (rewards.Count > 0)
        {
            ShowReward();
        }
    }

    private ArtifactNodeModel FindNextRewardNode()
    {
        int curLevelIndex = GetCurrentLevelIndex();
        var relicReward = Config.relicConfig.ArtifactLevels[curLevelIndex].artifactLevelRewards;
        int maxLevel = Config.relicConfig.ArtifactLevels.Count;
        var curToken = PlayerData.relicData.currentRelicTokenCount;
        var aboveNode = relicReward.FirstOrDefault(t => relicConfig.GetStackTokenCost(t.id) > curToken);

        return aboveNode;
    }


    private void UpdateButtonState()
    {
        var isDoneLevel = PlayerData.relicData.IsDoneLevel(relicConfig);
        PlayerData.relicData.hasDoneLevel = !isDoneLevel;
        bool hasReward = PlayerData.relicData.CheckIfHasReward(Config.relicConfig);
        bool hasLockReward = PlayerData.relicData.CheckIfHasLockReward(Config.relicConfig, true);
        playMainButton.transform.parent.gameObject.SetActive(!hasReward && hasLockReward);
        claimButton.transform.parent.gameObject.SetActive(hasReward);
        finishGroup.SetActive(!hasLockReward && !hasReward);
    }

    private void ClaimAllLevelReward()
    {
        CaculateAndShowReward();
        if (PlayerData.relicData.AbleTolevelUp(Config.relicConfig)) OnLevelUpRelicLevel();
        ResetVisual();
    }

    private void OnLevelUpRelicLevel()
    {
        int curLevelIndex = GetCurrentLevelIndex();
        var relicReward = Config.relicConfig.ArtifactLevels[curLevelIndex].artifactLevelRewards;
        var tokenToLevelUp = relicConfig.GetStackTokenCost(relicReward[^1].id);
        if (PlayerData.relicData.LevelUp(tokenToLevelUp, Config.relicConfig.ArtifactLevels.Count))
        {
            //reset visual
            relicProgressBar.OnNewLevel();
            UpdateRewardLevelNode();
        }
    }
    

    private int GetCurrentLevelIndex()
    {
        int curLevelIndex = PlayerData.relicData.curRelicLevel - 1;

        return curLevelIndex;
    }

    private void SetRelicToClaimText(int relicLeft, bool isLevelDone = false)
    {
        relicToClaimText.gameObject.SetActive(true);

        if (isLevelDone)
        {
            bool HasReward = PlayerData.relicData.CheckIfHasReward(Config.relicConfig);

            relicToClaimText.text = HasReward ? "Claim all reward to continue" : "More treasure will soon come.";
            return;
        }

        relicToClaimText.text = $"Collect {relicLeft.ToString()} more <sprite=0> to get a reward!";
    }

    [Button]
    private void ShowReward()
    {
        //Locator.Main.ClaimBonusRewardPanel.SetupTempTitle();
        Locator.Main.ClaimBonusRewardPanel.SetupRewardPanel(rewards, () =>
            {
                
                // unlock relic and can't levle up, mean the last reward 
                if (!PlayerData.relicData.CheckIfHasLockReward(Config.relicConfig) )
                {
                    PlayerData.trophyData.CollectTrophy(TrophyType.RelicHunt);
                    Locator.Main.HomeScreenTab.CheckShowTrophyPopup();
                    Locator.Main.HomeScreenTab.ShowNextPopup();
                }
                else
                {
                    Locator.Main.PopupRelicHunt.Show();
                }
            },
            true);
        Locator.Main.ClaimBonusRewardPanel.ShowingRewardPanel(ClaimBonusRewardSkin.Gift_Box_3);
        Locator.Main.PopupRelicHunt.Hide();
    }

    private Reward ConvertToReward(ArtifactRewardData relicData)
    {
        Reward rs = new(relicData.value, relicData.artifactRewardType);
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
            for (var index = 0; index < listInfoUI.Count; index++)
            {
                var ui = listInfoUI[index];
                showInfoSequence.Insert(delay, ui.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
                delay += 0.1f;
            }
            var scaleMulti = 1.1f;
            var duration = 0.6f;
            showInfoSequence.AppendCallback(() =>
            {
                listInfoUI[^1].transform.localScale = Vector3.one;
                listInfoUI[^1].transform.DOScale(Vector3.one * scaleMulti, duration)
                    .SetLoops(-1, LoopType.Yoyo);
            });
            

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
        var isDoneLevel = PlayerData.relicData.IsDoneLevel(relicConfig);

        if (IsEndEvent || isDoneLevel)
        {
            timerText.text = "Finished";
        }
        else
        {
            var timeLeft = PlayerData.relicData.eventTimeEnd - DateTime.Now;
            if (timeLeft.TotalSeconds < 0)
            {
                Locator.Main.PopupRelicHunt.Hide();
            }
            else
            {
                timerText.text = GetTimeFormat(timeLeft);
            }
        }
    }

    private string GetTimeFormat(TimeSpan time)
    {
        string outputString = "";

        var day = time.Days;
        var hour = time.Hours;
        var minute = time.Minutes;
        var seconds = time.Seconds;
        string days = day < 10 ? $"0{day}" : day.ToString();
        string hours = hour < 10 ? $"0{hour}" : hour.ToString();
        string minutes = minute < 10 ? $"0{minute}" : minute.ToString();
        string second = seconds.ToString();
        if (day > 0)
        {
            outputString = $"{days}d {hours}h";
        }
        else if (hour > 0)
        {
            outputString = $"{hours}h {minutes}m";
        }
        else
        {
            outputString = $"{minutes}m {second}s";
        }

        return outputString;
    }

    public bool CheckShowPopup()
    {
        return true;
    }
}