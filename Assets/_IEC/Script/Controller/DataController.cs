using System.Collections.Generic;
using _IECModules;
using IEC.CloudSave;
using Newtonsoft.Json;
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

            // check update version
            m_PlayerSecret.lastVersion = Application.version;
            Debug.Log("Application.version " + Application.version);
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
        
        // Convert from local data, param save is used to hold the reference
        public CloudSaveData ConvertToCloudSaveData(CloudSaveData save)
        {
            foreach (var kv in m_DictAutoSave)
            {
                save.dictDatas[kv.Key] = kv.Value.JsonSerialize();
            }

            return save;
        }

        public void ApplyCloudSave(CloudSaveData data)
        {
            if (data == null || data.dictDatas == null) return;

            Debug.LogWarning("[CLOUD SAVE] Data Controller Apply Cloud Save Data");
            foreach (var kv in data.dictDatas)
            {
                var dataKey = kv.Key;
                var dataValue = kv.Value;
                
                if (m_DictAutoSave.TryGetValue(dataKey, out var value))
                {
                    var deserializedData = JsonConvert.DeserializeObject(dataValue);
                    Debug.LogWarning($"BEFORE: DataKey: {dataKey}, DATA TYPE: {deserializedData}, DeserializedData: {deserializedData.JsonSerialize()}");
                    value.CopyValuesFrom(deserializedData);
                    Debug.LogWarning($"AFTER: DataKey: {dataKey}, DATA TYPE: {value}, Value: {value.JsonSerialize()}");
                }
            }

            if (PlayerData.isNoAds) // Only apply if no ads = true
            {
                Services.ActiveNoAds(true);
                // TODO: Uncommnent and modify this if your code already has an event when no ads is successfully applied
                // this.RaiseEvent(EventList.IAP.BUY_NO_ADS_SUCCESSFULLY, "");
            }
            
            EventController.RaiseEvent(EventList.Service.CLOUD_APPLY_COMPLETED);
        }
    }
}