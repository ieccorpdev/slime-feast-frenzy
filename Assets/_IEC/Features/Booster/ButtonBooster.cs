using System;
using _IEC.Features.Common.Extension;
using _IEC.Features.VipProHackerUITutorial;
using IEC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Features.Booster
{
    public class ButtonBooster : _BaseBehaviour
    {
        [SerializeField] private Button btn;
        [SerializeField] private GameObject groupUsable;
        [SerializeField] private GameObject groupUsing;
        [SerializeField] private GameObject groupLock;
        [SerializeField] private GameObject groupUnlimited;
        [SerializeField] private GameObject groupNeedBuy;
        [SerializeField] private GameObject groupTutorial;
        [SerializeField] private TextMeshProUGUI txtPool;
        [SerializeField] private TextMeshProUGUI txtTimer;
        [SerializeField] private TextMeshProUGUI txtCondition;
        [SerializeField] private BoosterId boosterId;

        private State m_CurrentState;
        private RectTransform m_RectTransform;
        private bool _isForceEndTutorial = false;
        public State CurrentState => m_CurrentState;
        public BoosterId BoosterId => boosterId;
        public bool IsForceEndTutorial => _isForceEndTutorial;
        public RectTransform RectTransform
        {
            get
            {
                if (m_RectTransform == null) m_RectTransform = GetComponent<RectTransform>();
                return m_RectTransform;
            }
        }

        //private BoosterConfigModel ConfigModel => Config.boosterConfig.GetBoosterConfigModelById(boosterId);
        private BoosterDataModel DataModel => PlayerData.boosterData.GetDataModelById(boosterId);
        public void OnShow()
        {
            SetActiveTutorial(false);
            UpdateUIState();
            gameObject.SetActive(true);
        }

        private void UpdateUIState()
        {
            if (DataModel.IsInUnlimitedTime())
            {
                ChangeState(State.Unlimited);
            }
            else
            {
                //if (PlayerData.level < ConfigModel.unlockLevel)
                //{
                //    ChangeState(State.Locked);
                //}
                //else
                {
                    ChangeState(DataModel.number > 0 ? State.Usable : State.NeedBuy);
                }
            }
        }

        
        public void Dispose()
        {
            m_CurrentState = State.None;
            gameObject.SetActive(false);
        }
        
        private void Awake()
        {
            btn.onClick.AddListener(() =>
            {
                //SoundController.PlayClickBtnAudio();
                switch (m_CurrentState)
                {
                    case State.Usable:
                        ChangeState(State.Using);
                        break;
                    case State.Using:
                        ChangeState(State.Usable);
                        break;
                    case State.NeedBuy:
                        ShowBuyingBoosterPopup(isPurchased =>
                        {
                            if (isPurchased)
                            {
                                ChangeState(State.Using);
                            }
                        });
                        break;
                    case State.Unlimited:
                        ForceEndTutorial();
                        break;
                }
            });
            Observer.AddObserver(EventList.GamePlay.UPDATE_BOOSTER_UI, OnUpdateBoosterUI);
        }

        private void OnUpdateBoosterUI(object[] param)
        {
            var id = (BoosterId)param[0];
            if (id != boosterId) return;
            
            UpdateUIState();
        }
        
        private void Update()
        {
            if (m_CurrentState == State.Unlimited)
            {
                UpdateUnlimitedState();
            }
        }

        private void UpdateUnlimitedState()
        {
            var remainingTime = DataModel.unlimitedExpiredTime.Subtract(DateTime.Now);
            if (remainingTime.TotalSeconds > 0)
            {
                txtTimer.text = remainingTime.ToString(@"mm\:ss");
            }
            else
            {
                ChangeState(DataModel.number > 0 ? State.Usable : State.NeedBuy);
            }
        }

        public void ChangeState(State newState)
        {
            if (newState == m_CurrentState)
            {
                if (m_CurrentState == State.Usable)
                {
                    txtPool.text = DataModel.number.ToString(); //force update UI
                }
                return;
            }
            groupUsable.SetActive(newState == State.Usable);
            groupUsing.SetActive(newState == State.Using);
            groupLock.SetActive(newState == State.Locked);
            groupUnlimited.SetActive(newState == State.Unlimited);
            groupNeedBuy.SetActive(newState == State.NeedBuy);
            m_CurrentState = newState;
            switch (newState)
            {
                case State.Usable:
                    txtPool.text = DataModel.number.ToString();
                    break;
                case State.Locked:
                    //txtCondition.text = "Lv." + ConfigModel.unlockLevel;
                    break;
            }
        }

        public void SetActiveTutorial(bool active)
        {
            groupTutorial.SetActive(active);
        }
        private void ShowBuyingBoosterPopup(Action<bool> callback)
        {
            //var buyingPopup = UI.groupBoosterPopup.GetPopupBuyBoosterById(boosterId);
            //buyingPopup.Show(callback);
        }

        void ForceEndTutorial()
        {
            SetActiveTutorial(false);
            var boosterData = PlayerData.boosterData.GetDataModelById(boosterId);
            boosterData.CompleteTutorial();
            Pool.Get<HighLightCanvas>().RemoveCurrentHighLight();
            _isForceEndTutorial = true;
        }
        
        public enum State
        {
            None,
            Usable,
            Using,
            Locked,
            Unlimited,
            NeedBuy
        }
    }
}
