using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class DataController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart, IEventPauseApp, IEventQuitApp
    {
        public const string PLAYER_DATA_KEY = "PlayerData";

        [SerializeField]
        private GameConfig m_GameConfig;

        [SerializeField]
        private Game m_GameData;

        [SerializeField]
        private PlayerSecret m_PlayerSecret;

        private bool m_IsNeedSaveData;

        private readonly Dictionary<string, object> m_DictAutoSave = new Dictionary<string, object>();

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.SAVE_DATA:
                    m_IsNeedSaveData = true;
                    break;
                case EventList.GamePlay.APPLY_AB_TEST:
                    m_GameData = Pool.Get<Game>();
                    break;
            }
        }

        public void BehaviourAwake()
        {
            Pool.Set(m_GameConfig);

            m_GameData = Pool.Get<Game>();
            LoadData();
        }

        public void BehaviourStart()
        {
            EventController.RaiseEvent(EventList.NONE);
        }

        public void SetAutoSaveObject(string saveDataKey, object obj)
        {
            m_DictAutoSave[saveDataKey] = obj;
        }

        public void SaveData()
        {
            m_IsNeedSaveData = false;
            if (Game.isDebugReset) return;
            PlayerData.SaveLastPlayTime();
            Services.DataSecure().SetObject(PLAYER_DATA_KEY, Pool.Get<PlayerSecret>());

            foreach (var kv in m_DictAutoSave)
            {
                Services.DataSecure().SetObject(kv.Key, kv.Value);
            }

            Services.SaveData();
        }

        public T LoadData<T>(string saveDataKey) where T : class, new()
        {
            return Services.DataSecure().GetObject<T>(saveDataKey, Pool.Get<T>());
        }

        void LoadData()
        {
            var playerData = Services.DataSecure().GetObject(PLAYER_DATA_KEY, Pool.Get<PlayerSecret>());
            Pool.Set(playerData);

            m_PlayerSecret = Pool.Get<PlayerSecret>();
            m_PlayerSecret.session++;
            m_PlayerSecret.AfterInit();
            // check update version
            m_PlayerSecret.lastVersion = Application.version;
            Debug.Log("Application.version " + Application.version);
        }

        public void BehaviourPauseApp(bool pause)
        {
            SetPraramUserQuit(pause);
            if (pause)
            {
                SaveData();
            }
        }
        private static void SetPraramUserQuit(bool isPause)
        {
            if (Scene.CurrentScene != SceneName.GAME)
            {
                PlayerData.levelPending = isPause;
                PlayerData.isKilledAppGamePlay = isPause;
            }
        }

        public void BehaviourQuitApp()
        {
            SetPraramUserQuit(true);
            SaveData();
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