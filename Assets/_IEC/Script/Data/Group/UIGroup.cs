using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

namespace IEC
{
    public class UIGroup : _BaseBehaviour, IGroup
    {
        public Canvas canvas;
        public CanvasScaler canvasScaler;
        public EventSystem eventSystem;

        public RectTransform bottomAds;
        public TMP_Text textHello, txtLevel, txtGO, txtUndo, txtMove;

        public GameObject groupGameOver;

        public Button btnRestart, btnSkip, btnGONext, btnSetting, btnHint, btnUndo;

        public PopupSetting popupSettings;

        [SerializeField]
        private RectTransform rectTopPanel, rectPanelGO, rectPanelBossLevel, rectTextBossLevel;

        private Tween m_Tween;
        private Tween m_GameOver;

        private float effectDuration = 0.25f;

        public GameObject undoAds, undoCount;

        public void OnVibrateHint()
        {
            m_Tween?.Kill(true);
            m_Tween = btnHint.GetComponent<RectTransform>().DOShakeRotation(1f, new Vector3(0, 0, Random.Range(8f, 12f)), Random.Range(15, 20)).SetLoops(Random.Range(1,3));
        }

        public void TweenPannelTop(bool isActive, System.Action callBack = null)
        {
            if (Game.isHaveUIAnimation)
            {
                m_Tween?.Kill(true);
                rectTopPanel.anchoredPosition = new Vector2(0f, isActive ? 500f : 0f);
                Ease ease = isActive ? Ease.OutBack : Ease.InBack;
                m_Tween = rectTopPanel.DOAnchorPosY(isActive ? 0f : 500f, .5f).SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        callBack?.Invoke();
                    });
            }
            else
            {
                rectTopPanel.anchoredPosition = new Vector2(0f, 0f);
            }
        }

        public void TweenGameOver(bool isActive)
        {
            groupGameOver.SetActive(isActive);
            txtGO.transform.localScale = Game.isHaveUIAnimation ? Vector3.one * .1f : Vector3.one;
            rectPanelGO.anchoredPosition = new Vector2(Game.isHaveUIAnimation ? -1500f : 0f, 80f);
            btnGONext.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Game.isHaveUIAnimation ? -1500f: -450f);

            if (Game.isHaveUIAnimation && isActive)
            {
                m_GameOver?.Kill(true);
                TweenPannelTop(false, () => {
                    Sequence sequence = DOTween.Sequence();
                    m_GameOver = sequence.Append(rectPanelGO.DOAnchorPosX(0f, effectDuration))
                        .Append(txtGO.transform.DOScale(Vector3.one, effectDuration))
                        .Append(btnGONext.GetComponent<RectTransform>().DOAnchorPosY(-450f, effectDuration));
                });
            }
        }

        private float effectBossDuration = .35f;
        public void TweenBossLevel()
        {
            m_GameOver?.Kill(true);
            Sequence sequence = DOTween.Sequence();
            rectTextBossLevel.anchoredPosition = new Vector2(-1500f, 85f);
            rectPanelBossLevel.localScale = new Vector3(1f, 0f);

            rectTextBossLevel.gameObject.SetActive(true);
            rectPanelBossLevel.gameObject.SetActive(true);

            sequence.Insert(0f, rectPanelBossLevel.DOScaleY(1f, effectDuration))
                .Insert(.5f, rectTextBossLevel.DOAnchorPosX(0f, effectBossDuration).SetEase(Ease.OutBack))
                .Insert(1.5f, rectTextBossLevel.DOAnchorPosX(1500f, effectBossDuration).SetEase(Ease.InBack))
                .Insert(2f, rectPanelBossLevel.DOScaleY(0f, effectDuration))
                .OnComplete(() => {
                    rectTextBossLevel.gameObject.SetActive(false);
                    rectPanelBossLevel.gameObject.SetActive(false);
                });
        }

        public void LocateGroup()
        {
            Pool.Set(this);
        }
    }
}