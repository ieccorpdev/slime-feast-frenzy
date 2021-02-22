using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DailyMissionConfig", menuName = "Services/DailyMissionConfig")]
public class DailyMissionConfig : ScriptableObject
{
    public string KeyName;
    public int ConditionValue;
    public bool IsCanSkip;
    public int RewardEarn;
    public int SkipCost;
}
