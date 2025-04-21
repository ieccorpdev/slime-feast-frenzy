using System;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay;
using _IECModules;

namespace IEC
{
    public class MagicPouchController : _BaseBehaviour, IController, IEventListener, IEventStart, IEventPauseApp, IEventQuitApp
    {
        private const string MAGIC_POUCH_WIN_STREAK_SAVED_KEY = "MAGIC_POUCH_WIN_STREAK_SAVED_KEY";
        
        private WinStreakData _winStreakData;

        public WinStreakData WinStreakData => _winStreakData;
        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeInHierarchy;
        }

        public void BehaviourQuitApp()
        {
            if(!Game.isHaveBooster) return;
            SaveDataOnUserQuit(true);
            Data.SaveData();
        }

        public void BehaviourPauseApp(bool isPause)
        {
            if(!Game.isHaveBooster) return;
            SaveDataOnUserQuit(isPause);
            if (isPause)
            {
                Data.SaveData();
            }
        }

        private void SaveDataOnUserQuit(bool isPause)
        {
           // if (_winStreakData == null) return;
           // if (UI.ScreenInGame.IsShow || (UI.ScreenLevelLose.IsShow && !UI.ScreenLevelLose.groupLevelFailed.IsShow))
           // {
           //     _winStreakData.isKilledAppInGameplay = isPause;
           // }
        }
        
        public void BehaviourStart()
        {
            if(!Game.isHaveBooster) return;
            AddCheatcode();
            InitData();
        }

        private void AddCheatcode()
        {
            // ModuleController.cheatCode.AddCheatCodeButtonWithInput("Add Win Magic Pouch Streak Level", param =>
            // {
            //     _winStreakData.AddLevelWin();
            //     EventController.RaiseEvent(EventList.SAVE_DATA);
            // }, new CheatCode.InputParam
            // {
            //     value = 10
            // });
            //
            // ModuleController.cheatCode.AddCheatCodeButtonWithInput("Set Win Magic Pouch Streak Level", param =>
            // {
            //     int levelCount = int.Parse(param.inputParams[0].value.ToString());
            //     _winStreakData.SetLevelWin(levelCount);
            //     EventController.RaiseEvent(EventList.SAVE_DATA);
            // }, new CheatCode.InputParam
            // {
            //     value = 1
            // });
        }
        
        void InitData()
        {
            //_winStreakData = DataController.LoadData<WinStreakData>(MAGIC_POUCH_WIN_STREAK_SAVED_KEY);
//
            //if (_winStreakData.isKilledAppInGameplay) //reset data if killed in game
            //{
            //    _winStreakData.isKilledAppInGameplay = false;
            //    _winStreakData.ResetWinStreak();
            //}
            //
            //DataController.SetAutoSaveObject(MAGIC_POUCH_WIN_STREAK_SAVED_KEY, _winStreakData);
            //EventController.RaiseEvent(EventList.SAVE_DATA);
        }
        
        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.LEVEL_WIN:
                    var levelType = (LevelType) param.p0;
                    OnLevelWin(levelType);
                    break;
            }
        }

        void OnLevelWin(LevelType levelType)
        {
            if (!Game.isHaveBooster) return;
            //if (PlayerData.level < Config.magicPouchConfig.levelUnlock) return;
            if (levelType == LevelType.COIN) return;
            
            _winStreakData.AddLevelWin();
        }

        public void OnLevelLose()
        {
            if (!Game.isHaveBooster) return;
            //if (PlayerData.level < Config.magicPouchConfig.levelUnlock) return;
            
            _winStreakData.ResetWinStreak();
        }
    }
}