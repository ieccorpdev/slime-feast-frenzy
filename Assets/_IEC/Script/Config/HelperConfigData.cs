using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/HelperConfigData")]
public class HelperConfigData : ScriptableObject
{
    public List<HelperData> helperDatas;
    public List<HelperData> helperDatasOverrideAB;
    public List<HelperData> helperDatasLostOverrideAB;
    public int maxExpandHelper = 3;
    public HelperData GetHelperData(ResourceType typeHelper, int usedCount)
    {
        var data =  helperDatas.Find(x => x.typeHelper == typeHelper && x.usedCount == usedCount);
        if (data == null)
        {
            data =  helperDatas.Find(x => x.typeHelper == typeHelper && x.usedCount == 0);
        }

        return data;
    }
    
    public HelperData GetHelperDataOverrideAB(ResourceType typeHelper, int usedCount)
    {
        var data =  helperDatasOverrideAB.Find(x => x.typeHelper == typeHelper && x.usedCount == usedCount);
        if (data == null)
        {
            data =  helperDatasOverrideAB.Find(x => x.typeHelper == typeHelper && x.usedCount == 0);
        }

        return data;
    }
    
    public HelperData GetHelperDataLostOverrideAB(ResourceType typeHelper, int usedCount)
    {
        var data =  helperDatasLostOverrideAB.Find(x => x.typeHelper == typeHelper && x.usedCount == usedCount);
        if (data == null)
        {
            data =  helperDatasLostOverrideAB.Find(x => x.typeHelper == typeHelper && x.usedCount == 0);
        }

        return data;
    }
}

[Serializable]
public class HelperData
{
    public int usedCount;
    public ResourceType typeHelper;
    public int levelUnlock;
    public int GoldCost;
}

