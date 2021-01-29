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

        [SerializeField]
        private Button m_BtnGetx5, m_BtnNoThanks;

        private const string COIN = "<sprite=\"coin\" index=0>";

        [SerializeField]
        private RectTransform m_RectBot;

        public void OnInit()
        {
            m_BtnGetx5.onClick.AddListener(() => {
                EventController.RaiseEvent(EventList.GamePlay.END_GAME_GET_REWARD, new EventParam(this, true));
            });

            m_BtnNoThanks.onClick.AddListener(() => {
                EventController.RaiseEvent(EventList.GamePlay.END_GAME_GET_REWARD, new EventParam(this, false));
            });
        }

        public void SetGoldReward(int gold)
        {
            m_TxtGold.text = $"{StringHelper.GetNumber(gold)} {COIN}";
            m_TxtGO.text = GameOverGroup.TextWinBonus[Random.Range(0, GameOverGroup.TextWinBonus.Count)];
        }

        public void SetPosGroupBottom(float bannerHeight)
        {
            m_RectBot.anchoredPosition = new Vector2(0f, bannerHeight);
        }
    }
}