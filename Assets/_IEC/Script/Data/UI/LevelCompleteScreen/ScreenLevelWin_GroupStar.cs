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
        [SerializeField] private ParticleSystem starLeftExplosion;
        [SerializeField] private Image starMid;
        [SerializeField] private ParticleSystem starMidExplosion;
        [SerializeField] private Image starRight;
        [SerializeField] private ParticleSystem starRightExplosion;
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

            
            Sequence sequence = DOTween.Sequence().SetId(m_tween_ID);
            float timeInterval = 0.05f;
            //float timeDelay = startWaitTime + timeDelayEachStep;
            sequence.AppendInterval(startWaitTime);
            sequence.AppendCallback(() =>
             {
                 starLeft.gameObject.SetActive(true);
                 
                // DoMoveStarAnim(starLeft.transform, starImages[2].transform, timeDelayEachStep, ()=>EventController.RaiseEvent(EventList.Audio.GET_STAR_1));
             });
            //DoMoveStarAnim(starLeft.transform, starImages[2].transform, timeDelay, ()=>EventController.RaiseEvent(EventList.Audio.GET_STAR_1));
            //timeDelay += timeInterval + timeEachStep + timeDelayEachStep;
            sequence.Append(starLeft.transform.DOScale(0, timeEachStep).SetEase(Ease.OutBounce).From().OnComplete(() =>
            {
                starLeftExplosion.Play();
            }));
            EventController.RaiseEvent(EventList.Audio.GET_STAR_1);

            if (Pool.Get<InGamePlayGroup>().currentLevelType == LevelType.COIN)
            {
                //Override 3 star in CoinLevel
                remainingTimeState = GameplayTimer.RemainingTimeState.Safe;
            }
            switch (remainingTimeState)
            {
                case GameplayTimer.RemainingTimeState.Safe:
                    sequence.AppendInterval(timeInterval + timeEachStep);
                     sequence.AppendCallback(() =>
                     {
                         //DoMoveStarAnim(starMid.transform, starImages[1].transform, timeDelay,()=>EventController.RaiseEvent(EventList.Audio.GET_STAR_2));
                         starMid.gameObject.SetActive(true);
                     });
                    // DoMoveStarAnim(starMid.transform, starImages[1].transform, timeDelay,()=>EventController.RaiseEvent(EventList.Audio.GET_STAR_2));
                    // timeDelay += timeInterval + timeEachStep + timeDelayEachStep;
                     sequence.Append(starMid.transform.DOScale(0, timeEachStep).SetEase(Ease.OutBounce).From().OnComplete(
                         () =>
                         {
                             EventController.RaiseEvent(EventList.Audio.GET_STAR_2);
                             starMidExplosion.Play();
                         }
                     ));
                    sequence.AppendInterval(timeInterval + timeEachStep);
                     sequence.AppendCallback(() =>
                     {
                         //DoMoveStarAnim(starRight.transform, starImages[0].transform, timeDelay,()=>EventController.RaiseEvent(EventList.Audio.GET_STAR_3));
                         starRight.gameObject.SetActive(true);
                     });
                    //DoMoveStarAnim(starRight.transform, starImages[0].transform, timeDelay,()=>EventController.RaiseEvent(EventList.Audio.GET_STAR_3));
                    sequence.Append(starRight.transform.DOScale(0, timeEachStep).SetEase(Ease.OutBounce).From().OnComplete(
                        () =>
                        {
                            EventController.RaiseEvent(EventList.Audio.GET_STAR_3);
                            starRightExplosion.Play();
                        }

                    ));
                    StarWon = 3;
                    break;
                case GameplayTimer.RemainingTimeState.Warning:
                    sequence.AppendInterval(timeInterval + timeEachStep + timeDelayEachStep);
                     sequence.AppendCallback(() =>
                     {
                         //DoMoveStarAnim(starMid.transform, starImages[1].transform, timeDelay,()=>EventController.RaiseEvent(EventList.Audio.GET_STAR_2));
                         starMid.gameObject.SetActive(true);
                     });
                    //DoMoveStarAnim(starMid.transform, starImages[1].transform, timeDelay,()=>EventController.RaiseEvent(EventList.Audio.GET_STAR_2));
                    sequence.Append(starMid.transform.DOScale(0, timeEachStep).SetEase(Ease.OutBounce).From().OnComplete(() =>
                    {
                        EventController.RaiseEvent(EventList.Audio.GET_STAR_2);
                        starMidExplosion.Play();
                    }
                    ));
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