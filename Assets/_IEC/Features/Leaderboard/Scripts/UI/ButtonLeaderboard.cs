using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ButtonLeaderboard : ScreenBehaviour
    {
        public Image icon;
        public Button btn; 
       
        public TextMeshProUGUI txtTimeLeaderboard;

        private void Start()
        {
            SetEvent(btn, () =>
            {
                Locator.Main.PopupLeaderboard.Show();
            });
        }

        public void UpdateUI()
        {
            icon.sprite = LeaderboardConfig.GetSpirteCurrentLeague();
            icon.SetNativeSize();
            var rect = icon.GetComponent<RectTransform>();
            if (rect.sizeDelta.x >= 300) icon.transform.localScale = Vector3.one * .4f;
            else if (rect.sizeDelta.x >= 200) icon.transform.localScale = Vector3.one * .5f;
            else icon.transform.localScale = Vector3.one * .6f;
        }

        private void Update()
        {
            txtTimeLeaderboard.text = LeaderboardController.GetTimeRemainText();
        }
    }
}