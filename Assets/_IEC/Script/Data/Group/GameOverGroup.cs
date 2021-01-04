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

        [SerializeField]
        private Image m_ImgBlur;

        private Tween m_GameOver;

        private static readonly List<string> TextWinBonus = new List<string>
        {"You've done well", "Cool", "Bravo", "Awesome!", "Great", "Nicely Done!"};

        private float timeEffect = .4f;

        public void OnInit()
        {
            m_BtnNext.onClick.AddListener(ButtonRestart);
        }

        private void ButtonRestart()
        {
            if (Game.isHaveUIAnimation)
            {
                TweenGameOver(false, () => {
                    EventController.RaiseEvent(EventList.GamePlay.GAME_RESTART);
                });
            }
            else
            {
                EventController.RaiseEvent(EventList.GamePlay.GAME_RESTART);
            }
        }

        public void SetTextGameOver(bool isWin)
        {
            m_TxtGO.text = isWin ? TextWinBonus[Random.Range(0, TextWinBonus.Count)] : "Mission Fail!";
        }

        public void TweenGameOver(bool isActive, System.Action callback = null)
        {
            m_RectPanelGO.anchoredPosition = new Vector2(0f, isActive ? 2000f : 300f);

            var ease = isActive ? Ease.OutBack : Ease.InBack;
            m_GameOver?.Kill(true);
            Sequence sequence = DOTween.Sequence();
            m_GameOver = sequence.Join(m_ImgBlur.DOFade(isActive ? .3f : 0f, timeEffect))
                .Join(m_RectPanelGO.DOAnchorPosY(isActive ? 300f : 2000f, timeEffect).SetEase(ease));

            sequence.OnComplete(() => {
                callback?.Invoke();
            });
        }
    }
}