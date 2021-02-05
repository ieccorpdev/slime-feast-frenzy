using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace IEC
{
    public class GameOverGroupNew : MonoBehaviour
    {
        [SerializeField]
        private RectTransform m_RectBot;

        [SerializeField]
        private TextMeshProUGUI m_TxtGO;

        [SerializeField]
        private Button m_BtnNext;

        public void OnInit()
        {
            m_BtnNext.onClick.AddListener(() => {
                EventController.RaiseEvent(EventList.GamePlay.GAME_RESTART);
            });
        }

        public void SetPosGroupBottom(float bannerHeight)
        {
            m_RectBot.anchoredPosition = new Vector2(0f, bannerHeight + 30f);
        }

        public void SetTextGameOver(bool isWin)
        {
            m_TxtGO.text = isWin ? GameOverGroup.TextWinBonus[Random.Range(0, GameOverGroup.TextWinBonus.Count)] : "Mission Fail!";
        }
    }
}