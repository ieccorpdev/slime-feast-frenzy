using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupProgressReward : BasePopup
    {

        public ProgressRewardItem progressRewardItemPrefab;
        [Header("Progress bar")]
        public ProgressRewardProgressBar progressBar;
        
        [Space(10)]
        public Transform rewardItemContainer;
        public List<ProgressRewardItem> listProgressRewardItems = new();
        
        public ProgressRewardItem CurrentRewardItem { get; private set; }
        [Header("Tutorial")] 
        public Button infoButton;
        public Button closeTutorialPanel;
        public GameObject tutorialPopup;

        [Header("Info ui")] 
        public Transform tapToContinueImg;
        public List<Transform> listInfoUI = new();

        private Sequence showInfoSequence;
        private void Start()
        {
            SetEvent(btnCloseAll, OnHideClick);
            SetEvent(infoButton, ()=>
            {
                OnShowInfo();
            });            
            SetEvent(closeTutorialPanel, ()=>
            {
                tutorialPopup.gameObject.SetActive(false);
            });
        }

        public override void Show()
        {
            base.Show();
            Setup();
        }

        public void Setup()
        {
            for (int i = 0; i < Config.progressRewardConfig.listRewardData.Count; i++)
            {
                if (i >= listProgressRewardItems.Count)
                {
                    listProgressRewardItems.Add(Instantiate(progressRewardItemPrefab, rewardItemContainer));
                }
                
                listProgressRewardItems[i].Setup(i + 1, Config.progressRewardConfig.GetRewardData(i));

                listProgressRewardItems[i].SetRewardClaimed(i < PlayerData.currentProgressMilestone);

                if (i == PlayerData.currentProgressMilestone)
                {
                    CurrentRewardItem = listProgressRewardItems[i];
                }
                
            }
            tutorialPopup.gameObject.SetActive(false);
            progressBar.Setup();
            progressBar.DoAnimProgressBar();
            
            listProgressRewardItems[0].SetBotItem();
            listProgressRewardItems[^1].SetTopItem();
        }

        private void OnShowInfo()
        {
            tutorialPopup.gameObject.SetActive(true);
            
            DOTween.Kill(showInfoSequence);
            showInfoSequence = DOTween.Sequence();

            foreach (var ui in listInfoUI)
            {
                ui.localScale = Vector3.zero;
            }

            tapToContinueImg.transform.localScale = new Vector3(0, 1, 1);

            float delay = 0f;
            foreach (var ui in listInfoUI)
            {
                showInfoSequence.Insert(delay, ui.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
                delay += 0.1f;
            }

            showInfoSequence.Insert(delay, tapToContinueImg.transform.DOScaleX(1, 0.2f).SetEase(Ease.OutBack));
            var scaleMulti = 1.1f;
            var duration = 0.6f;
            showInfoSequence.Append(tapToContinueImg.transform.DOScale(Vector3.one * scaleMulti, duration)
                .SetLoops(-1, LoopType.Yoyo));

            showInfoSequence.Play();
        }
    }
}
