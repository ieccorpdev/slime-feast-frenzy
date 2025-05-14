using System.Collections;
using System.Collections.Generic;
using _IECModules;
using Cysharp.Threading.Tasks;
using IEC;
using Sirenix.OdinInspector;
using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LeaderboardModule.Login(gameCode: "wspp-3").Forget();
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
