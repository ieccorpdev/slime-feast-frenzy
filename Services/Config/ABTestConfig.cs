using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}