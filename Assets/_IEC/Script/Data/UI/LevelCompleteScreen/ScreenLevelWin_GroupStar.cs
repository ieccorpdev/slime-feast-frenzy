using System;
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

        public int StarWon { get; private set; }
        float startWaitTime = 0.3f;
        float timeEachStep = 0.25f;

        private void OnEnable()
        {
            DeactiveGroupStar();
            SoundController.StopSound(EventList.Audio.MAGNET_IDLE);
            SoundController.StopSound(EventList.Audio.COMPASS);
            SoundController.StopSound(EventList.Audio.ICE_IDLE);
        }

        private void DeactiveGroupStar()
        {
            starLeft.gameObject.SetActive(false);
            starMid.gameObject.SetActive(false);
            starRight.gameObject.SetActive(false);
        }

        public void DoShowStarEffect(GameplayTimer.RemainingTimeState remainingTimeState)
        {
            Sequence sequence = DOTween.Sequence().SetId(m_tween_ID);
            float timeInterval = 0.05f;
            sequence.AppendInterval(startWaitTime);
            sequence.AppendCallback(() => starLeft.gameObject.SetActive(true));
            sequence.Append(starLeft.transform.DOScale(0, timeEachStep).SetEase(Ease.OutBounce).From());
            EventController.RaiseEvent(EventList.Audio.GET_STAR_1);

            if (Pool.Get<InGamePlayGroup>().currentLevelType == LevelType.COIN)
            {
                //Override 3 star in CoinLevel
                remainingTimeState = GameplayTimer.RemainingTimeState.Safe;
            }
            switch (remainingTimeState)
            {
                case GameplayTimer.RemainingTimeState.Safe:
                    sequence.AppendInterval(timeInterval);
                    sequence.AppendCallback(() => starMid.gameObject.SetActive(true));
                    sequence.Append(starMid.transform.DOScale(0, timeEachStep).SetEase(Ease.OutBounce).From().OnComplete(() =>
                        EventController.RaiseEvent(EventList.Audio.GET_STAR_2)
                    ));
                    sequence.AppendInterval(timeInterval);
                    sequence.AppendCallback(() => starRight.gameObject.SetActive(true));
                    sequence.Append(starRight.transform.DOScale(0, timeEachStep).SetEase(Ease.OutBounce).From().OnComplete(() =>
                        EventController.RaiseEvent(EventList.Audio.GET_STAR_3)
                    ));
                    StarWon = 3;
                    break;
                case GameplayTimer.RemainingTimeState.Warning:
                    sequence.AppendInterval(timeInterval);
                    sequence.AppendCallback(() => starMid.gameObject.SetActive(true));
                    sequence.Append(starMid.transform.DOScale(0, timeEachStep).SetEase(Ease.OutBounce).From().OnComplete(() =>
                        EventController.RaiseEvent(EventList.Audio.GET_STAR_2)
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