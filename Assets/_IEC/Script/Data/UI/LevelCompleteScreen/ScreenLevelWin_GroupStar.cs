using System;
using System.Collections.Generic;
using _IEC.Features.Gameplay;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace IEC
{
    public class ScreenLevelWin_GroupStar : _BaseBehaviour
    {
        [SerializeField] private Image starLeft;
        [SerializeField] private Image starMid;
        [SerializeField] private Image starRight;
        private string m_tween_ID => gameObject.GetInstanceID().ToString();
        
        private List<GameObject> m_starAnimObjects = new();

        public int StarWon { get; private set; }
        float startWaitTime = 0.3f;
        float timeEachStep = 0.25f;
        float timeDelayEachStep = 0.1f;

        private void OnEnable()
        {
            //DeactiveGroupStar();
        }

        private void DeactiveGroupStar()
        {
            starLeft.gameObject.SetActive(false);
            starMid.gameObject.SetActive(false);
            starRight.gameObject.SetActive(false);
        }

        public void DoShowStarEffect(GameplayTimer.RemainingTimeState remainingTimeState)
        {
            DeactiveGroupStar();
            
            void DoMoveStarAnim(Transform starTransform, Transform timerStarTransform,float delay = 0f, Action onComplete = null)
            {
                starTransform.gameObject.SetActive(true);
                
                var targetPos = starTransform.position;
                var targetRotation = starTransform.rotation;
                var targetScale = starTransform.localScale;
                
                RectTransform starRect = starTransform as RectTransform;
                RectTransform timerStarRect = timerStarTransform as RectTransform;
                
                var targetSize = starRect.sizeDelta;
                starRect.sizeDelta = timerStarRect.sizeDelta;
                
                starTransform.position = timerStarTransform.position;
                starTransform.rotation = timerStarTransform.rotation;
                starTransform.localScale = timerStarTransform.localScale;
                timerStarTransform.gameObject.SetActive(false);

                starTransform.DOMove(targetPos, timeEachStep).SetEase(Ease.InOutQuart).SetDelay(delay);
                starTransform.DORotate(targetRotation.eulerAngles, timeEachStep).SetEase(Ease.InOutQuart).SetDelay(delay);
                //starTransform.DOScale(targetScale, timeEachStep).SetEase(Ease.OutQuad).SetDelay(delay).OnComplete(()=>onComplete?.Invoke());
                
                DOTween.To(() => starRect.sizeDelta, x => starRect.sizeDelta = x, targetSize, timeEachStep)
                    .SetEase(Ease.OutBack).SetDelay(delay)
                    .OnComplete(() => onComplete?.Invoke());
            }
            
            var starImages = Locator.Game.ScreenInGame.GetGameplayTimer().GetListStarImages();
            
            //Sequence sequence = DOTween.Sequence().SetId(m_tween_ID);
            float timeInterval = 0.05f;
            float timeDelay = startWaitTime + timeDelayEachStep;
            //sequence.AppendInterval(startWaitTime);
            // sequence.AppendCallback(() =>
            // {
            //     DoMoveStarAnim(starLeft.transform, starImages[2].transform, timeDelayEachStep, ()=>EventController.RaiseEvent(EventList.Audio.GET_STAR_1));
            // });
            DoMoveStarAnim(starLeft.transform, starImages[2].transform, timeDelay, ()=>EventController.RaiseEvent(EventList.Audio.GET_STAR_1));
            timeDelay += timeInterval + timeEachStep + timeDelayEachStep;
            //sequence.Append(starLeft.transform.DOScale(0, timeEachStep).SetEase(Ease.OutBounce).From());
            //EventController.RaiseEvent(EventList.Audio.GET_STAR_1);

            if (Pool.Get<InGamePlayGroup>().currentLevelType == LevelType.COIN)
            {
                //Override 3 star in CoinLevel
                remainingTimeState = GameplayTimer.RemainingTimeState.Safe;
            }
            switch (remainingTimeState)
            {
                case GameplayTimer.RemainingTimeState.Safe:
                    //sequence.AppendInterval(timeInterval + timeEachStep + timeDelayEachStep);
                    // sequence.AppendCallback(() =>
                    // {
                    //     DoMoveStarAnim(starMid.transform, starImages[1].transform, timeDelay,()=>EventController.RaiseEvent(EventList.Audio.GET_STAR_2));
                    // });
                    DoMoveStarAnim(starMid.transform, starImages[1].transform, timeDelay,()=>EventController.RaiseEvent(EventList.Audio.GET_STAR_2));
                    timeDelay += timeInterval + timeEachStep + timeDelayEachStep;
                    // sequence.Append(starMid.transform.DOScale(0, timeEachStep).SetEase(Ease.OutBounce).From().OnComplete(() =>
                    //     EventController.RaiseEvent(EventList.Audio.GET_STAR_2)
                    // ));
                    //sequence.AppendInterval(timeInterval + timeEachStep + timeDelayEachStep);
                    // sequence.AppendCallback(() =>
                    // {
                    //     DoMoveStarAnim(starRight.transform, starImages[0].transform, timeDelay,()=>EventController.RaiseEvent(EventList.Audio.GET_STAR_3));
                    // });
                    DoMoveStarAnim(starRight.transform, starImages[0].transform, timeDelay,()=>EventController.RaiseEvent(EventList.Audio.GET_STAR_3));
                    // sequence.Append(starRight.transform.DOScale(0, timeEachStep).SetEase(Ease.OutBounce).From().OnComplete(() =>
                    //     EventController.RaiseEvent(EventList.Audio.GET_STAR_3)
                    // ));
                    StarWon = 3;
                    break;
                case GameplayTimer.RemainingTimeState.Warning:
                    //sequence.AppendInterval(timeInterval + timeEachStep + timeDelayEachStep);
                    // sequence.AppendCallback(() =>
                    // {
                    //     DoMoveStarAnim(starMid.transform, starImages[1].transform, timeDelay,()=>EventController.RaiseEvent(EventList.Audio.GET_STAR_2));
                    // });
                    DoMoveStarAnim(starMid.transform, starImages[1].transform, timeDelay,()=>EventController.RaiseEvent(EventList.Audio.GET_STAR_2));
                    // sequence.Append(starMid.transform.DOScale(0, timeEachStep).SetEase(Ease.OutBounce).From().OnComplete(() =>
                    //     EventController.RaiseEvent(EventList.Audio.GET_STAR_2)
                    // ));
                    StarWon = 2;
                    break;
                case GameplayTimer.RemainingTimeState.Dangerous:
                    StarWon = 1;
                    break;
            }
        }

        private void OnDestroy()
        {
            DOTween.Kill(m_tween_ID);
        }
    }
}