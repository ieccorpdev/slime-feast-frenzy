using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/FeatureUnlockConfig")]
public class FeatureUnlockConfig : ScriptableObject
{
    public List<FeaturePopupData> featurePopupDatas;

    public bool IsFeatureUnlockedAtLevel(NewFeature featureType, int level)
    {
        var feature = featurePopupDatas.Find(x => x.newFeatureType == featureType);

        if (feature == null)
            return false;
        
        return level >= feature.level;
    }
}

[Serializable]
public class FeaturePopupData
{
    public NewFeature newFeatureType;
    public Sprite sprite;
    public Sprite spriteBG;
    public Sprite spriteFill;
    public Sprite topBarSprite;
    public int level;
    public string skinName;
    public string des;
}

public enum NewFeature
{
    None,
    HiddenCube,
    NewCubePink,
    NewCubeYellow,
    NewCubeRed,
    HiddenBox,
    DoubleBox,
}
