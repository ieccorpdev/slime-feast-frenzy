using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/DataDailyReward")]
public class DataDailyReward : ScriptableObject
{
    public int levelUnlock = 16;
    public Reward mainReward;
    public List<Reward> dailyRewards;
    
}