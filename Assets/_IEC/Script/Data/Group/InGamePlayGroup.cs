using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using _IEC.Features.Gameplay;
using UnityEngine;
using UnityEngine.Serialization;

namespace IEC
{
    public class InGamePlayGroup : _BaseBehaviour, IGroup
    {
        public ScreenUI GameState { get; private set; }

        public Camera mainCamera;
        public Camera uiCamera;
        public LevelController levelController;
        public bool IsWin => GameState == Locator.Game.ScreenLevelWin || GameState == Locator.Game.ScreenLevelWinHard;
        
        public bool IsInDebugAllItemsLevel { get; private set; }
        
        public bool IsInLevelHard { get; private set; } = false;
        private ILevelController m_CurrentLevelController;
        public LevelType currentLevelType => GetLevelController().GetLevelData().levelType;
        
        public int currentThemeSelect = -1;
        public WeatherTheme currentWeatherSelect = WeatherTheme.NONE;
        public void Awake()
        {

        }
        public ILevelController GetLevelController()
        {
            return levelController;
        }

        public void Start()
        {
            currentThemeSelect = -1;
            GamePlay.Init();
        }

        public void Init()
        {
            
        }

        public void LocateGroup()
        {
            Pool.Set(this);
        }

        public void SetGameState(ScreenUI screen)
        {
            GameState = screen;
        }

        public void SetNewLevel(int level)
        {
            ClearCurrentLevel();
            m_CurrentLevelController = GetLevelController();
            m_CurrentLevelController.LoadLevel(level);
            
            CheckLevelForcedTrial();

            IsInDebugAllItemsLevel = false;
        }

        public void SetLevelDebugAllItems()
        {
            ClearCurrentLevel();
            m_CurrentLevelController = GetLevelController();
            m_CurrentLevelController.LoadLevelDebugAllItems();

            IsInDebugAllItemsLevel = true;
        }
        
        public LevelType GetCurrentLevelType()
        {
            return m_CurrentLevelController.GetLevelData().levelType;
        }

        private void CheckLevelForcedTrial()
        {
            var data = Config.characterRecommendConfig.GetForcedTrialCharacter(PlayerData.level);

            if (data == null)
                return;
            
            TweenControl.GetInstance().DelayCall(Locator.Main.PopupUnlockCharacter.gameObject.transform, 0.5f, () =>
            {
                Locator.Main.PopupUnlockCharacter.Setup(Config.allCharactersConfig.GetCharacterConfig(data.characterName), "Help has arrived!", "Tap to Continue");
                Locator.Main.PopupUnlockCharacter.Show();
            });
        }

        public void SetFinishPlay()
        {
            EventController.RaiseEvent(EventList.GamePlay.LEVEL_WIN);
        }

        public void StartGamePlay()
        {
            GetLevelController().StartPlay();
        }
        public void SetNewLevelHard()
        {

        }
        public void ClearCurrentLevel()
        {
            if (m_CurrentLevelController != null)
                m_CurrentLevelController.Dispose();
        }
        public void SetWin()
        {
            EventController.RaiseEvent(EventList.GamePlay.LEVEL_WIN);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }
    }
}