using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;

public class LeaderboardController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        IECLeaderboard.Login();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
