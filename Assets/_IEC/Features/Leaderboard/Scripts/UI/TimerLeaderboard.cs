using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace IEC
{
    public class TimerLeaderboard : _BaseBehaviour
    {
        public TextMeshProUGUI txtTimeLeaderboard;
        void Update()
        {
            if (Game.isHaveLeaderboard)
            {
                txtTimeLeaderboard.text = LeaderboardController.GetTimeRemainText();
            }
        }
    }
}