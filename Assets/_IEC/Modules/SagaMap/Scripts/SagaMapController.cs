using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class SagaMapController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart,
        IEventPauseApp, IEventQuitApp
    {
        public bool m_IsHaveBossLevel;//=>Game.isHaveBossLevel;

        public bool m_IsHaveBonusLevel;//=>Game.isHaveBonusLevel;

        public int m_BossLevelSpacing;

        public const string PLAYER_DATA_SAGAMAP_KEY = "PlayerDataSagaMap";
        [SerializeField] private PlayerSecretSagaMap m_SagaMapPlayerSecret;
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
                case EventList.GamePlay.LEVEL_WIN:
                    OnGameOver(true);
                    break;
                case EventList.GamePlay.LEVEL_LOSE:
                    OnGameOver(false);
                    break;
                case EventList.GamePlay.APPLY_AB_TEST:
                    break;
            }
        }

        void LoadData()
        {
            var playerData = Services.DataSecure()
                .GetObject(PLAYER_DATA_SAGAMAP_KEY, Pool.Get<PlayerSecretSagaMap>());
            Pool.Set(playerData);
            m_SagaMapPlayerSecret = Pool.Get<PlayerSecretSagaMap>();
            Debug.Log("Application.version " + Application.version);
        }
        public void SaveData()
        {
            if (Game.isDebugReset) return;
            Services.DataSecure().SetObject(PLAYER_DATA_SAGAMAP_KEY, Pool.Get<PlayerSecretSagaMap>());
            Services.SaveData();
        }

        void OnGameOver(bool isWin)
        {
            if (!isWin)
                return;
            m_SagaMapPlayerSecret.SetUnlockLevel();
            if (!IsBonusLevel(PlayerData.level + 1))
                return;
            m_SagaMapPlayerSecret.AddListBonusLevel(PlayerData.level);


        }

        public bool IsBonusLevel(int level)
        {
            return m_IsHaveBonusLevel && level > 3 && (level - 1) % 5 == 0;
        }

        public bool IsBossLevel(int level)
        {
            if (m_IsHaveBossLevel)
            {
                return (level - 5) % m_BossLevelSpacing == 0;
            }

            return false;
        }
    }
}