using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopUpMagicPouch : _BaseBehaviour
    {
        public Action OnClose;
        public Action OnPlay;
        public Action OnShow; 
        
        [Header("Magic Pouch Progress")] 
        [SerializeField] private GameObject lockedProgress;
        [SerializeField] private GameObject unlockedProgress;
        [SerializeField] private Image progressFill;
        [SerializeField] private TextMeshProUGUI txtProgress;
        [SerializeField] private TextMeshProUGUI txtUnlockLevel;
        [SerializeField] private Button btnInfo;

        [Header("Magic Pouch Popup")] 
        [SerializeField] private Button btnPlayFromPopup;
        [SerializeField] private Button btnClosePopup;
        [SerializeField] private GameObject popupMagicPouch;
        
        private void Awake()
        {
            btnInfo.onClick.AddListener(ShowPopupMagicPouch);
            btnClosePopup.onClick.AddListener(OnCloseBtnClicked);
            btnPlayFromPopup.onClick.AddListener(OnPlayBtnClicked);
        }

        void OnCloseBtnClicked()
        {
            //SoundController.PlayExitBtnAudio();
            OnClose?.Invoke();
            HidePopupMagicPouch();
        }

        void OnPlayBtnClicked()
        {
            //SoundController.PlayClickBtnAudio();
            OnPlay?.Invoke();
        }
        
        public void UpdateMagicPouchProgress()
        {
            HidePopupMagicPouch();
            
            //if (PlayerData.level < Config.magicPouchConfig.levelUnlock)
            //{
            //    txtUnlockLevel.text = $"Unlock at level {Config.magicPouchConfig.levelUnlock}";
            //    lockedProgress.SetActive(true);
            //    unlockedProgress.SetActive(false);
            //    return;
            //}
            
            ShowAndUpdateProgress();
        }

        void ShowPopupMagicPouch()
        {
            //SoundController.PlayClickBtnAudio();
            //.showMagicPouchTutorial = true;
            popupMagicPouch.SetActive(true);
            OnShow?.Invoke();
        }

        void HidePopupMagicPouch()
        {
            popupMagicPouch.SetActive(false);
        }
        
        void ShowAndUpdateProgress()
        {
            lockedProgress.SetActive(false);
            unlockedProgress.SetActive(true);

            //var currentWinStreakCount = MagicPouchController.WinStreakData.countLevelWin;
            //if (currentWinStreakCount > 3)
            //{
            //    currentWinStreakCount = 3;
            //}

            //txtProgress.text = $"{currentWinStreakCount}/3";
            //progressFill.fillAmount = currentWinStreakCount / 3f;
        }
    }
}