using _IEC.Features.Common.Extension;
using IEC;
using IEC.TrackingEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Features.Gameplay
{
    public class PopupPauseGame : BasePopup
    {
        [SerializeField] private Button btnQuit;
        [SerializeField] private Button btnResume;
        [SerializeField] private GroupCurrencyResult groupCurrencyResult;

        protected override void Awake()
        {
            base.Awake();
            btnQuit.onClick.AddListener(QuitBtnFunction);
            btnResume.onClick.AddListener(HideButtonFunction);
        }

        public override void Show()
        {
            UpdateUI();
            base.Show();
            this.RaiseEvent(EventList.GamePlay.PAUSE_GAME);
        }

        void HideButtonFunction()
        {
            this.RaiseEvent(EventList.GamePlay.RESUME_GAME);
            //SoundController.PlayClickBtnAudio();
            Hide();
        }
        
        private void UpdateUI()
        {
            groupCurrencyResult.UpdateUI();
        }

        private void QuitBtnFunction()
        {
            Hide();
            //SoundController.PlayClickBtnAudio();
         
        }

        void Quit()
        {
            PlayerData.DecreasePlayerLife();
            EventController.RaiseEvent(EventList.GamePlay.LEVEL_CLEAR);
            //MagicPouchController.OnLevelLose();
            
        }
    }
}