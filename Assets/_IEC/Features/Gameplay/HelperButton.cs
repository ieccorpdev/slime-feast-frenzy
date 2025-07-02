using System;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay.ObserverEventData;
using _IECModules;
using DG.Tweening;
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
        [SerializeField] private Image imgIcon;
        [SerializeField] private TextMeshProUGUI txtUnlockCondition;
        [SerializeField] private GameObject groupHint;
        public Action actionEffect;
        public Action completeActionEffect;
        private State m_CurrentState;
        private float m_CooldownTime;
        private float HelperActiveTime => GamePlay.levelController.CheckCharacterPowerOnHelperDuration(HelperType, Config.gamePlayConfig.GetHelperActiveTime(helperId));
        private bool m_IsGamePause;
        private PopupHelper m_MyPopup;
        [SerializeField] private Animator fillAnim;
        private readonly int fillAnimName = Animator.StringToHash("Fill");
        private Dictionary<HelperId, string> _paramTrackingById = new Dictionary<HelperId, string>()
        {
            { HelperId.ScaleUp, "bigger_hole" },
            { HelperId.Magnet, "magnet" },
            { HelperId.Compass, "compass" },
            { HelperId.FrozenTime, "time_freeze" },
        };
        [SerializeField] private Transform lockIcon, groupUnlockDescription;
        private static Transform lastGroupUnlockDescription;
        public event Action OnClick;

        public ResourceType HelperType => helperId switch
        {
            HelperId.Magnet => ResourceType.Vortex,
            HelperId.FrozenTime => ResourceType.TimeStop,
            HelperId.ScaleUp => ResourceType.Enlarge,
            HelperId.Compass => ResourceType.Compass,
        };
        private void Awake()
        {
            button.onClick.AddListener(() =>
            {
                OnClick?.Invoke();
                switch (m_CurrentState)
                {
                    case State.Locked:
                        HandleLockedOnClick();
                        return;
                    case State.None:
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

        

        private void HandleLockedOnClick()
        {
            UpdateLockDescription();
            if (lastGroupUnlockDescription)
            {
                DOTween.Kill(lastGroupUnlockDescription.GetHashCode());
                lastGroupUnlockDescription.DOKill();
                lastGroupUnlockDescription.gameObject.SetActive(false);
            }
            DOTween.Kill(lockIcon.GetHashCode());

            Sequence sequence = DOTween.Sequence().SetId(lockIcon.GetHashCode());

            sequence.Append(lockIcon.DORotate(new Vector3(0, 0, 5), 0.05f).SetEase(Ease.OutQuad))
                    .Append(lockIcon.DORotate(new Vector3(0, 0, -5), 0.1f).SetEase(Ease.InOutQuad))
                    .Append(lockIcon.DORotate(new Vector3(0, 0, 2.5f), 0.05f).SetEase(Ease.InOutQuad))
                    .Append(lockIcon.DORotate(new Vector3(0, 0, -2.5f), 0.05f).SetEase(Ease.InOutQuad))
                    .Append(lockIcon.DORotate(Vector3.zero, 0.05f).SetEase(Ease.OutBack));

            if (GamePlay.levelController.GetLevelData().levelType != LevelType.COIN)
            {
                groupUnlockDescription.DOKill();
                DOTween.Kill(groupUnlockDescription.GetHashCode());
                groupUnlockDescription.gameObject.SetActive(true);
                groupUnlockDescription.transform.localScale = Vector3.zero;
                groupUnlockDescription.transform.DOScale(Vector3.one, 0.5f)
                    .SetEase(Ease.OutBack);
                lastGroupUnlockDescription = groupUnlockDescription;
                DOVirtual.DelayedCall(1.5f, () =>
                {
                    groupUnlockDescription.transform.DOScale(Vector3.zero, 0.3f)
                        .SetEase(Ease.InBack)
                        .OnComplete(() => groupUnlockDescription.gameObject.SetActive(false));
                }).SetId(groupUnlockDescription.GetHashCode());
            }
        }

        public void UpdateLockDescription()
        {
            if (GamePlay.levelController.GetLevelData().levelType == LevelType.COIN)
            {
                txtUnlockCondition.text ="Level Bonus";
            }
            else
            {
                txtUnlockCondition.text ="Unlocks \nat level "+  Config.gamePlayConfig.GetUnlockCondition(helperId);
            }

        }
        private void Start()
        {
            AddCheatCode();
            Material instanceMaterial = new Material(imgIcon.material);
            imgIcon.material = instanceMaterial;
            txtUnlockCondition.text += Config.gamePlayConfig.GetUnlockCondition(helperId);
        }

        void AddCheatCode()
        {
            ModuleController.cheatCode.AddCheatCodeButtonWithInput($"Add power item {helperId}", cheatCodeParam =>
            {
                int number = int.Parse(cheatCodeParam.inputParams[0].value.ToString());
                PlayerData.helperData.CollectHelper(helperId,number);
                UpdatePoolNumberUI();
            }, new CheatCode.InputParam
            {
                value = 10
            });
            
            ModuleController.cheatCode.AddCheatCodeButtonWithInput($"Set power item {helperId}", cheatCodeParam =>
            {
                int number = int.Parse(cheatCodeParam.inputParams[0].value.ToString());
                PlayerData.helperData.SetHelperValue(helperId,number);
                UpdatePoolNumberUI();
            }, new CheatCode.InputParam
            {
                value = 10
            });
        }
        
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
            groupHint.gameObject.SetActive(false);
        }

        public void HideHint()
        {
            groupHint.gameObject.SetActive(false);
        }
        public void OnShow()
        {
            m_IsGamePause = false;
            m_CooldownTime = HelperActiveTime;
            UpdatePoolNumberUI();
            if (Config.gamePlayConfig.IsHelperUnlocked(helperId) == false || (GamePlay.levelController.GetLevelData().levelType == LevelType.COIN && helperId == HelperId.Compass ))
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

            DOTween.Kill(IconPunchScale);
            imgIcon.transform.localScale = Vector3.one;
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
        protected string IconPunchScale => "Icon_Punch_Scale" + GetInstanceID();
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
            imgIcon.transform.DOKill();
            imgIcon.transform.DOPunchScale(Vector3.one*0.2f, 0.3f,1,0.15f).SetId(IconPunchScale);
            actionEffect?.Invoke();
            Services.PushEvent(new UserHelper(helperId.ToString()));
            switch (helperId)
            {
                case HelperId.Magnet:
                    //Locator.Game.ScreenInGame.ShowHelperIntroAnimation("Magnetic Force");
                    EventController.RaiseEvent(EventList.Audio.MAGNET_START);
                    DOVirtual.DelayedCall(0.5f, () =>
                    {
                        EventController.RaiseEvent(EventList.Audio.MAGNET_IDLE);
                    });
                    break;
                case HelperId.Compass:
                    //Locator.Game.ScreenInGame.ShowHelperIntroAnimation("Guiding Compass");
                    EventController.RaiseEvent(EventList.Audio.COMPASS);
                    break;
                case HelperId.ScaleUp:
                    //Locator.Game.ScreenInGame.ShowHelperIntroAnimation("Scale Up");
                    EventController.RaiseEvent(EventList.Audio.HOLE_LEVELUP);
                    break;
                case HelperId.FrozenTime:
                    //Locator.Game.ScreenInGame.ShowHelperIntroAnimation("Time Freeze");
                    EventController.RaiseEvent(EventList.Audio.ICE_START);
                    DOVirtual.DelayedCall(0.5f, () =>
                    {
                        EventController.RaiseEvent(EventList.Audio.ICE_IDLE);
                    });
                    break;
            }
        }

        public void PlayShinyEffect()
        {
            imgIcon.material.SetFloat("_ShineLocation",0);
            imgIcon.material.DOFloat(1f, "_ShineLocation", 0.3f).SetId(imgIcon.GetInstanceID()+"_Shiny");
            imgIcon.DOKill();
            imgIcon.transform.DOPunchScale(Vector3.one*0.2f, 0.3f,1,0.15f).SetId(IconPunchScale);
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
                    //imgProgress.fillAmount = 1 - m_CooldownTime / HelperActiveTime;
                   
                    fillAnim.Play(fillAnimName, 0, 1 - m_CooldownTime / HelperActiveTime);
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
                        PlayShinyEffect();
                        switch (helperId)
                        {
                            case HelperId.Magnet:
                                SoundController.StopSound(EventList.Audio.MAGNET_IDLE);
                                break;
                            case HelperId.Compass:
                                SoundController.StopSound(EventList.Audio.COMPASS);
                                break;
                            case HelperId.FrozenTime:
                                SoundController.StopSound(EventList.Audio.ICE_IDLE);
                                break;
                        }
                        this.RaiseEvent(EventList.GamePlay.COMPLETE_HELPER, new UserCompleteHelper(helperId));
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
            button.interactable = true;
            groupLock.SetActive(newState == State.Locked);
            groupUnlocked.SetActive(newState != State.Locked);
            groupAdd.SetActive(newState!= State.Pool);
            groupPool.SetActive(newState!= State.Addable);
            groupPlaying.SetActive(newState == State.Playing);
            UpdatePoolNumberUI();
            fillAnim.Play("idle");
            imgIcon.transform.localScale = Vector3.one;
            switch (newState)
            {
                case State.Playing:
                    m_CooldownTime = HelperActiveTime;
                    imgProgress.fillAmount = 1;
                    imgIcon.enabled = false;
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
                default:
                    imgIcon.enabled = true;
                    imgProgress.fillAmount = 0;
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
            if (!Config.gamePlayConfig.IsHelperUnlocked(helperId)) return;
            if (PlayerData.helperData.GetPool(helperId) > 0)
                ChangeState(State.Pool);
        }

        private void OnDestroy()
        {
            DOTween.Kill(imgIcon.GetInstanceID()+"_Shiny");
            DOTween.Kill(IconPunchScale);
            Observer.RemoveObserver(EventList.GamePlay.UPDATE_HELPER_UI, UpdateHelperUI);
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