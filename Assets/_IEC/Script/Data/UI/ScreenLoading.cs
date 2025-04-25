using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using IEC.Common.Event;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace IEC
{
    public class ScreenLoading : ScreenUI
    {
        [SerializeField] private Image m_Progress;

        public void Awake()
        {
            EncryptedAssetModule.LoadPackage(this);
        }

        public void ShowLoading(float timeShow, Action loadingDoneCallback = null,Action onLoadingCallBack = null)
        {
            base.Show();
            m_Progress.fillAmount = 0;
            m_Progress.DOFillAmount(1F, timeShow + 0.1f).SetEase(Ease.OutQuad)
                .OnComplete(Hide);
            DOVirtual.DelayedCall(timeShow, () =>
            {
                loadingDoneCallback?.Invoke();
            });
        }
        public void ShowLoading()
        {
            base.Show();
            m_Progress.fillAmount = 0;
        }
    }
    
}