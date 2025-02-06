using System.Collections;
using System.Collections.Generic;
using IEC;
using NaughtyAttributes;
using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        IECLeaderboard.Login("wspp-3");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button]
    public void TestSubmitScore()
    {
        IECLeaderboard.SubmitScore("Level", 100);
    }

    [Button]
    public void TestGetGlobalLB()
    {
        IECLeaderboard.GetGlobalLB("Level", 1);
    }

    [Button]
    public void TestGetCountryLB()
    {
        IECLeaderboard.GetCountryLB("Level", 1);
    }
}
