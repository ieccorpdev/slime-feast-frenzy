using DG.Tweening;
using TMPro;
using UnityEngine;

namespace IEC
{
    public class ToastNotify : BaseToast
    {
        public RectTransform RectTrans => SingleGet<RectTransform>();
        public CanvasGroup CanvasGr => SingleGet<CanvasGroup>();
        public TextMeshProUGUI TxtContent => SingleGetChild<TextMeshProUGUI>();

        public override void Show()
        {
            if (IsShowing) 
                return;

            base.Show();

            RectTrans.localPosition = Vector3.zero;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(CanvasGr.DOFade(0, 0));
            sequence.Append(CanvasGr.DOFade(1, 0.5f).SetEase(Ease.InOutQuad));
            sequence.Join(RectTrans.DOMoveY(1, 0.5f).SetEase(Ease.InOutQuad));
            sequence.Join(RectTrans.transform.DOScale(1.1f * Vector3.one, 1.2f).SetEase(Ease.OutQuad));
            sequence.OnComplete(delegate
            {
                Hide();
            });
        }

        public ToastNotify SetContent(string content)
        {
            TxtContent.text = content;
            return this;
        }

        public void ShowAdsNotReady()
        {
            SetContent("The ads is not ready").Show();
        }
    }
}