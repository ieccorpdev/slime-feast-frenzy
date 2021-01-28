using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace IEC
{
    public class MainUIGroup : MonoBehaviour, IUIGroup
    {
        private Game m_Game;
        private Game Game => m_Game ?? Pool.Get<Game>();

        public Button btnRestart, btnSkip, btnSetting, btnHint, btnUndo, btnTime, btnShop, btnNoAds;

        [SerializeField]
        private RectTransform rectTopPanel, rectPanelBossLevel, rectTextBossLevel, rectAddTime;

        [SerializeField]
        private TMP_Text txtUndo, txtHintCount, txtTimer;

        private Tween m_Tween;

        private float effectDuration = 0.25f;

        public GameObject undoAds, undoCount;

        public GameObject hintAds, hintCount;

        [SerializeField]
        private Gradient m_MoveColor;


        public void OnInit()
        {
            
        }

        public void SetActiveTopButtons(bool isActive)
        {
            btnSetting.interactable = isActive;
            btnSkip.interactable = isActive;
            btnRestart.interactable = isActive;
        }

        public void SetupAbUI()
        {
            btnSetting.gameObject.SetActive(Game.isHaveSetting);
            btnSkip.gameObject.SetActive(!Game.isHaveHint);
            btnShop.gameObject.SetActive(Game.isHaveShop);
        }

        public void SetupBossLevel(bool isBossLevel)
        {
            btnTime.gameObject.SetActive(Game.isHaveBossTimerLevels && isBossLevel);
        }

        public void OnVibrateHint()
        {
            m_Tween?.Kill(true);
            m_Tween = btnHint.GetComponent<RectTransform>().DOShakeRotation(1f, new Vector3(0, 0, 10f), 16).SetLoops(2);
        }

        public void TweenPannelTop(bool isActive, System.Action callBack = null)
        {
            rectTopPanel.anchoredPosition = new Vector2(0f, 0f);
        }

        private float effectBossDuration = .35f;
        public void TweenBossLevel(System.Action callback)
        {
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

                    callback?.Invoke();
                });
        }

        public void UpdateUndoText()
        {
            txtUndo.text = StringHelper.GetNumber(Game.undoRemains);
            undoAds.SetActive(Game.undoRemains <= 0);
            undoCount.SetActive(Game.undoRemains > 0);
        }

        public void UpdateHintText()
        {
            txtHintCount.text = StringHelper.GetNumber(Game.hintRemains);
            hintAds.SetActive(Game.hintRemains <= 0 || !Game.isHintStyleBSP);
            hintCount.SetActive(Game.isHintStyleBSP && Game.hintRemains > 0);
        }

        public void OnEffectTime(bool isActive)
        {
            if (rectAddTime.gameObject.activeSelf == isActive) return;
            rectAddTime.gameObject.SetActive(isActive);
            
            btnTime.GetComponent<Image>().raycastTarget = isActive;
            rectAddTime.localRotation = Quaternion.Euler(0f, 0f, 10f);

            m_Tween?.Kill(true);
            if (isActive)
            {
                Quaternion quaternion = Quaternion.Euler(0f, 0f, -10f);
                m_Tween = rectAddTime.DOLocalRotateQuaternion(quaternion, .4f).SetLoops(-1, LoopType.Yoyo);
            }
        }

        public void UpdateMove()
        {
            return;
            //txtMove.text = "Moves: " + StringHelper.GetNumber(Game.moveRemains);
            //float valueColor = (float)Game.moveRemains / Game.totalMove;
            //txtMove.color = m_MoveColor.Evaluate(valueColor);
        }

        public void SetTextTimer(int time)
        {
            txtTimer.text = StringHelper.GetTime(time);
        }
    }
}