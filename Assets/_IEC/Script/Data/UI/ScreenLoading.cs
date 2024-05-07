using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ScreenLoading : _BaseBehaviour
    {
        public Image progressBar;
        public Action hideAction;

        public void Start()
        {
            gameObject.SetActive(true);
            progressBar.fillAmount = 0f;
            Fill(Services.config.remoteConfigTimeout);
        }

        public void Hide()
        {
            Fill(0.3f);
        }

        private void Fill(float time)
        {
            if(!gameObject.activeSelf) return;
            progressBar.DOKill(false);
            progressBar.DOFillAmount(1f, time).SetEase(Ease.Linear)
                .OnComplete(()=>
                {
                    // UI.ShowScreen(UI.ScreenInGame);
                    gameObject.SetActive(false);
                    hideAction?.Invoke();
                });
        }
    }
}