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
        public Button hideInfoButton;

        [Header("Win streak group")] 
        public GameObject streakLockObj, streakUnlockObj;
        public List<GameObject> streakDisplayObject = new();
        public List<GameObject> streakIconObject = new();

        [Header("Booster Buttons")] 
        public List<BoosterBaseButton> listBoosterButtons = new();

        [Header("Info panel")]
        public GameObject infoPanel;

        public List<GameObject> listInfoObjects = new();

        public Sequence showInfoSequence;

        private void Start()
        {
            SetEvent(infoButton, DoShowInfoPanel);
            SetEvent(playButton, OnClickPlayButton);
            SetEvent(hideInfoButton, () => { infoPanel.gameObject.SetActive(false); });
        }

        public override void Show()
        {
            base.Show();

            levelText.text = "Level " + PlayerData.level;
            
            UpdateBoosterButton();
            UpdateStreakDisplay();
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
            streakLockObj.SetActive(PlayerData.level < Config.AllBoosterConfig.levelUnlockStreak);
            streakUnlockObj.SetActive(PlayerData.level >= Config.AllBoosterConfig.levelUnlockStreak);

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
                                if (btn.State == BoosterBaseState.Using)
                                {
                                    this.RaiseEvent(EventList.GamePlay.ON_BOOSTER_COLLECTED, btn.BoosterData);
                                }
                            }
                        });
                    
                });
        }

        private void DoShowInfoPanel()
        {
            infoPanel.gameObject.SetActive(true);

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
