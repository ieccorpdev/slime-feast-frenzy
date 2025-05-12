using System;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay;
using _IEC.Features.VipProHackerUITutorial;
using IEC;
using UnityEngine;

namespace _IEC.Features.Tutorial
{
    public class BaseHelperTutorial : _BaseBehaviour
    {
        [SerializeField] private BasePopupUnlockHelper popupUnlockHelper;
        [SerializeField] private HelperButton helperButton;
        [SerializeField] private HelperId helperId;
        private HighLightCanvas HighLightCanvas => Pool.Get<HighLightCanvas>();

        public void StartTutorial()
        {
            this.RaiseEvent(EventList.GamePlay.PAUSE_GAME);
            popupUnlockHelper.Show();
        }
        private void Awake()
        {
            helperButton.OnClick += CompleteHelper;

            popupUnlockHelper.OnClose += () =>
            {
                helperButton.ShowHint();
                HighLightCanvas.HighLightUI(helperButton.GetComponent<RectTransform>(),true,true);
            };
        }

        private void CompleteHelper()
        {
            HighLightCanvas.RemoveCurrentHighLight();
            PlayerData.helperData.SetCompleteTutorial(helperId);
            this.RaiseEvent(EventList.GamePlay.RESUME_GAME);
            helperButton.HideHint();
        }
    }
}