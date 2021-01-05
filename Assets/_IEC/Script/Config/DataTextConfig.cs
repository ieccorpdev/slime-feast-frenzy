using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InterDelayConfig", menuName = "Game/Data Text")]
public class DataTextConfig : ScriptableObject
{
    public int totalItem;
    public List<string> listText = new List<string>();
}
