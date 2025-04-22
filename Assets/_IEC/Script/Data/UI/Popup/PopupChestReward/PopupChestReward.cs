using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupChestReward : BasePopup
    {
        public Button btnCloseAll;
        public TextMeshProUGUI chestProgressText;
        public Image progressFillImage;

        public Button continueButton;

        private float _currentProgress;

        protected override void Awake()
        {
            base.Awake();
            
            SetEvent(continueButton, OnHideClick);
            SetEvent(btnCloseAll, OnHideClick);
        }

        public void Setup(ChestRewardData chestRewardData, int currentChestProgress)
        {
            chestProgressText.text = $"{currentChestProgress}/{chestRewardData.progressToReward}";

            progressFillImage.fillAmount = 0;

            _currentProgress = (float)currentChestProgress / chestRewardData.progressToReward;
        }

        public override void Show()
        {
            base.Show();

            progressFillImage.DOFillAmount(_currentProgress, 0.2f);
        }
        
    }
}
