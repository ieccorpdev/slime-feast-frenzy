using System;
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

        ServicesController m_ServicesController;

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
                case EventList.NONE:
                    Debug.Log("event none");
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
            int emptyConfig = Pool.Get<GameConfig>().emptyConfig.configData;
            Debug.Log("emptyConfig : " + emptyConfig);
        }

        public void SaveData()
        {
            if (Game.isDebugReset) return;
            PlayerData.SaveLastPlayTime();
            Services.DataSecure().SetObject(PLAYER_DATA_KEY, Pool.Get<PlayerSecret>());
            Services.DataSecure().Save();
        }

        void LoadData()
        {
            var playerData = Services.DataSecure().GetObject(PLAYER_DATA_KEY, Pool.Get<PlayerSecret>());
            Pool.Set(playerData);

            m_PlayerSecret = Pool.Get<PlayerSecret>();

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
    }
}