using System.Collections;
using System.Collections.Generic;
using _IECModules;
using IEC;
using NaughtyAttributes;
using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LeaderboardModule.Login("wspp-3");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button]
    public void TestSubmitScore()
    {
        LeaderboardModule.SubmitScore("Level", 100);
    }

    [Button]
    public void TestGetGlobalLB()
    {
        LeaderboardModule.GetGlobalLB("Level", 1);
    }

    [Button]
    public void TestGetCountryLB()
    {
        LeaderboardModule.GetCountryLB("Level", 1);
    }
}
