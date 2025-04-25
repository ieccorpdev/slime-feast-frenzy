using System;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay.ObserverEventData;
using _IECModules;
using IEC;
using IEC.TrackingEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Features.Gameplay
{
    public class HelperButton : _BaseBehaviour
    {
        [SerializeField] private GameObject groupLock;
        [SerializeField] private GameObject groupUnlocked;
        [SerializeField] private GameObject groupPool;
        [SerializeField] private GameObject groupAdd;
        [SerializeField] private GameObject groupPlaying;
        [SerializeField] private TextMeshProUGUI txtNumber;
        [SerializeField] private Button button;
        [SerializeField] private HelperId helperId;
        [SerializeField] private Image imgProgress;
        [SerializeField] private TextMeshProUGUI txtUnlockCondition;
        [SerializeField] private GameObject groupHint;
        private State m_CurrentState;
        private float m_CooldownTime;
        private float HelperActiveTime => PlayConfig.gamePlayConfig.GetHelperActiveTime(helperId);
        private bool m_IsGamePause;
        private PopupHelper m_MyPopup;

        private Dictionary<HelperId, string> _paramTrackingById = new Dictionary<HelperId, string>()
        {
            { HelperId.ScaleUp, "bigger_hole" },
            { HelperId.Magnet, "magnet" },
            { HelperId.Compass, "compass" },
            { HelperId.FrozenTime, "time_freeze" },
        };
        
        public event Action OnClick;
        private void Awake()
        {
            button.onClick.AddListener(() =>
            {
                OnClick?.Invoke();
                switch (m_CurrentState)
                {
                    case State.None:
                    case State.Locked:
                    case State.Playing:
                        return;
                    case State.Pool:
                        OnClickInPoolState();
                        break;
                    case State.Addable:
                        OnClickInAddableState();
                        break;
                }
            });
            groupHint.gameObject.SetActive(false);
            this.RegisterListener(EventList.GamePlay.PAUSE_GAME,OnPauseGame);
            this.RegisterListener(EventList.GamePlay.RESUME_GAME,OnResumeGame);
            Observer.AddObserver(EventList.GamePlay.UPDATE_HELPER_UI, UpdateHelperUI);
            
        }

        private void Start()
        {
           // AddCheatCode();
        }

        // void AddCheatCode()
        // {
        //     ModuleController.cheatCode.AddCheatCodeButtonWithInput($"Add power item {helperId}", cheatCodeParam =>
        //     {
        //         int number = int.Parse(cheatCodeParam.inputParams[0].value.ToString());
        //         PlayerData.helperData.CollectHelper(helperId,number);
        //         UpdatePoolNumberUI();
        //     }, new CheatCode.InputParam
        //     {
        //         value = 10
        //     });
        //     
        //     ModuleController.cheatCode.AddCheatCodeButtonWithInput($"Set power item {helperId}", cheatCodeParam =>
        //     {
        //         int number = int.Parse(cheatCodeParam.inputParams[0].value.ToString());
        //         PlayerData.helperData.SetHelperValue(helperId,number);
        //         UpdatePoolNumberUI();
        //     }, new CheatCode.InputParam
        //     {
        //         value = 10
        //     });
        // }
        
        private void OnDisable()
        {
            //if (SoundController.dictSoundActiveByHelperId.ContainsKey(helperId))
            //{
            //    var audioInfo = SoundController.dictSoundActiveByHelperId[helperId];
            //    SoundController.StopSound(audioInfo);
            //}
        }

        public void SetPopup(PopupHelper myPopup)
        {
            m_MyPopup = myPopup;
        }

        public void ShowHint()
        {
            groupHint.gameObject.SetActive(true);
        }

        public void HideHint()
        {
            groupHint.gameObject.SetActive(false);
        }
        public void OnShow()
        {
            m_IsGamePause = false;
            m_CooldownTime = HelperActiveTime;
            imgProgress.fillAmount = 1;
            //UpdatePoolNumberUI();
            if (PlayConfig.gamePlayConfig.IsHelperUnlocked(helperId) == false)
            {
                ChangeState(State.Locked);
            }
            else
            {
                if (PlayerData.helperData.GetPool(helperId) > 0)
                {
                    ChangeState(State.Pool);
                }else
                {
                    ChangeState(State.Addable);
                }
            }
        }
        
        private void OnPauseGame(object param)
        {
            m_IsGamePause = true;
            //if (SoundController.dictSoundActiveByHelperId.ContainsKey(helperId))
            //{
            //    var audioInfo = SoundController.dictSoundActiveByHelperId[helperId];
            //    SoundController.PauseSound(audioInfo);
            //}
        }

        private void OnResumeGame(object param)
        {
            m_IsGamePause = false;
            //if (SoundController.dictSoundActiveByHelperId.ContainsKey(helperId))
            //{
            //    var audioInfo = SoundController.dictSoundActiveByHelperId[helperId];
            //    SoundController.UnPauseSound(audioInfo);
            //}
        }

        
        private void OnClickInAddableState()
        {
            //SoundController.PlayClickBtnAudio();
            m_MyPopup.Show(result =>
            {
                switch (result)
                {
                    case PopupHelper.Result.Cancel:
                        break;
                    case PopupHelper.Result.Buy:
                        if (PlayerData.helperData.GetPool(helperId) > 0)
                        {
                            UpdatePoolNumberUI();
                            ChangeState(State.Pool);
                        }
                        break;
                }
            });
        }

        private void OnClickInPoolState()
        {
            this.RaiseEvent(EventList.GamePlay.USER_USE_HELPER, new UserUseHelper()
            {
                helperId = helperId
            });
            PlayerData.helperData.ConsumeHelper(helperId);
            if (PlayerData.helperData.GetPool(helperId) == 0)
            {
                ChangeState(State.Addable);
            }
            //Services.PushEvent(new UseItem(_paramTrackingById[helperId]));
            ChangeState(State.Playing);
            //PlaySoundHelper(SoundController.dictSoundStartByHelperId, helperId);
            //PlaySoundHelper(SoundController.dictSoundActiveByHelperId, helperId);
            
            Locator.Game.ScreenInGame.GetGameplayTimer().StartTimer();
        }

        void PlaySoundHelper(Dictionary<HelperId, AudioInfo> dict, HelperId id)
        {
            if (dict.ContainsKey(id))
            {
                var audioInfo = dict[helperId];
                //SoundController.PlaySound(audioInfo);
            }
        }
        
        private void Update()
        {
            switch (m_CurrentState)
            {
                case State.None:
                case State.Locked:
                case State.Pool:
                case State.Addable:
                    return;
                case State.Playing:
                    if (m_IsGamePause) return;
                    m_CooldownTime -= Time.deltaTime;
                    imgProgress.fillAmount = m_CooldownTime / HelperActiveTime;
                    if (m_CooldownTime <= 0)
                    {
                        if (PlayerData.helperData.GetPool(helperId) > 0 )
                        {
                            ChangeState(State.Pool);
                        }
                        else
                        {
                            ChangeState(State.Addable);
                        }

                        //if (SoundController.dictSoundActiveByHelperId.ContainsKey(helperId))
                        //{
                        //    var audioInfo = SoundController.dictSoundActiveByHelperId[helperId];
                        //    SoundController.StopSound(audioInfo);
                        //}
                        //PlaySoundHelper(SoundController.dictSoundEndByHelperId, helperId);
                    }
                    break;
            }
        }
        
        private void ChangeState(State newState)
        {
            if (newState == m_CurrentState) return;
            m_CurrentState = newState;
            groupLock.SetActive(newState == State.Locked);
            groupUnlocked.SetActive(newState != State.Locked);
            groupAdd.SetActive(newState!= State.Pool);
            groupPool.SetActive(newState!= State.Addable);
            groupPlaying.SetActive(newState == State.Playing);
            UpdatePoolNumberUI();
            switch (newState)
            {
                case State.Playing:
                    m_CooldownTime = HelperActiveTime;
                    imgProgress.fillAmount = 1;
                    if (PlayerData.helperData.GetPool(helperId) > 0)
                    {
                        groupPool.SetActive(true);
                        groupAdd.SetActive(false);
                    }
                    else
                    {
                        groupAdd.SetActive(true);
                        groupPool.SetActive(false);
                    }
                    break;
                case State.Locked:
                    txtUnlockCondition.text = PlayConfig.gamePlayConfig.GetUnlockCondition(helperId);
                    break;
                
            }
        }

        private void UpdatePoolNumberUI()
        {
            txtNumber.text = PlayerData.helperData.GetPool(helperId).ToString();
        }

        private void UpdateHelperUI()
        {
            UpdatePoolNumberUI();
            if (!gameObject.activeInHierarchy) return;
            if (!PlayConfig.gamePlayConfig.IsHelperUnlocked(helperId)) return;
            if (PlayerData.helperData.GetPool(helperId) > 0)
                ChangeState(State.Pool);
        }
        
        private enum  State
        {
            None,
            Locked,
            Pool,
            Addable,
            Playing
        }
    }
}