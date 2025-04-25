using System;
using System.Collections;
using _IEC.Features.Booster;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay.ObserverEventData;
using DG.Tweening;
using IEC;
using IEC.TrackingEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Features.Gameplay
{
    public class GameplayTimer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtCoolDown;
        [SerializeField] private Image imgProgress;
        [SerializeField] private Sprite spriteSafe;
        [SerializeField] private Sprite spriteWarning;
        [SerializeField] private Sprite spriteDangerous;
        [SerializeField] private Image frozenMark;
        [SerializeField] private Image frozenMark2;
        [SerializeField] private TextMeshProUGUI txtAddedTime;
        protected static InGamePlayGroup GamePlay => Pool.Get<InGamePlayGroup>();
        [SerializeField]private float m_CurrentTime;
        private float m_MaxTime;
        private State m_CurrentState;
        private RemainingTimeState m_CurrentRemainingTimeState;
        private float m_FrozenTimeCoolDown;
        private bool m_IsGameStarted;
        private bool m_isTimeShowScreenVignette;
        public FloatingJoystick Joystick;
        private string TweenIdMelting => "melting" + gameObject.GetInstanceID();
        private LevelController _levelController => (LevelController)GamePlay.GetLevelController();
        public event Action OnReset;
        public event Action<float> OnTimeUpdate;
        public event Action<bool> OnUpdateFrozenState; 
        
        public RemainingTimeState CurrentRemainingTimeState => m_CurrentRemainingTimeState;
        private void Awake()
        {
            this.RegisterListener(EventList.GamePlay.USER_USE_HELPER,OnUserUseHelper);
            this.RegisterListener(EventList.GamePlay.PAUSE_GAME,OnPauseGame);
            this.RegisterListener(EventList.GamePlay.RESUME_GAME,OnResumeGame);
            this.RegisterListener(EventList.GamePlay.COMPLETE_HELPER,OnCompleteHelper);
            this.RegisterListener(EventList.GamePlay.ADD_TIME_FOR_TIMER,OnAddTimeByLoseScreen);
            this.RegisterListener(EventList.GamePlay.USE_BOOSTER,OnUseBooster);
            this.RegisterListener(EventList.GamePlay.ON_BOOSTER_COLLECTED, OnCollectedBooster);
        }

        private void SetActiveFrozenMark(bool isActive)
        {
            DOTween.Kill(TweenIdMelting);
            frozenMark.color = Color.white;
            frozenMark2.color = Color.white;
            SetActiveMarks(isActive);

            frozenMark2.rectTransform.anchoredPosition = Vector2.zero;
            frozenMark2.rectTransform.sizeDelta = Vector2.zero;   
        }

        private void SetActiveMarks(bool isActive)
        {
            frozenMark.gameObject.SetActive(isActive);
            frozenMark2.gameObject.SetActive(isActive);
        }
        
        private void OnPauseGame(object param)
        {
            ChangeState(State.Inactive);
        }

        private void OnResumeGame(object param)
        {
            ChangeState(State.Active);
        }

        private void OnAddTimeByLoseScreen(object param)
        {
            
        }
        private void OnUserUseHelper(object param)
        {
        }

        private void OnUseBooster(object param)
        {
           
        }

        private void OnCollectedBooster(object param)
        {
            // var data = (MagicPouchBooster)param;
            // if (data.type != MagicPouchBoosterType.Chrono) return;
            // PlayAddedTimeAnimation(Mathf.FloorToInt(data.effectValue));
            // //SoundController.PlaySound(new AudioInfo(Config.soundConfig.littleChrono));
        }
        
       

        private void StopFreezeTime()
        {
            PlayMeltingAnimation();
            ResetFreezeTime();
        }

        public void ResetFreezeTime()
        {
            m_FrozenTimeCoolDown = 0;
            OnUpdateFrozenState?.Invoke(false);
        }

        private void OnCompleteHelper(object pram)
        {
           
        }

        public float GetRemainingTime()
        {
            return m_CurrentTime;
        }

        public float GetMaxTime()
        {
            return m_MaxTime;
        }
        
        private void Update()
        {
            if (Joystick.Direction.magnitude > 0)
            {
                StartTimer();
            }

            if (!m_IsGameStarted) return;
            if (_levelController.IsInResultState()) return;
            
            switch (m_CurrentState)
            {
                case State.Active:
#if UNITY_EDITOR
                    if (Input.GetKeyDown(KeyCode.T))
                    {
                        m_CurrentTime = 5;
                    }
#endif
                    if (m_FrozenTimeCoolDown > 0)
                    {
                        m_FrozenTimeCoolDown -= Time.deltaTime;
                        if (m_FrozenTimeCoolDown <= 0)
                        {
                            StopFreezeTime();
                        }
                    }
                    else
                    {
                        m_CurrentTime -= Time.deltaTime;
                        OnTimeUpdate?.Invoke(m_CurrentTime);
                        if (m_CurrentTime < 0)
                        {
                            m_CurrentTime = 0;
                            this.RaiseEvent(EventList.GamePlay.TIMER_TIME_OUT);
                            ChangeState(State.Inactive);
                        }
                        
                        UpdateCoolDown();
                    }
                    break;
                case State.Inactive:
                    break;
            }
        }

        public void StartTimer()
        {
            if (m_IsGameStarted) return;
            m_IsGameStarted = true;
            ChangeState(State.Active);
        }

        public void SetStartTime(float time)
        {
            m_IsGameStarted = false;
            m_MaxTime = time;
            m_CurrentTime = m_MaxTime;
            UpdateCoolDown();
            Reset();
            m_CurrentState = State.None;
            m_CurrentRemainingTimeState = RemainingTimeState.None;
            ChangeRemainingTimeState(RemainingTimeState.Safe);
        }

        public void CheatTime(float time)
        {
            m_CurrentTime = time;
        }

        public void Pause()
        {
            ChangeState(State.Inactive);
        }

        public void Play()
        {
            if(m_CurrentTime > 0)
                ChangeState(State.Active);
        }

        private void AddMoreTime(float extraTime, bool isAddedToOriginalLevelTime)
        {
            if (isAddedToOriginalLevelTime)
            {
                m_MaxTime += extraTime;
            }
            m_CurrentTime += extraTime;
            UpdateCoolDown();
        }
        
        private void PlayAddedTimeAnimation(int second, bool isAddedToOriginalLevelTime = false)
        {
            if (!gameObject.activeInHierarchy) return;
            
            txtAddedTime.text = $"+{second}";
            StartCoroutine(SetPositionNextFrame(second, isAddedToOriginalLevelTime));
        }
        
        private IEnumerator SetPositionNextFrame(float second, bool isAddedToOriginalLevelTime)
        {
            yield return null;
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                txtAddedTime.transform.parent as RectTransform, 
                screenCenter, 
                GamePlay.uiCamera, 
                out Vector2 localPoint
            );
    
            txtAddedTime.transform.localPosition = localPoint;
            txtAddedTime.gameObject.SetActive(true);
    
            DOTween.Sequence().SetId(txtAddedTime.gameObject.GetInstanceID())
                .AppendInterval(0.5f)
                .Append(txtAddedTime.transform.DOMove(txtCoolDown.transform.position, 0.65f))
                .AppendCallback(() =>
                {
                    AddMoreTime(second, isAddedToOriginalLevelTime);
                    txtAddedTime.gameObject.SetActive(false);
                });
        }
        
        private void UpdateCoolDown()
        {
            txtCoolDown.text = Common.Helper.StringHelper.ConvertTimeInSecondToString(m_CurrentTime);
            ChangeRemainingTimeState(PlayableController.Instance.gamePlayConfig.GetRemainingTimeState(m_CurrentTime, m_MaxTime));
            var proportion = m_CurrentTime / m_MaxTime;
            imgProgress.fillAmount = proportion;
        }

        public State GetCurrentState()
        {
            return m_CurrentState;
        }

        public void Reset()
        {
            SetActiveFrozenMark(false);
            m_FrozenTimeCoolDown = 0;
            OnReset?.Invoke();
        }

        private void PlayMeltingAnimation()
        {
            // var zeroAlphaColor = Color.white;
            // zeroAlphaColor.a = 0;
            // DOTween.Sequence().SetId(TweenIdMelting)
            //     .Append(frozenMark.DOColor(zeroAlphaColor, 0.5f))
            //     .Join(frozenMark2.DOColor(zeroAlphaColor, 0.5f))
            //     .AppendCallback(() =>
            //     {
            //         SetActiveFrozenMark(false);
            //     });
            StartMeltSequence();
        }

        #region Luna

        private void StartMeltSequence()
        {
            StartCoroutine(MeltSequence());
        }

        private IEnumerator MeltSequence()
        {
            var zeroAlphaColor = Color.white;
            zeroAlphaColor.a = 0;
            var fadeDuration = 0.5f;
            // Bắt đầu 2 fade song song
            Coroutine fade1 = StartCoroutine(FadeImageColor(frozenMark, zeroAlphaColor, fadeDuration));
            Coroutine fade2 = StartCoroutine(FadeImageColor(frozenMark2, zeroAlphaColor, fadeDuration));

            // Đợi cả hai xong
            yield return fade1;
            yield return fade2;

            // Callback giống AppendCallback
            SetActiveFrozenMark(false);
        }

        private IEnumerator FadeImageColor(Image image, Color targetColor, float duration)
        {
            Color startColor = image.color;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                image.color = Color.Lerp(startColor, targetColor, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            image.color = targetColor;
        }

        #endregion
        public void ChangeState(State newState)
        {
            if (newState == m_CurrentState) return;
            // exit current state
            switch (m_CurrentState)
            {
                case State.Active:
                    break;
                case State.Inactive:
                    break;
            }

            m_CurrentState = newState;
            //enter new state
            switch (m_CurrentState)
            {
                case State.Active:
                    break;
                case State.Inactive:
                    break;
            }
        }

        private void ChangeRemainingTimeState(RemainingTimeState newState)
        {
            if (newState == m_CurrentRemainingTimeState) return;
            txtCoolDown.color = PlayableController.Instance.gamePlayConfig.GetTimeColorByRemainingTimeState(newState);
            switch (newState)
            {
                case RemainingTimeState.Safe:
                    imgProgress.sprite = spriteSafe;
                    break;
                case RemainingTimeState.Warning:
                    imgProgress.sprite = spriteWarning;
                    break;
                case RemainingTimeState.Dangerous:
                    imgProgress.sprite = spriteDangerous;
                    break;
            }
            m_CurrentRemainingTimeState = newState;
        }
        public enum State
        {
            None,
            Active,
            Inactive
        }

        public enum RemainingTimeState
        {
            None,
            Safe,
            Warning,
            Dangerous
        }
    }
}