using System.Collections.Generic;
using System.Linq;
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
            Services.DataSecure().SetObject(UserVersionData.KEY, Pool.Get<UserVersionData>());

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
            var userVerData = Services.DataSecure().GetObject(UserVersionData.KEY, Pool.Get<UserVersionData>());
            Pool.Set(userVerData);

            //InitUserVersionData();

            m_PlayerSecret = Pool.Get<PlayerSecret>();
            m_PlayerSecret.session++;

            // check update version
            m_PlayerSecret.lastVersion = Application.version;
            Debug.Log("Application.version " + Application.version);
        }

        void InitUserVersionData()
        {
            var serviceAbTest = Services.ABTest();
            var listAbTestConfig = Services.config.GetListAbTestConfig();
            var abIndex = listAbTestConfig.FindLastIndex(x => serviceAbTest.HasValue(x.KeyName));

            var forceSetAb = "ForceSetAB";
            var forceSetAbName = Services.DataSecure().GetString(forceSetAb, "No Force Set");
            var forceSetAbIndex = listAbTestConfig.FindLastIndex(x => x.KeyName == forceSetAbName);
            abIndex = (forceSetAbIndex >= 0) ? forceSetAbIndex : abIndex;

            var abConfig = listAbTestConfig.ElementAtOrDefault(abIndex);
            var userVerData = Pool.Get<UserVersionData>();
            if (userVerData.IsEmpty() && abConfig != null && abConfig.Date < 20240121) // Just example
            {
                userVerData.AddUserVersion(new UserVersion()
                {
                    version = "15.0.0",
                    abFullName = "AB_20240107_v580"
                });
            }
        }

        public void BehaviourPauseApp(bool pause)
        {
            if (pause)
            {
                SaveData();
            }
        }

        public void BehaviourQuitApp()
        {
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