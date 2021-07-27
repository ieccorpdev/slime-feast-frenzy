using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace IEC
{
    public class ScreenInGame : ScreenUI
    {
        public override bool IS_PAUSE_GAME => false;

        public TextMeshProUGUI txtLevel;
        public Button btnRestart;

        [Header("Debug")]
        public Button btnLevelWin;
        public Button btnLevelLose;

        public void SetLevel(int level)
        {
            txtLevel.text = "LEVEL " + level.ToString();
        }
    }
}