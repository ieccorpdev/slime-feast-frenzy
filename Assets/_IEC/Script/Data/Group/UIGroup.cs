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
        public TMP_Text textHello, txtLevel, txtGO;

        public GameObject groupGameOver;

        public Button btnRestart, btnSkip, btnGONext, btnSetting, btnHint;

        public PopupSetting popupSettings;

        [SerializeField]
        private RectTransform rectTopPannel, rectPannelGO;

        private Tween m_Tween;
        private Tween m_GameOver;

        private float effectDuration = 0.3f;

        public void OnVibrateHint()
        {
            m_Tween?.Kill(true);
            m_Tween = btnHint.GetComponent<RectTransform>().DOShakeRotation(1f, new Vector3(0, 0, Random.Range(8f, 12f)), Random.Range(15, 20)).SetLoops(Random.Range(1,3));
        }

        public void TweenPannelTop(bool isActive, System.Action callBack = null)
        {
            if (Game.isUIEffect)
            {
                m_Tween?.Kill(true);
                rectTopPannel.anchoredPosition = new Vector2(0f, isActive ? 500f : 0f);
                Ease ease = isActive ? Ease.OutBack : Ease.InBack;
                m_Tween = rectTopPannel.DOAnchorPosY(isActive ? 0f : 500f, .5f).SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        callBack?.Invoke();
                    });
            }
            else
            {
                rectTopPannel.anchoredPosition = new Vector2(0f, 0f);
            }
        }

        public void TweenGameOver(bool isActive)
        {
            groupGameOver.SetActive(isActive);
            txtGO.transform.localScale = Game.isUIEffect ? Vector3.one * .1f : Vector3.one;
            rectPannelGO.anchoredPosition = new Vector2(Game.isUIEffect ? -1500f : 0f, 80f);
            btnGONext.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Game.isUIEffect ? -1500f: -450f);

            if (Game.isUIEffect && isActive)
            {
                m_GameOver?.Kill(true);
                TweenPannelTop(false, () => {
                    Sequence sequence = DOTween.Sequence();
                    m_GameOver = sequence.Append(rectPannelGO.DOAnchorPosX(0f, effectDuration))
                        .Append(txtGO.transform.DOScale(Vector3.one, effectDuration))
                        .Append(btnGONext.GetComponent<RectTransform>().DOAnchorPosY(-450f, effectDuration));
                });
            }
        }

        public void LocateGroup()
        {
            Pool.Set(this);
        }
    }
}