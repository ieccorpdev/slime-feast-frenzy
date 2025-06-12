using System;
using System.Collections.Generic;
using IEC;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/TrophyReward")]
public class TrophyRewardConfig : ScriptableObject
{
    public List<TroyphyReward> listTrophyReward  = new();

    public TroyphyReward GetReward(TrophyType type)
    {
        var objF = listTrophyReward.Find(x => x.type == type);
        TroyphyReward result = new TroyphyReward(objF.type,objF.listSpiteTrophy,objF.listTimeToCollect,objF.listTrophyReward);
        return result;
    }
}
[Serializable]
public class TroyphyReward
{
    public TroyphyReward(TrophyType type,List<Sprite> listSpiteTrophy,List<int> listTimeToCollect,List<TroyphyRewardDetails> listTrophyReward )
    {
        this.type = type;
        this.listTimeToCollect = listTimeToCollect;
        this.listSpiteTrophy = listSpiteTrophy;
        this.listTrophyReward = listTrophyReward;
    }

    public Sprite GetSpriteByIndex(int index)
    {
        if (listSpiteTrophy == null) return null;
        index = Mathf.Clamp(index,0, listSpiteTrophy.Count-1);
        return listSpiteTrophy[index];
    }
    public TrophyType type;
    public List<Sprite> listSpiteTrophy;
    public List<int> listTimeToCollect;
    public List<TroyphyRewardDetails> listTrophyReward = new();
}
[Serializable]
public class TroyphyRewardDetails
{
    public List<Reward> listTrophyReward = new();
}