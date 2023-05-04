using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupNotification : BasePopup
    {
        public RectTransform contentNoti;
        public TextMeshProUGUI txtContent;
        private bool isShowing;

        public override void Show()
        {
            if (isShowing)
                return;
            isShowing = true;
            contentNoti.gameObject.SetActive(true);

            contentNoti.localPosition = Vector3.zero;
            Image imgContent = contentNoti.GetComponent<Image>();
            imgContent.DOFade(0, 0);
            base.Show();
            Sequence sequence = DOTween.Sequence();

            sequence.Join(imgContent.DOFade(1, 0.5f));
            sequence.Join(contentNoti.DOMoveY(1, 0.5f));
            sequence.Join(contentNoti.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 1.2f).SetEase(Ease.OutQuad));
            sequence.OnComplete(delegate
            {
                Hide();
            });
        }

        public override void Hide()
        {
            base.Hide();
            isShowing = false;
            contentNoti.localScale = new Vector3(0, 0, 0);
        }

        public PopupNotification SetContent(string content)
        {
            txtContent.text = content;
            return this;
        }
    }
}