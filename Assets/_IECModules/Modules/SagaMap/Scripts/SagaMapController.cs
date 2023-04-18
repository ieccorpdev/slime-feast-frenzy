using UnityEngine;

namespace IEC
{
    public class SagaMapController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
        public const string PLAYER_DATA_SAGAMAP_KEY = "PlayerDataSagaMap";
        [SerializeField] private PlayerSecretSagaMap m_SagaMapData;

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void BehaviourAwake()
        {
            var dataController = Controllers.Get<DataController>();
            m_SagaMapData = dataController.LoadData<PlayerSecretSagaMap>(PLAYER_DATA_SAGAMAP_KEY);
            Pool.Set(m_SagaMapData);
            dataController.SetAutoSaveObject(PLAYER_DATA_SAGAMAP_KEY, m_SagaMapData);
        }

        public void BehaviourStart()
        {
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.LEVEL_WIN:
                    OnGameOverWin();
                    break;
                case EventList.GamePlay.APPLY_AB_TEST:
                    break;
            }
        }

        void OnGameOverWin()
        {
            m_SagaMapData.SetUnlockLevel();
            if (!m_SagaMapData.IsBonusLevel(PlayerData.level + 1))
                return;
            m_SagaMapData.AddListBonusLevel(PlayerData.level);
        }
    }
}