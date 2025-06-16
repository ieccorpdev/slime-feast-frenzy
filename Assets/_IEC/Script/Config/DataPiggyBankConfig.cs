using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/DataPiggyBankConfig")]
public class DataPiggyBankConfig : ScriptableObject
{
    public int MaxGoldCapByPiggyBank => listGoldMilestone[1];
    public int levelUnlocked = 5;
    public List<int> listGoldMilestone = new List<int>();
    public int goldEarnedEachLevel = 125;

    public bool PassMileStonePiggyBank(int value)
    {
        bool pass = false;
        foreach (var mlStong in listGoldMilestone)
        {
            if (value > mlStong)
            {
                pass = true;
                break;
            }
        }
        return pass;
    }
}


