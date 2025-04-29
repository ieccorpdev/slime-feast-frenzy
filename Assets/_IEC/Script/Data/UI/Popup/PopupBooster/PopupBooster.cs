using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupBooster : BasePopup
    {
        public TextMeshProUGUI levelText;
        public Button infoButton;
        public Button playButton;
        public Button closeAllButton;

        [Header("Win streak group")] 
        public GameObject streakLockObj, streakUnlockObj;
        public TextMeshProUGUI levelUnlockStreak;
        public List<GameObject> streakDisplayObject = new();
        public List<GameObject> streakIconObject = new();

        [Header("Booster Buttons")] 
        public List<BoosterBaseButton> listBoosterButtons = new();
        public GameObject boosterPanel;

        [Header("Info panel")]
        public GameObject infoPanel;

        public List<GameObject> listInfoObjects = new();

        public Sequence showInfoSequence;

        private void Start()
        {
            SetEvent(infoButton, DoShowInfoPanel);
            SetEvent(playButton, OnClickPlayButton);
            SetEvent(closeAllButton, OnHideClick);
        }

        public override void Show()
        {
            base.Show();

            levelText.text = "Level " + PlayerData.level;
            
            infoPanel.SetActive(false);
            boosterPanel.SetActive(true);
            
            UpdateBoosterButton();
            UpdateStreakDisplay();
        }

        protected override void OnHideClick()
        {
            if (infoPanel.activeInHierarchy)
            {
                infoPanel.SetActive(false);
                boosterPanel.SetActive(true);
            }
            else
            {
                base.OnHideClick();
            }
        }

        private void UpdateBoosterButton()
        {
            foreach (var btn in listBoosterButtons)
            {
                btn.Setup();
            }
        }

        private void UpdateStreakDisplay()
        {
            levelUnlockStreak.text ="Unlock at lv."+ Config.AllBoosterConfig.levelUnlockStreak.ToString();
            streakLockObj.SetActive(PlayerData.level < Config.AllBoosterConfig.levelUnlockStreak);
            bool isUnlockFeature = PlayerData.level >= Config.AllBoosterConfig.levelUnlockStreak;
            streakUnlockObj.SetActive(isUnlockFeature);
            if (isUnlockFeature && PlayerData.currentBoosterStreak == 0)
            {
                //MeoMew fix Task #67920, unlock first phase
                PlayerData.currentBoosterStreak = 1;
            }
            for (int i = 0; i < streakDisplayObject.Count; i++)
            {
                streakDisplayObject[i].gameObject.SetActive(i <= PlayerData.currentBoosterStreak - 1);
            }

            for (int i = 0; i < streakIconObject.Count; i++)
            {
                streakIconObject[i].SetActive(i == Mathf.Clamp(PlayerData.currentBoosterStreak,0, streakIconObject.Count - 1));
            }
            
        }

        private void OnClickPlayButton()
        {
            Hide();
            Locator.Main.HomeScreenTab.DoMoveOutAllObj(0.3f,
                () =>
                {
                    Services.PushEvent(new IEC.TrackingEvent.ClickPlayHomeScreen(PlayerData.GetPlayerCurrencyCount(ResourceType.Gold).ToString()));
                    GameController.OpenScene(GameScene.Game,
                        () =>
                        {
                            foreach (var btn in listBoosterButtons)
                            {
                                if (btn.State == BoosterBaseState.Using || btn.State == BoosterBaseState.Unlimited)
                                {
                                    //this.RaiseEvent(EventList.GamePlay.ON_BOOSTER_COLLECTED, btn.BoosterData);
                                    this.RaiseEvent(EventList.GamePlay.SPAWN_BOOSTER_ITEM, new SpawnBoosterData(){amount = 1, boosterData = btn.BoosterData});
                                    
                                    if (!PlayerData.IsUnlimitedBooster(btn.BoosterData.boosterType))
                                    {
                                        PlayerData.SubtractCurrency(btn.BoosterData.boosterType, 1);
                                    }
                                }
                            }
                        });
                    
                });
        }

        private void DoShowInfoPanel()
        {
            boosterPanel.SetActive(false);
            infoPanel.SetActive(true);

            foreach (var obj in listInfoObjects)
            {
                obj.transform.localScale = Vector3.zero;
            }

            DOTween.Kill(showInfoSequence);
            showInfoSequence = DOTween.Sequence();

            float delay = 0f;
            
            foreach (var obj in listInfoObjects)
            {
                showInfoSequence.InsertCallback(delay,
                    () =>
                    {
                        obj.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutQuad);
                    });

                delay += 0.1f;
            }
        }
    }
}
