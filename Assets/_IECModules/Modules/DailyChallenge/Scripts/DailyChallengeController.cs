using UnityEngine;

namespace IEC
{
    public class DailyChallengeController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {

        public const string PLAYER_DATA_DAILYCHALLENGE_KEY = "PlayerDataDailyChallenge";
        [SerializeField] private PlayerSecretDailyChallenge m_PlayerSecretDailyChallenge;

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void BehaviourStart()
        {
            EventController.RaiseEvent(EventList.NONE);
        }

        public void BehaviourAwake()
        {
            var dataController = Controllers.Get<DataController>();
            m_PlayerSecretDailyChallenge = dataController.LoadData<PlayerSecretDailyChallenge>(PLAYER_DATA_DAILYCHALLENGE_KEY);
            Pool.Set(m_PlayerSecretDailyChallenge);
            dataController.SetAutoSaveObject(PLAYER_DATA_DAILYCHALLENGE_KEY, m_PlayerSecretDailyChallenge);
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.APPLY_AB_TEST:
                    break;
                case EventList.GamePlay.LEVEL_WIN:
                    OnGameOver();
                    break;
                case EventList.GamePlay.LEVEL_LOSE:
                    OnGameOver();
                    break;
                case EventList.GamePlay.END_LEVEL_ADS:
                    ShowPopup();
                    break;
            }
        }

        void ShowPopup()
        {
            var popup = UI.GroupPopup.DailyChallenge;
            if (popup.isInDailyMode)
            {
                popup.Show();
                popup.ShowEffectStar();
            }

            popup.isInDailyMode = false;
        }

        void OnGameOver()
        {
            var popupChallenge = UI.GroupPopup.DailyChallenge;
            var dataChallenge = popupChallenge.DataDailyChallenge;
            if (popupChallenge.isInDailyMode)
            {
                bool isShowDailyEffect = dataChallenge
                    .months[popupChallenge.choosingMonth]
                    .days[popupChallenge.choosingDay] < 2;

                dataChallenge.months[popupChallenge.choosingMonth]
                    .days[popupChallenge.choosingDay] = 2;

                Services.PushEvent(new TrackingEvent.WinDailyChallenges());
            }
        }
    }
}