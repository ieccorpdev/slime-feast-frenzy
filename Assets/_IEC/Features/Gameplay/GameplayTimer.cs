using System;
using System.Collections;
using System.Collections.Generic;
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
    public class GameplayTimer : _BaseBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtCoolDown;
        [SerializeField] private Image imgProgress;
        [SerializeField] private Sprite spriteSafe;
        [SerializeField] private Sprite spriteWarning;
        [SerializeField] private Sprite spriteDangerous;
        [SerializeField] private Image frozenMark;
        [SerializeField] private Image frozenMark2;
        [SerializeField] private TextMeshProUGUI txtAddedTime;
        [SerializeField] private List<Image> listStarTime;
        
        public float CurrentTime;
        private float timeEffectStar = 0.5f;
        private float m_MaxTime;
        private State m_CurrentState;
        private RemainingTimeState m_CurrentRemainingTimeState;
        private float m_FrozenTimeCoolDown;
        private bool m_IsGameStarted;
        private bool m_isTimeShowScreenVignette;
        private bool m_isLevelCoin;
        public TextMeshProUGUI timeTextTMP => txtCoolDown;
        private string TweenIdMelting => "melting" + gameObject.GetInstanceID();
        private LevelController _levelController => (LevelController)GamePlay.GetLevelController();
        public event Action OnReset;
        public event Action<float> OnTimeUpdate;
        public event Action<bool> OnUpdateFrozenState; 
        
        public RemainingTimeState CurrentRemainingTimeState => m_CurrentRemainingTimeState;

        public bool GameStarted => m_IsGameStarted;
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

        private void OnDestroy()
        {
            this.UnregisterListener(EventList.GamePlay.USER_USE_HELPER,OnUserUseHelper);
            this.UnregisterListener(EventList.GamePlay.PAUSE_GAME,OnPauseGame);
            this.UnregisterListener(EventList.GamePlay.RESUME_GAME,OnResumeGame);
            this.UnregisterListener(EventList.GamePlay.COMPLETE_HELPER,OnCompleteHelper);
            this.UnregisterListener(EventList.GamePlay.ADD_TIME_FOR_TIMER,OnAddTimeByLoseScreen);
            this.UnregisterListener(EventList.GamePlay.USE_BOOSTER,OnUseBooster);
            this.UnregisterListener(EventList.GamePlay.ON_BOOSTER_COLLECTED, OnCollectedBooster);
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
            var data = (AddTimeForTimerData) param;
            AddMoreTime(data.time, false);
            ChangeState(State.Active);
        }
        private void OnUserUseHelper(object param)
        {
            var data = (UserUseHelper) param;
            //if (data.helperId != HelperId.FrozenTime) return;
            //StartFreezeTime();
        }

        private void OnUseBooster(object param)
        {
            var data = (UseBoosterEventData) param;
            if (data.boosterId == BoosterId.BonusTime)
            {
                //Services.PushEvent(new UseItem("time_extend"));
                //var bonusTimeConfig = Config.boosterConfig.GetBoosterConfigModelById(BoosterId.BonusTime);
                //PlayAddedTimeAnimation(bonusTimeConfig.advantageNumber);
                //SoundController.PlaySound(new AudioInfo(Config.soundConfig.boosterChrono));
            }
        }

        private void OnCollectedBooster(object param)
        {
            // var data = (MagicPouchBooster)param;
            // if (data.type != MagicPouchBoosterType.Chrono) return;
            // PlayAddedTimeAnimation(Mathf.FloorToInt(data.effectValue));
            // //SoundController.PlaySound(new AudioInfo(Config.soundConfig.littleChrono));
        }
        
        public void StartFreezeTime()
        {
            m_FrozenTimeCoolDown = GamePlay.levelController.CheckCharacterPowerOnHelperDuration(ResourceType.TimeStop, Config.gamePlayConfig.GetHelperActiveTime(HelperId.FrozenTime));
            SetActiveFrozenMark(true);
            OnUpdateFrozenState?.Invoke(true);
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
            var data = (UserCompleteHelper) pram;
            if (data.helperId != HelperId.FrozenTime) return;
            Play();
        }

        public float GetRemainingTime()
        {
            return CurrentTime;
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
                        CurrentTime = 5;
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
                        CurrentTime -= Time.deltaTime;
                        OnTimeUpdate?.Invoke(CurrentTime);
                        if (CurrentTime < 0)
                        {
                            CurrentTime = 0;
                            if (!GamePlay.IsInDebugAllItemsLevel)
                            {
                                this.RaiseEvent(EventList.GamePlay.TIMER_TIME_OUT);
                                ChangeState(State.Inactive);
                            }
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

        public void SetStartTime(LevelData levelData)
        {
            m_IsGameStarted = false;
            m_MaxTime = levelData.time;
            m_MaxTime += Config.gamePlayConfig.GetTimeLevelExpand(levelData.level);
            CurrentTime = m_MaxTime;
            CheckActiveStar(levelData);
            UpdateCoolDown();
            Reset();
            m_CurrentState = State.None;
            m_CurrentRemainingTimeState = RemainingTimeState.None;
            ChangeRemainingTimeState(RemainingTimeState.Safe);
        }

        public void CheckActiveStar(LevelData levelData)
        {
            m_isLevelCoin = levelData.levelType == LevelType.COIN;
            foreach (var star in listStarTime)
            {
                star.gameObject.SetActive(!m_isLevelCoin);
            }
        }

        public List<Image> GetListStarImages()
        {
            return listStarTime;
        }
        
        public void CheatTime(float time)
        {
            CurrentTime = time;
        }

        public void Pause()
        {
            ChangeState(State.Inactive);
        }

        public void Play()
        {
            if(CurrentTime > 0)
                ChangeState(State.Active);
        }

        private void AddMoreTime(float extraTime, bool isAddedToOriginalLevelTime)
        {
            if (isAddedToOriginalLevelTime)
            {
                m_MaxTime += extraTime;
            }
            CurrentTime += extraTime;
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
            txtCoolDown.text = Common.Helper.StringHelper.ConvertTimeInSecondToStringNoLeadingZero(CurrentTime);
            ChangeRemainingTimeState(Config.gamePlayConfig.GetRemainingTimeState(CurrentTime, m_MaxTime));
            var proportion = CurrentTime / m_MaxTime;
            imgProgress.fillAmount = proportion;
            //UpdateStarProgress();
        }

        public void UpdateStarProgress()
        {
            float fillAmount = imgProgress.fillAmount;
            //0 = 3 star, 1 = 2 star, 2= 1 star
            for (int i = 0; i < listStarTime.Count; i++)
            {
                //Code change position of star if need 
                //float angle = fillAmount * 360f * Mathf.Deg2Rad;
                //Vector3 newPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 200f;
                //listStarTime[i].rectTransform.localPosition = newPosition;
            }

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
            foreach (var star in listStarTime)
            {
                DOTween.Kill((star.transform.GetInstanceID()+"_ANIM_STAR"));
                star.transform.localScale = Vector3.one;
            }
        }

        private void PlayMeltingAnimation()
        {
            var zeroAlphaColor = Color.white;
            zeroAlphaColor.a = 0;
            DOTween.Sequence().SetId(TweenIdMelting)
                .Append(frozenMark.DOColor(zeroAlphaColor, 0.5f))
                .Join(frozenMark2.DOColor(zeroAlphaColor, 0.5f))
                .AppendCallback(() =>
                {
                    SetActiveFrozenMark(false);
                });
        }
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
            txtCoolDown.color = Config.gamePlayConfig.GetTimeColorByRemainingTimeState(newState);
            switch (newState)
            {
                case RemainingTimeState.Safe:

                    imgProgress.sprite = spriteSafe;
                    //txtCoolDown.fontMaterial = safeMaterialPreset;
                    break;
                case RemainingTimeState.Warning:
                    listStarTime[0].transform.DOScale(Vector3.zero, timeEffectStar).SetId(listStarTime[0].transform.GetInstanceID()+"_ANIM_STAR").SetEase(Ease.InBack);
                    imgProgress.sprite = spriteWarning;
                    //txtCoolDown.fontMaterial = warningMaterialPreset;
                    break;
                case RemainingTimeState.Dangerous:
                    listStarTime[1].transform.DOScale(Vector3.zero, timeEffectStar).SetId(listStarTime[1].transform.GetInstanceID()+"_ANIM_STAR").SetEase(Ease.InBack);;
                    imgProgress.sprite = spriteDangerous;
                    //txtCoolDown.fontMaterial = dangerousMaterialPreset;
                    break;
                case RemainingTimeState.None:
                    listStarTime[2].transform.DOScale(Vector3.zero, timeEffectStar).SetId(listStarTime[2].transform.GetInstanceID()+"_ANIM_STAR").SetEase(Ease.InBack);
                    imgProgress.sprite = spriteDangerous;
                    //txtCoolDown.fontMaterial = dangerousMaterialPreset;
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