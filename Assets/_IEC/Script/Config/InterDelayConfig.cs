using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InterDelayConfig", menuName = "Game/InterDelayConfig")]
public class InterDelayConfig : ScriptableObject
{
    public int inter30CountRequire;
    public int inter45CountRequire;
    public int inter60CountRequire;
    public int inter90CountRequire;
    public int inter120CountRequire;
}
