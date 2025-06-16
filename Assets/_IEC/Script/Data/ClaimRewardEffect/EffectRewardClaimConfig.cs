using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IEC;

[CreateAssetMenu(menuName = "Game/EffectRewardClaimConfig")]
public class EffectRewardClaimConfig : ScriptableObject
{
    public List<EffectRewardClaimSprite> listEffectSprite = new List<EffectRewardClaimSprite>();
    
    public Sprite GetRewardSprite(ResourceType rewardType)
    {
        return listEffectSprite.Find(data => data.rewardType == rewardType).sprite;
    }
}

[System.Serializable]
public class EffectRewardClaimSprite
{
    public ResourceType rewardType;
    public Sprite sprite;
}