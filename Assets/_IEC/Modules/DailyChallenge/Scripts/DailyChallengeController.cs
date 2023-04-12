using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class DailyChallengeController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart,
        IEventPauseApp, IEventQuitApp
    {
        #region Game Dailychallenge

        public int dailyChallengeChoosingMonth = -1;
        public int dailyChallengeChoosingDay = -1;

        #endregion
        public const string PLAYER_DATA_DAILYCHALLENGE_KEY = "PlayerDataDailyChallenge";
        [SerializeField] private PlayerSecretDailyChallenge m_PlayerSecretDailyChallenge;
        private bool m_IsNeedSaveData;
        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void BehaviourQuitApp()
        {
            SaveData();
        }
        public void BehaviourPauseApp(bool isPause)
        {
            if (isPause)
            {
                SaveData();
            }
        }
        public void BehaviourStart()
        {
            EventController.RaiseEvent(EventList.NONE);
        }

        public void BehaviourAwake()
        {
            LoadData();
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.SAVE_DATA:
                    m_IsNeedSaveData = true;
                    break;
                case EventList.GamePlay.APPLY_AB_TEST:
                    break;
            }
        }

        void LoadData()
        {
            var playerData = Services.DataSecure()
                .GetObject(PLAYER_DATA_DAILYCHALLENGE_KEY, Pool.Get<PlayerSecretDailyChallenge>());
            Pool.Set(playerData);
            m_PlayerSecretDailyChallenge = Pool.Get<PlayerSecretDailyChallenge>();
            Debug.Log("Application.version " + Application.version);
        }
        public void SaveData()
        {
            m_IsNeedSaveData = false;
            if (Game.isDebugReset) return;
            Services.DataSecure().SetObject(PLAYER_DATA_DAILYCHALLENGE_KEY, Pool.Get<PlayerSecretDailyChallenge>());
            Services.SaveData();
        }
        void LateUpdate()
        {
            if (m_IsNeedSaveData)
            {
                SaveData();
            }
        }
    }
}