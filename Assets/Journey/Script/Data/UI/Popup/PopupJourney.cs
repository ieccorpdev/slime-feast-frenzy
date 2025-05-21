using System;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupJourney : BasePopup
    {
        public ClaimBonusRewardPanel ClaimBonusRewardPanel;
        [Header("MainJourney")]
        public Button tutorialJourneyBtn;
        public Image lightBGFilled;
        public Image darkBGFilled;
        public SlicedFilledImage middleFilledBar;
        public GameObject progressbarEffect;
        public GameObject progressbarMove;
        public GameObject progressbarLockEnd;
        public VerticalLayoutGroup verticalItemJouney;
        public ScrollRect scrollRect;
        public RectTransform contentPanel;
        public List<ItemJourney> listItemJourney;
        [Header("TutorialJourney")]
        public TutorialJourney tutorialJourneyPanel;

        [Header("ChestBottom")] 
        public Button collectEndJourneyTripBtn;
        public GameObject bottomChestVipPassNotPurchase;
        public GameObject bottomChestVipPassPurchase;
        [Header("Bannerbottom")]
        public RectTransform vipPassBar;
        public GameObject bottomBannerVipPassPurchase;
        public Button activeVipPassBtn;
        public Button collectAllBtn;
        [Header("FuelBar")]
        public Image fillFuelBar;
        public TextMeshProUGUI currentProgressTextTMP;
        public TextMeshProUGUI currentTarget;
        [Header("Anim")] 
        public SpineAnimOT cloudAim;
        public SpineAnimOT carAnim;
        public SpineAnimOT carVipAnim;
        public bool inAnimCar;
        private void Start()
        {
            SetEvent(tutorialJourneyBtn, OnClickButtonTutorialJourney);
            SetEvent(activeVipPassBtn, OnClickActiveVipPass);
            SetEvent(collectAllBtn, OnCollectAllButtonClick);
        }

        private void OnDisable()
        {
            carAnim.ResetAnim();
            carVipAnim.ResetAnim();
        }

        public override void Hide()
        {
            base.Hide();
            //UI.buildCityUI.ShowScale();
        }
        [Button]
        public override void Show()
        {
            //PlayerData.wasOpenJourney = true;
            //if (PlayerData.timeStartJourneyTrip == DateTime.MinValue)
            //{
            //    PlayerData.timeStartJourneyTrip = DateTime.Now;
            //}
            //UI.buildCityUI.DeactiveBuildCity();
            SetUpUI();
            base.Show();
            carAnim.SetAnimation("2-Idle", true);
            AnimCarStart();
        }

        public void AnimCarStart()
        {
            carAnim.gameObject.SetActive(false);
            carVipAnim.gameObject.SetActive(false);
            carVipAnim.ResetAnim();
            carAnim.ResetAnim();
            if (!PlayerData.purchasedJourneyPass)
            {
                carAnim.gameObject.SetActive(true);
                carAnim.SetAnimation("1-In",false, () =>
                {
                    carAnim.SetAnimation("2-Idle", true);
                    inAnimCar = false;
                });
            }
            else
            {
                carVipAnim.gameObject.SetActive(true);
                carVipAnim.SetAnimation("1-In",false, () =>
                {
                    carVipAnim.SetAnimation("2-Idle", true);
                    inAnimCar = false;
                });
            }
        }

        public void PlayChangeCarSelectVipPass()
        {
            if(inAnimCar) return;
            inAnimCar = true;
            carAnim.SetAnimation("3-Out",false, () =>
            {
                carAnim.gameObject.SetActive(false);
                carVipAnim.gameObject.SetActive(true);
                carVipAnim.ResetAnim();
                carVipAnim.SetAnimation("1-In", false, () =>
                {
                    carVipAnim.SetAnimation("2-Idle", true);
                    TweenControl.GetInstance().DelayCall(gameObject.transform,2f, () =>
                    {
                        carVipAnim.SetAnimation("3-Out", false, () =>
                        {
                            carAnim.gameObject.SetActive(true);
                            carVipAnim.gameObject.SetActive(false);
                            carAnim.ResetAnim();
                            carAnim.SetAnimation("1-In",false, () =>
                            {
                                TweenControl.GetInstance().DelayCall(gameObject.transform, 2f, () => { inAnimCar = false;});
                                carAnim.SetAnimation("2-Idle", true, () =>
                                {
                                });
                            });
                        });
                    });
                });
            });
        }
        public void SetUpUI()
        {
            InitData();
            SetProgressFuel();
            SetProgressBar();
            SetChestBottom();
            SetBannerBottom();
        }

        public void RefreshUI()
        {
            InitData();
            SetProgressFuel();
            SetProgressBar(false);
            SetChestBottom();
            SetBannerBottom();
            CheckNotiHomeScreen();
        }

        public void CheckNotiHomeScreen()
        {
            //var item = UI.popupJourney.GetAllReward(out var listItem);
            //UI.screenHome.journeyHomeIcon.SetNoti(item.Count>0);
        }
        public void InitData()
        {
            for (int i = 0; i < listItemJourney.Count; i++)
            {
                //listItemJourney[i].SetUpDataItemJourney(Config.JourneyTripConfig.GetRewardsByLevelJourney(i));
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (tutorialJourneyPanel.gameObject.activeInHierarchy && tutorialJourneyPanel.canDisable)
                {
                    tutorialJourneyPanel.gameObject.SetActive(false);
                }
            }
        }

        void OnClickButtonTutorialJourney()
        {
            tutorialJourneyPanel.gameObject.SetActive(true);
            tutorialJourneyPanel.Show();
        }
        
        void SetProgressFuel()
        {
            //int currentFuel = PlayerData.currentFuelJourney;
            //int targetFuel = Config.JourneyTripConfig.GetRewardsByLevelJourney(PlayerData.currentTargetJourney).fuelNeed;
            //while (currentFuel >= targetFuel)
            //{
            //    PlayerData.currentFuelJourney -= targetFuel;
            //    currentFuel = PlayerData.currentFuelJourney;
            //    PlayerData.currentTargetJourney++;
            //    targetFuel = Config.JourneyTripConfig.GetRewardsByLevelJourney(PlayerData.currentTargetJourney).fuelNeed;
            //}
            //if (PlayerData.currentTargetJourney >= Config.JourneyTripConfig.listRewardByJourneys.Count)
            //{
            //    fillFuelBar.fillAmount = 1;
            //    currentProgressTextTMP.text = targetFuel + "/" + targetFuel;
            //    currentTarget.text = Config.JourneyTripConfig.GetRewardsByLevelJourney(PlayerData.currentTargetJourney).idJourney.ToString();
            //}
            //else
            //{
            //    fillFuelBar.fillAmount = (float)currentFuel / (float)targetFuel;
            //    currentProgressTextTMP.text = currentFuel + "/" + targetFuel;
            //    currentTarget.text = PlayerData.currentTargetJourney.ToString();
            //}

        }
        void SetProgressBar(bool snap = true)
        {
            int currentTarget = PlayerData.currentTargetJourney+1;
            //int currentTarget = 20;
            if (currentTarget > listItemJourney.Count)
            {
                currentTarget = listItemJourney.Count;
            }
            lightBGFilled.fillAmount = (float)(currentTarget-0.5f)/ (float)(listItemJourney.Count-1);
            darkBGFilled.fillAmount = 1-lightBGFilled.fillAmount;
            darkBGFilled.gameObject.SetActive(darkBGFilled.fillAmount>0);
            middleFilledBar.fillAmount =(float) currentTarget / (float)(listItemJourney.Count-1);
            
            progressbarEffect.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,CaculatePositionYProgressBar(currentTarget));
            progressbarMove.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,CaculatePositionYProgressBar(currentTarget));
            if(snap) SnapTo(listItemJourney[currentTarget-1].GetComponent<RectTransform>());
        }
        
        int CaculatePositionYProgressBar(int id)
        {
            int padding = verticalItemJouney.padding.bottom;
            float spacing = verticalItemJouney.spacing;
            int itemHeight = (int) padding + (int) (spacing+100) * (listItemJourney.Count - id)+450;
            int diffY = (int)verticalItemJouney.GetComponent<RectTransform>().anchoredPosition.y*-1;
            return itemHeight + diffY-50;
        }

        void OnClickActiveVipPass()
        {
            //UI.popupJourneyIAP.Show();
        }

        void OnCollectAllButtonClick()
        {
            var listReward = GetAllReward(out var listItem);

            //Claim all Chest
            ClaimBonusRewardPanel.gameObject.SetActive(true);
            ClaimBonusRewardPanel.SetupRewardPanel(listReward, () =>
            {
                foreach (var item in listItem)
                {
                    item.MakeItemClaimed();
                }
                RefreshUI();
            });
            ClaimBonusRewardPanel.ShowingRewardPanel("Wooden_Chest");
            //UI.screenHome.journeyHomeIcon.SetNoti();
        }

        public List<Reward> GetAllReward(out List<ItemPassJourney> listItem)
        {
            List<Reward> listReward = new List<Reward>();
            listItem = new List<ItemPassJourney>();
            TypeItemPassJourney type = TypeItemPassJourney.NORMAL;
            foreach (var item in listItemJourney)
            {
                if (item.regularPass.statusItem == JourneyItemStatus.CANCLAIM)
                {
                    foreach (var dataReward in item.regularPass.dataReward.listRewards)
                    {
                        AddDataReward(ref listReward, dataReward);
                    }

                    listItem.Add(item.regularPass);
                }

                if (item.vipPass.statusItem == JourneyItemStatus.CANCLAIM)
                {
                    foreach (var dataReward in item.vipPass.dataReward.listRewards)
                    {
                        AddDataReward(ref listReward, dataReward);
                    }

                    listItem.Add(item.vipPass);
                }
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
            if (PlayerData.purchasedJourneyPass)
            {
                Reward rewardLastChest = new Reward();
                rewardLastChest.typeReward = ResourceType.Gold;
                //rewardLastChest.rewardAmount = PlayerData.currentTargetJourney * Config.JourneyTripConfig.coinGetEacLevel;
                AddDataReward(ref listReward, rewardLastChest);
            }
            return listReward;
        }
        bool AnyItemCanClaimed()
        {
            //Logic claim all
            foreach (var item in PlayerData.vipJourneyPass)
            {
                if (item.Value == JourneyItemStatus.CANCLAIM) return true;
            }              
            foreach (var item in PlayerData.regularJourneyPass)
            {
                if (item.Value == JourneyItemStatus.CANCLAIM) return true;
            }
            return false;
        }
        void SetChestBottom()
        {
            collectEndJourneyTripBtn.gameObject.SetActive(false);
            //Add logic collect end season
            bottomChestVipPassNotPurchase.gameObject.SetActive(!PlayerData.purchasedJourneyPass);
            bottomChestVipPassPurchase.gameObject.SetActive(PlayerData.purchasedJourneyPass);
        }

        private void SetBannerBottom()
        {
            bottomBannerVipPassPurchase.gameObject.SetActive(PlayerData.purchasedJourneyPass);
            activeVipPassBtn.gameObject.SetActive(!PlayerData.purchasedJourneyPass);
            //Add Logic active when can claim;
            Vector2 pos = PlayerData.level >= 15 ? new Vector2(vipPassBar.anchoredPosition.x, Services.Ads().GetBannerHeight()) : Vector2.zero;
            vipPassBar.anchoredPosition = pos;
            if (PlayerData.purchasedJourneyPass)
            {
                if (AnyItemCanClaimed())
                {
                    collectAllBtn.gameObject.SetActive(true);
                    bottomBannerVipPassPurchase.gameObject.SetActive(false);
                }
                else
                {
                    collectAllBtn.gameObject.SetActive(false);
                    bottomBannerVipPassPurchase.gameObject.SetActive(true);
                }
            }
            else
            {
                collectAllBtn.gameObject.SetActive(AnyItemCanClaimed());
            }

        }
        public void SnapTo(RectTransform target)
        {
            Canvas.ForceUpdateCanvases();
            contentPanel.anchoredPosition =
                (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position) - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
        }
    }
}
