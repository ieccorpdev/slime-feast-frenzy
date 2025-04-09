using System;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay.ObserverEventData;
using DG.Tweening;
using IEC;
using UnityEngine;

namespace _IEC.Features.Tutorial
{
    public class TutorialStartGame : _BaseBehaviour
    {
        [SerializeField] private RectTransform moveYourHole;
        [SerializeField] private RectTransform collectInYourHole;
        [SerializeField] private RectTransform myRectTransform;
        [SerializeField] private RectTransform hintCollectItems;
        private Transform m_HoleTransform;
        private RectTransform m_CurrentHint;
        private State m_CurrentState;
        private string DelayTweenId => gameObject.GetInstanceID() + "delay";
        
        public void StartTutorial(Transform holeTransform)
        {
            m_CurrentState = State.None;
            m_HoleTransform = holeTransform;
            m_CurrentHint = moveYourHole;
            ChangeState(State.Move);
        }
        private void OnEnable()
        {
            
            this.RegisterListener(EventList.GamePlay.COLLECT_ITEM,OnCollectItem);
        }

        private void OnDisable()
        {
            DOTween.Kill(DelayTweenId);
            this.UnregisterListener(EventList.GamePlay.COLLECT_ITEM,OnCollectItem);
        }

        private void OnCollectItem(object param)
        {
            var data = (CollectItemData) param;
            //Debug.Log(data.itemID);
            if (data.itemID == "Banana_NoSkin_Slice" || data.itemID == "Apricot")
            {
                ChangeState(State.CollectItems);
            }
        }
        
        
        private void Update()
        {
            if (m_CurrentHint == null) return;
            if (m_CurrentState == State.Move)
            {
                if (Joystick.Direction.magnitude > 0)
                {
                    ChangeState(State.Collect);
                }
            }
            
        }

        
        private void ChangeState(State newState)
        {
            if (newState == m_CurrentState) return;
            m_CurrentState = newState;
            moveYourHole.gameObject.SetActive(false);
            switch (newState)
            {
                case State.Move:
                    moveYourHole.transform.localScale = Vector3.one * 0.1f;
                    moveYourHole.gameObject.SetActive(true);
                    moveYourHole.transform.DOScale(Vector3.one, .3f).SetEase(Ease.OutBack);
                    m_CurrentHint = moveYourHole;
                    collectInYourHole.gameObject.SetActive(false);
                    break;
                case State.Collect:
                    moveYourHole.transform.DOScale(Vector3.one * .1f, .3f).SetEase(Ease.OutBack).OnComplete(() =>
                    {
                        moveYourHole.gameObject.SetActive(false);
                        collectInYourHole.transform.localScale = Vector3.one * 0.1f;
                        collectInYourHole.gameObject.SetActive(true);
                        collectInYourHole.transform.DOScale(Vector3.one, .3f).SetEase(Ease.OutBack);
                    });
                    m_CurrentHint = collectInYourHole;
                    break;
                case State.CollectItems:
                    DOVirtual.DelayedCall(0.1f, () =>
                    {
                        collectInYourHole.gameObject.SetActive(false);
                        hintCollectItems.gameObject.SetActive(true);
                    }).SetId(DelayTweenId);
                    break;
            }
            m_CurrentHint.anchoredPosition =
                m_HoleTransform.position.GetLocalAnchorPos(myRectTransform, GamePlay.mainCamera, GamePlay.uiCamera);
        }

        private enum State
        {
            None,
            Move,
            Collect,
            CollectItems
        }
    }
}
