using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace IEC
{
    public class GameOverRewardGroup : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_TxtGO, m_TxtGold;

        public Button btnGetx5, btnNoThanks;

        private const string COIN = "<sprite=\"coin\" index=0>";

        [SerializeField]
        private RectTransform m_RectBot;

        public void OnInit()
        {

        }

        public void SetGoldReward(int gold)
        {
            m_TxtGold.text = $"{StringHelper.GetNumber(gold)} {COIN}";
            m_TxtGO.text = GameOverGroup.TextWinBonus[Random.Range(0, GameOverGroup.TextWinBonus.Count)];
        }

        public void SetPosGroupBottom(float bannerHeight)
        {
            m_RectBot.anchoredPosition = new Vector2(0f, bannerHeight + 150f);
        }
    }
}