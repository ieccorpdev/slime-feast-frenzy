using DG.Tweening;
using IEC;
using TMPro;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class XpVFX : _BaseBehaviour
    {
        [SerializeField] private TextMeshProUGUI textUI;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Vector2 anchorPosOffset;
        [SerializeField] private Ease easePosition;
        [SerializeField] private Ease easeColor;
        [SerializeField] private float time;
        [SerializeField] private Color targetColor;
        [SerializeField] private Color defaultColor;
        private string TweenID => gameObject.GetInstanceID() + "_fly";
        public void SetAnchorPosition(Vector2 position)
        {
            rectTransform.anchoredPosition = position;
        }
        public void Show(int xp)
        {
            textUI.text = $"+{xp}";
            gameObject.SetActive(true);
            //var currentAnchorPos = rectTransform.anchoredPosition;
            //var targetAnchorPos = anchorPosOffset + currentAnchorPos;

            rectTransform.localScale = Vector3.one;

            var currentAnchorPos = new Vector2(Random.Range(-50f, 50f), Random.Range(-20f, 20f));

            rectTransform.anchoredPosition = currentAnchorPos;
            var targetAnchorPos = currentAnchorPos;
            targetAnchorPos.y += Random.Range(150f, 250f);

            DOTween.Sequence().Append(rectTransform.DOAnchorPos(targetAnchorPos, time).SetEase(easePosition))
                .Join(textUI.DOColor(targetColor,time).SetEase(easeColor))
                //.Join(transform.DOScale(Vector3.one * 0.5f,time).SetEase(easeColor))
                .AppendCallback(() => { gameObject.Recycle(); });
        }

        private void OnDisable()
        {
            DOTween.Kill(TweenID);
        }

        public void Reset()
        {
            gameObject.SetActive(false);
            rectTransform.localScale = Vector3.one;
            textUI.color = defaultColor;
        }
    }
}