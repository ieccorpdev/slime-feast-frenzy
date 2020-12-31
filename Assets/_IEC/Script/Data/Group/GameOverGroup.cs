using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace IEC
{
    public class GameOverGroup : MonoBehaviour, IUIGroup
    {
        private Game m_Game;
        private Game Game => m_Game ?? Pool.Get<Game>();

        [SerializeField]
        private TMP_Text m_TxtGO;

        [SerializeField]
        private RectTransform m_RectPanelGO;

        [SerializeField]
        private Button m_BtnNext;

        private Tween m_GameOver;

        private static readonly List<string> TextWinBonus = new List<string>
        {"You've done well", "Cool", "Bravo", "Awesome!", "Great", "Nicely Done!"};

        public void OnInit()
        {
            m_BtnNext.onClick.AddListener(() => {
                EventController.RaiseEvent(EventList.GamePlay.GAME_RESTART);
            });
        }

        public void SetTextGameOver(bool isWin)
        {
            m_TxtGO.text = isWin ? TextWinBonus[Random.Range(0, TextWinBonus.Count)] : "Mission Fail!";
        }

        public void TweenGameOver(bool isActive)
        {
            //txtGO.text = isWin ? TextWinBonus[Random.Range(0, TextWinBonus.Count)] : "Mission Fail!";
            //txtGO.transform.localScale = Game.isHaveUIAnimation ? Vector3.one * .1f : Vector3.one;
            //rectPanelGO.anchoredPosition = new Vector2(Game.isHaveUIAnimation ? -1500f : 0f, 80f);
            //btnNext.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Game.isHaveUIAnimation ? -1500f : -450f);

            //if (Game.isHaveUIAnimation && isActive)
            //{
            //    m_GameOver?.Kill(true);
            //    TweenPannelTop(false, () =>
            //    {
            //        Sequence sequence = DOTween.Sequence();
            //        m_GameOver = sequence.Append(rectPanelGO.DOAnchorPosX(0f, effectDuration))
            //            .Append(txtGO.transform.DOScale(Vector3.one, effectDuration))
            //            .Append(btnGONext.GetComponent<RectTransform>().DOAnchorPosY(-450f, effectDuration));
            //    });
            //}
        }
    }
}