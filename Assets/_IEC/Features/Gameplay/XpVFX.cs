using System.Collections;
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
        [SerializeField] private AnimationCurve  easePosition;
        [SerializeField] private AnimationCurve  easeColor;
        [SerializeField] private float time;
        [SerializeField] private Color targetColor;
        [SerializeField] private Color defaultColor;

        public Vector2 targetAnchorPos;

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

            // DOTween.Sequence().Append(rectTransform.DOAnchorPos(targetAnchorPos, time).SetEase(easePosition))
            //     .Join(textUI.DOColor(targetColor, time).SetEase(easeColor))
            //     //.Join(transform.DOScale(Vector3.one * 0.5f,time).SetEase(easeColor))
            //     .AppendCallback(() => { gameObject.Recycle(); });
            StartAnimation();
        }

        #region Replace for Luna

        public void StartAnimation()
        {
            StartCoroutine(AnimateSequence());
        }

        private IEnumerator AnimateSequence()
        {
            yield return StartCoroutine(MoveRectTransform(rectTransform, targetAnchorPos, time, easePosition));
            yield return StartCoroutine(FadeTextColor(textUI, targetColor, time, easeColor));

            // Optional scale animation
            // yield return StartCoroutine(ScaleTransform(transform, Vector3.one * 0.5f, time, easeColor));

            // Callback like AppendCallback
            gameObject.Recycle(); // assuming Recycle() is your pooling logic
        }

        private IEnumerator MoveRectTransform(RectTransform rect, Vector2 target, float duration, AnimationCurve ease)
        {
            Vector2 start = rect.anchoredPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                rect.anchoredPosition = Vector2.LerpUnclamped(start, target, ease.Evaluate(t));
                elapsed += Time.deltaTime;
                yield return null;
            }

            rect.anchoredPosition = target;
        }

        private IEnumerator FadeTextColor(TextMeshProUGUI text, Color targetColor, float duration, AnimationCurve ease)
        {
            Color startColor = text.color;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                text.color = Color.LerpUnclamped(startColor, targetColor, ease.Evaluate(t));
                elapsed += Time.deltaTime;
                yield return null;
            }

            text.color = targetColor;
        }

// Optional if you want to use scale
        private IEnumerator ScaleTransform(Transform target, Vector3 targetScale, float duration, AnimationCurve ease)
        {
            Vector3 startScale = target.localScale;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                target.localScale = Vector3.LerpUnclamped(startScale, targetScale, ease.Evaluate(t));
                elapsed += Time.deltaTime;
                yield return null;
            }

            target.localScale = targetScale;
        }

        #endregion

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