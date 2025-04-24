using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay;
using _IEC.Features.Gameplay.ObserverEventData;
using _IEC.Features.Tutorial;
using UnityEngine.SocialPlatforms;

namespace IEC
{
    public class ScreenInGame : ScreenUI
    {
        public override bool IS_PAUSE_GAME => false;

        public TextMeshProUGUI txtLevel;

        [Header("Group objective")] 
        [SerializeField] private RectTransform holderImage;
        [SerializeField] private RectTransform groupLevelObjectiveLayoutGroup;

        [Space]
        //public Button btnRestart;
        [SerializeField]
        private GameplayTimer gameplayTimer;
        public Button btnSetting;
        [SerializeField] private GroupLevelObjectives groupLevelObjectives;
        [SerializeField] private GroupHelperUI groupHelperUI;
        [SerializeField] private Button btnPause;
        //[SerializeField] private PopupPauseGame popupPauseGame;
        [SerializeField] private GroupScreenInGameMark groupMark;
        [SerializeField] private CompassDirectorControllerUI compassDirectorControllerUI;
        [SerializeField] private TutorialStartGame tutorialStartGame;
        [SerializeField] private BaseHelperTutorial tutorialMagnet;
        [SerializeField] private BaseHelperTutorial tutorialCompass;
        [SerializeField] private BaseHelperTutorial tutorialFrozen;
        [SerializeField] private BaseHelperTutorial tutorialScaleUp;
        [SerializeField] private GameObject timerFrozenMark;
        
        private void Awake()
        {
            // btnPause.onClick.AddListener(() =>
            // {
            //     if(GamePlay.levelController.CurrentState == LevelController.State.Lose) return;
            //     Locator.Game.PopupSettingIngame.Show();
            //     //SoundController.PlayClickBtnAudio();
            //     //popupPauseGame.Show();
            // });
            gameplayTimer.OnReset += () =>
            {
                groupMark.SetActiveDangerousMark(false);
            };
            gameplayTimer.OnTimeUpdate += remainingTime => { groupMark.SetActiveDangerousMark(remainingTime <= 10); };
            gameplayTimer.OnUpdateFrozenState += isFrozen => { groupMark.SetActiveFrozenMark(isFrozen); };
        }
        
        public override void Show()
        {
            gameplayTimer.ResetFreezeTime();
            //popupPauseGame.gameObject.SetActive(false);
            //Services.SetActiveBottomBanner(true);
            base.Show();
        }

        public void StartTutorialHelper(HelperId helperId)
        {
            tutorialCompass.gameObject.SetActive(helperId == HelperId.Compass);
            tutorialMagnet.gameObject.SetActive(helperId == HelperId.Magnet);
            tutorialFrozen.gameObject.SetActive(helperId == HelperId.FrozenTime);
            tutorialScaleUp.gameObject.SetActive(helperId == HelperId.ScaleUp);
            switch (helperId)
            {
                case HelperId.Magnet:
                    tutorialMagnet.StartTutorial();
                    break;
                case HelperId.FrozenTime:
                    tutorialFrozen.StartTutorial();
                    break;
                case HelperId.ScaleUp:
                    tutorialScaleUp.StartTutorial();
                    break;
                case HelperId.Compass:
                    tutorialCompass.StartTutorial();
                    break;
            }
        }

        public GameplayTimer GetGameplayTimer()
        {
            return gameplayTimer;
        }

        public void UpdateUI()
        {
//            groupHelperUI.Show();
            //popupPauseGame.Hide();
            groupMark.SetActiveFrozenMark(false);
            groupMark.SetActiveDangerousMark(false);
            timerFrozenMark.SetActive(false);
        }
        
        private void OnEnable()
        {
            this.RegisterListener(EventList.GamePlay.COLLECT_ITEM, OnCollectItem);
            this.RegisterListener(EventList.GamePlay.COLLECT_OBJECTIVE_ITEM_NEEDED_FOR_UI, OnCollectObjectiveItem);
            this.RegisterListener(EventList.GamePlay.USER_USE_HELPER, OnUserUseHelper);
            this.RegisterListener(EventList.GamePlay.COMPLETE_HELPER, OnUserCompleteHelper);
            this.RegisterListener(EventList.GamePlay.COLLECT_SPECIAL_ITEM_FOR_UI, OnCollectSpecialItem);
        }

        private void OnDisable()
        {
            this.UnregisterListener(EventList.GamePlay.COLLECT_ITEM, OnCollectItem);
            this.UnregisterListener(EventList.GamePlay.COLLECT_OBJECTIVE_ITEM_NEEDED_FOR_UI, OnCollectObjectiveItem);
            this.UnregisterListener(EventList.GamePlay.USER_USE_HELPER, OnUserUseHelper);
            this.UnregisterListener(EventList.GamePlay.COMPLETE_HELPER, OnUserCompleteHelper);
            this.UnregisterListener(EventList.GamePlay.COLLECT_SPECIAL_ITEM_FOR_UI, OnCollectSpecialItem);

        }

        private void OnCollectSpecialItem(object param)
        {
            CollectItemData data = (CollectItemData)param;
            if (data.itemID.ToLower().Contains("coin"))
            {
                data.itemID = LevelAchievementData.ID_COIN;
            }
            var totalNumberCollected = GamePlay.levelController.SpecialProgressRewardObjCollected;
            groupLevelObjectives.CollectASpecialObject(data.itemID, totalNumberCollected, Vector2.zero);
        }

        private void OnCollectItem(object param)
        {
            var data = (CollectItemData)param;
            SpawnXpVFX(data.xp, data.position);
        }

        private void OnCollectObjectiveItem(object param)
        {
            var data = (CollectItemData)param;
            groupLevelObjectives.CollectAObject(data.itemID, Vector2.zero);
        }

        private void OnUserUseHelper(object param)
        {
            var data = (UserUseHelper)param;
            if (data.helperId == HelperId.FrozenTime)
            {
                groupMark.SetActiveFrozenMark(true);
            }
        }

        private void OnUserCompleteHelper(object param)
        {
            var data = (UserCompleteHelper)param;
            if (data.helperId == HelperId.FrozenTime)
            {
                groupMark.SetActiveFrozenMark(false);
            }
        }

        public void DeActiveStartGameTutorial()
        {
            tutorialStartGame.gameObject.SetActive(false);
        }

        public void OnStartTutorial(Transform holeTransform)
        {
            tutorialStartGame.gameObject.SetActive(true);
            tutorialStartGame.StartTutorial(holeTransform);
        }

        private void SpawnXpVFX(int xp, Vector3 position)
        {
            if (xp == 0) return;
            if (GamePlay.GetLevelController().GetLevelData().levelType == LevelType.COIN) return;
            var xpVFX = Config.prefabConfig.xpVFX.Spawn(transform);
            xpVFX.Reset();
            var localPoint = GetLocalAnchorPos(position);
            xpVFX.SetAnchorPosition(localPoint);
            xpVFX.Show(xp);
        }

        private Vector2 GetLocalAnchorPos(Vector3 position)
        {
            return position.GetLocalAnchorPos(transform as RectTransform, GamePlay.mainCamera, GamePlay.uiCamera);
        }

        public void LoadLevelData(LevelData levelData)
        {
            groupLevelObjectives.DisposeAll();
            groupLevelObjectives.LoadObjectiveData(levelData.levelObjectives, levelData.levelType);
            groupLevelObjectives.LoadObjectiveData(levelData.levelObjectives, levelData.levelType);
            //Joystick.OnPointerUp(null);
            UpdateUI();
            gameplayTimer.SetStartTime(levelData.time);
        }
    }
}
