using System;
using IEC;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Features.Tutorial
{
    public class BasePopupUnlockHelper : BasePopup
    {
        [SerializeField] private Button btnNext;

        public event Action OnClose;

        protected override void Awake()
        {
            base.Awake();
            SetEvent(btnClose, () =>
            {
                OnClose?.Invoke();
                Hide();
            });
            SetEvent(btnNext, () =>
            {
                OnClose?.Invoke();
                Hide();
                //SoundController.PlayClickBtnAudio();
            });
        }
    }
}