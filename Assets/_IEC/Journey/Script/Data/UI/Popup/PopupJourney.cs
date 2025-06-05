using System;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace IEC
{
    public class PopupJourney : BasePopup
    {
        public ClaimBonusRewardPanel ClaimBonusRewardPanel => Locator.Main.ClaimBonusRewardPanel;
        public Button tutorialJourneyBtn;
        public GameObject progressbarMove;
        public VerticalLayoutGroup verticalItemJouney;
        public ScrollRect scrollRect;
        public RectTransform contentPanel;
        public List<ItemJourney> listItemJourney;
        public TutorialJourney tutorialJourneyPanel;
        [Header("FuelBar")]
        public Image fillFuelBar;
        public TextMeshProUGUI currentProgressTextTMP;
        public TextMeshProUGUI currentTarget;
        public TextMeshProUGUI timerTxt;
        [SerializeField] private float offsetMove = 25;
        [SerializeField] private float offsetScrollTo = -100;
        private bool _canShow;
        [SerializeField] private Button buyIAP, doneBuyIAP;
        public ItemPassJourney hiddenTreasure;
        public EventTrigger trigger;
        public GameObject bgdark;

        private void Start()
        {
            SetEvent(tutorialJourneyBtn, OnClickButtonTutorialJourney);
            base.SetEvent(buyIAP, () =>
            {
                Locator.Main.PopupVoyagePassIAPInfor.Show();
            });
            base.SetEvent(doneBuyIAP, () =>
            {
                Locator.Main.PopupVoyagePassIAPInfor.Show();
            });
            Locator.Main.PopupVoyagePassIAPInfor.configBundle.OnPurchaseSuccess += () =>
            {
                DOVirtual.DelayedCall(0.02f, RefreshUI);
                doneBuyIAP.gameObject.SetActive(true);
                buyIAP.gameObject.SetActive(false);
            };
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            entry.callback.AddListener((data) => {
                var currentInfo = ItemPassJourney.currentInfo;
                if (currentInfo)
                {
                    currentInfo.transform.DOKill();
                    currentInfo.SetActive(false);
                }
            });
            trigger.triggers.Add(entry);
        }

        public void CheckShow()
        {
            if (_canShow)
            {
                DOVirtual.DelayedCall(1.68f, () =>
                {
                    Locator.Main.ScreenMain.SetInteractive(true);
                    Show();
                });
            }
        }

        public bool GetHideStatus()
        {
            return !_canShow && !IsShow;
        }

        public override void Hide()
        {
            base.Hide();
            StopAllCoroutines();
            Pool.Get<UnderseaVoyageButton>().InitStatusButton();
            _canShow = false;
        }

        [Button]
        public override void Show()
        {
            SetUpUI();
            base.Show();
            if (!PlayerData.journeyData.isShowTutorialInPopup)
            {
                tutorialJourneyPanel.Show();
            }
            StartCoroutine(UpdateTimer());
        }

        public void SetUpUI()
        {
            SetProgressFuel();
            InitData();
            SetProgressBar();
        }

        public void RefreshUI()
        {
            SetProgressFuel();
            InitData();
            SetProgressBar(false);
        }

        public int GetAvailableClaimCount()
        {
            InitData();
            GetAllReward(out var listItem);
            return listItem.Count;
        }

        public void ClaimAllItem()
        {
            InitData();
            GetAllReward(out var listItem);
            var listReward = GetAllReward(out List<ItemPassJourney> values);
            if (listReward.Count > 0)
            {
                ClaimBonusRewardPanel.SetupRewardPanel(listReward, () =>
                {
                    ResetData();
                }, false);
                ClaimBonusRewardPanel.ShowingRewardPanel();
            }
            else
            {
                ResetData();
            }
        }

        private static void ResetData()
        {
            PlayerData.journeyData.ResetJourneyData();
            PlayerData.CheckPlayerMaxHealth();
            Pool.Get<UnderseaVoyageButton>().InitStatusButton();
        }

        public void InitData()
        {
            for (int i = 0; i < listItemJourney.Count; i++)
            {
                listItemJourney[i].SetUpDataItemJourney(Config.journeyTripConfig.GetRewardsByLevelJourney(i));
            }
            hiddenTreasure.SetupDataItemPass(Config.journeyTripConfig.bonusReward.RewardsVips, Config.journeyTripConfig.bonusReward.idJourney);
        }

        IEnumerator UpdateTimer()
        {
            while (true)
            {
                timerTxt.text = PlayerData.journeyData.GetTimeRemainingString();
                bool value = PlayerData.journeyData.isBuyIAP;
                doneBuyIAP.gameObject.SetActive(value);
                buyIAP.gameObject.SetActive(!value);
                yield return new WaitForSeconds(1f);
            }
        }

        void OnClickButtonTutorialJourney()
        {
            tutorialJourneyPanel.gameObject.SetActive(true);
            tutorialJourneyPanel.Show();
        }

        void SetProgressFuel()
        {
            int currentFuel = PlayerData.journeyData.currentFuelJourney;
            int targetFuel = Config.journeyTripConfig.GetRewardsByLevelJourney(PlayerData.journeyData.currentTargetJourney).fuelNeed;
            while (currentFuel >= targetFuel)
            {
                PlayerData.journeyData.currentFuelJourney -= targetFuel;
                currentFuel = PlayerData.journeyData.currentFuelJourney;
                PlayerData.journeyData.currentTargetJourney++;
                targetFuel = Config.journeyTripConfig.GetRewardsByLevelJourney(PlayerData.journeyData.currentTargetJourney).fuelNeed;
            }
            if (PlayerData.journeyData.currentTargetJourney >= Config.journeyTripConfig.listRewardsByJourneys.Count)
            {
                progressbarMove.SetActive(false);
                PlayerData.journeyData.currentTargetJourney = Config.journeyTripConfig.listRewardsByJourneys.Count + 1;
                PlayerData.journeyData.isCompleteJourney = true;
                fillFuelBar.fillAmount = 1;
                currentProgressTextTMP.text = targetFuel + "/" + targetFuel;
                currentTarget.text = Config.journeyTripConfig.GetRewardsByLevelJourney(PlayerData.journeyData.currentTargetJourney).idJourney.ToString();
            }
            else
            {
                fillFuelBar.fillAmount = (float)currentFuel / (float)targetFuel;
                currentProgressTextTMP.text = currentFuel + "/" + targetFuel;
                currentTarget.text = PlayerData.journeyData.currentTargetJourney.ToString();
            }
        }

        void SetProgressBar(bool snap = true)
        {
            int currentTarget = PlayerData.journeyData.currentTargetJourney;
            //int currentTarget = 20;
            if (currentTarget > listItemJourney.Count)
            {
                currentTarget = listItemJourney.Count;
            }
            RectTransform target = listItemJourney[currentTarget - 1].GetComponent<RectTransform>();
            if (snap) 
                SnapTo(target);
            progressbarMove.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, target.anchoredPosition.y + offsetMove);
        }

        public void OnCollectAllButtonClick(List<Reward> dataReward = null, ItemPassJourney item = null)
        {
            List<Reward> listReward = new List<Reward>();
            if (dataReward != null)
            {
                listReward = dataReward;
                ClaimBonusRewardPanel.SetupRewardPanel(listReward, () =>
                {
                    item.MakeItemClaimed();
                    CheckShow();
                    RefreshUI();
                }, true);
            }
            else
            {
                listReward = GetAllReward(out var listItem);
                ClaimBonusRewardPanel.SetupRewardPanel(listReward, () =>
                {
                    foreach (var item in listItem)
                    {
                        item.MakeItemClaimed();
                    }
                    CheckShow();
                    RefreshUI();
                }, true);
            }
            
            Hide();
            _canShow = true;
            Locator.Main.ScreenMain.SetInteractive(false);
        }

        public List<Reward> GetAllReward(out List<ItemPassJourney> listItem)
        {
            List<Reward> listReward = new List<Reward>();
            listItem = new List<ItemPassJourney>();
            foreach (var item in listItemJourney)
            {
                if (item.regularPass.statusItem == JourneyItemStatus.CANCLAIM)
                {
                    foreach (var dataReward in item.regularPass.dataReward)
                    {
                        AddDataReward(ref listReward, dataReward);
                    }

                    listItem.Add(item.regularPass);
                }

                if (item.vipPass.statusItem == JourneyItemStatus.CANCLAIM)
                {
                    foreach (var dataReward in item.vipPass.dataReward)
                    {
                        AddDataReward(ref listReward, dataReward);
                    }

                    listItem.Add(item.vipPass);
                }
            }
            if (hiddenTreasure.statusItem == JourneyItemStatus.CANCLAIM)
            {
                foreach (var dataReward in hiddenTreasure.dataReward)
                {
                    AddDataReward(ref listReward, dataReward);
                }
                listItem.Add(hiddenTreasure);
            }
            return listReward;
        }

        private static void AddDataReward(ref List<Reward> listReward, Reward dataReward)
        {
            var objF = listReward.FirstOrDefault(x => x.typeReward == dataReward.typeReward);
            if (objF != null)
            {
                objF.rewardAmount += dataReward.rewardAmount;
            }
            else
            {
                listReward.Add( new Reward(dataReward.rewardAmount,dataReward.typeReward) );
            }
        }

        public List<Reward> GetRewardLastChest()
        {
            InitData();
            List<Reward> listReward = new List<Reward>();
            listReward = GetAllReward(out var listItem);
            if (PlayerData.journeyData.isEnableMaxHeart)
            {
                Reward rewardLastChest = new Reward();
                rewardLastChest.typeReward = ResourceType.Gold;
                //rewardLastChest.rewardAmount = PlayerData.currentTargetJourney * Config.JourneyTripConfig.coinGetEacLevel;
                AddDataReward(ref listReward, rewardLastChest);
            }
            return listReward;
        }

        public void SnapTo(RectTransform target)
        {
            Canvas.ForceUpdateCanvases();
            contentPanel.anchoredPosition =
                (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position) - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
            contentPanel.anchoredPosition = new Vector2(contentPanel.anchoredPosition.x, contentPanel.anchoredPosition.y + offsetScrollTo);
        }
    }
}
