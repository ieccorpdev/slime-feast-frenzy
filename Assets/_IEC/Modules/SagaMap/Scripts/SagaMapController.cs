using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class SagaMapController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart,
        IEventPauseApp, IEventQuitApp
    {
        public bool m_IsHaveBossLevel { get; private set;}
        public bool m_IsHaveBonusLevel { get; private set; }
        public int m_BossLevelSpacing { get; private set; }

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
                .GetObject(PLAYER_DATA_SAGAMAP_KEY, Pool.Get<PlayerSecretSagaMap>());
            Pool.Set(playerData);
            m_SagaMapPlayerSecret = Pool.Get<PlayerSecretSagaMap>();
            Debug.Log("Application.version " + Application.version);
        }
        public void SaveData()
        {
            m_IsNeedSaveData = false;
            if (Game.isDebugReset) return;
            Services.DataSecure().SetObject(PLAYER_DATA_SAGAMAP_KEY, Pool.Get<PlayerSecretSagaMap>());
            Services.SaveData();
        }
        void LateUpdate()
        {
            if (m_IsNeedSaveData)
            {
                SaveData();
            }
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

        public void SetParam(bool isHaveBonusLevel, bool isHaveBossLevel, int bossLevelSpacing)
        {
            m_BossLevelSpacing = bossLevelSpacing;
            m_IsHaveBonusLevel = isHaveBonusLevel;
            m_IsHaveBossLevel = isHaveBossLevel;
        }
    }
}