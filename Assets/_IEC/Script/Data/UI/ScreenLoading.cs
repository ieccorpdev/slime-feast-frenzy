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
            // EncryptedAssetModule.LoadPackage(this);
        }

        public void ShowLoading(float timeShow, Action loadingDoneCallback = null, Action onLoadingCallBack = null)
        {
            base.Show();
            m_Progress.fillAmount = 0;
            onLoadingCallBack?.Invoke();
            // m_Progress.DOFillAmount(1F, timeShow + 0.1f).SetEase(Ease.OutQuad)
            //     .OnComplete(() =>
            //     {
            //         Hide();
            //         DOVirtual.DelayedCall(0.5f, () => { loadingDoneCallback?.Invoke(); });
            //     });
            StartCoroutine(AnimateProgressFill(timeShow + 0.1f, Hide));

            // DOVirtual.DelayedCall(timeShow, () =>
            // {
            //     loadingDoneCallback?.Invoke();
            // });
        }

        IEnumerator AnimateProgressFill(float duration, Action onComplete)
        {
            float time = 0f;
            float startFill = m_Progress.fillAmount;
            float endFill = 1f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / duration);
                m_Progress.fillAmount = Mathf.Lerp(startFill, endFill, Mathf.SmoothStep(0f, 1f, t)); // OutQuad-ish
                yield return null;
            }

            m_Progress.fillAmount = 1f;

            Hide();
            StartCoroutine(DelayedInvoke(0.5f, (Action)null));
        }

        IEnumerator DelayedInvoke(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }

   


        public void ShowLoading()
        {
            base.Show();
            m_Progress.fillAmount = 0;
        }
    }
}