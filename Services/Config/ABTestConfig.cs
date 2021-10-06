using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "ABTestConfig", menuName = "Services/ABTestConfig")]
public class ABTestConfig : ScriptableObject
{
    public string KeyName;
    public bool IsActive;
    public bool IsOnlyNewInstall;
    public bool IsOverride;
    public string[] ListEventsAB;
    public int[] ListRandomRatio;
    public int DefaultValue;
    public bool IsForceValue;
    public int ForceValue;

    [Button]
    public void SetDefaultRatioAndAB()
    {
        ListRandomRatio = new int[ListEventsAB.Length];
        for(int i = 0; i < ListRandomRatio.Length; i++)
        {
            ListRandomRatio[i] = 1;
        }

        var firstEventAB = ListEventsAB.First();
        var version = int.Parse(firstEventAB.Split(new[]{"_v"}, StringSplitOptions.None)[1]);
        
        for(var i = 1; i < ListEventsAB.Length; i++)
        {
            ListEventsAB[i] = $"{KeyName}_v{version + i}";
        }
        
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}