using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupRankInfo : BasePopup
    {
        public GameObject glowObj;
        public List<Transform> lstRanks = new List<Transform>();
        private void Start()
        {
            btnClose.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
            });
            int index = (int)LeaderboardData.currentRankView;
            glowObj.transform.position = lstRanks[index].transform.position;
        }
        public override void Show()
        {
            gameObject.SetActive(true);
            int index = (int)LeaderboardData.currentRankView;
            glowObj.transform.position = lstRanks[index].transform.position;
        }
    }
}