using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using IEC;
using IEC.Wind.Leaderboard;
using UnityEngine;
using UnityEngine.UI;

public class ClaimLeaderBoardFinish : ClaimBonusRewardPanel
{
    public GameObject otherSafe;
    public GameObject rankUp;
    public GameObject rankDown;

    public Image leagueImage;
    public GameObject effectAura;
    public void SetData(LeaderboardRankType currentLeague, TypeFinish typeFinish, List<RewardData> rewards)
    {
        otherSafe.SetActive(false);
        rankUp.SetActive(false);
        rankDown.SetActive(false);
        effectAura.SetActive(false);
        switch (typeFinish)
        {
            case TypeFinish.Safe:
                Aura = otherSafe;
                otherSafe.SetActive(true);
                effectAura.gameObject.SetActive(true);
                break;
            case TypeFinish.Up:
                Aura = rankUp;
                rankUp.SetActive(true);
                effectAura.gameObject.SetActive(true);
                break;
            case TypeFinish.Down:
                Aura = rankDown;
                rankDown.SetActive(true);
                break;
        }

        leagueImage.sprite = LeaderboardConfig.GetLeagueIconRank();
        leagueImage.SetNativeSize();
        List<Reward> rawReward = new List<Reward>();
        foreach (var rw in rewards)
        {
            rawReward.Add(new Reward(rw.amount,rw.type));
        }
        rewards.Sort((x, y) => y.type.CompareTo(x.type));
        
        SetupRewardPanel(rawReward,OnCloseLeaderBoardFinish);
    }

    public void OnCloseLeaderBoardFinish()
    {
        if (Locator.Main.PopupLeaderboard.gameObject.activeInHierarchy)
        {
            // Locator.Main.PopupLeaderboard.LoadData();
            // Locator.Main.PopupLeaderboard.ReloadData();
            Locator.Main.PopupLeaderboard.Show();
        }
        Hide();
    }
}