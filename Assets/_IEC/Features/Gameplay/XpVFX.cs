using IEC;
using System.Collections;
using TMPro;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class XpVFX : _BaseBehaviour
    {
        [SerializeField] private TextMeshProUGUI textUI;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Vector2 anchorPosOffset;
        [SerializeField] private float time = 1f;
        [SerializeField] private Color targetColor;
        [SerializeField] private Color defaultColor;

        private Coroutine showCoroutine;

        public void SetAnchorPosition(Vector2 position)
        {
            rectTransform.anchoredPosition = position;
        }

        public void SetTargetColor(Color color)
        {
            targetColor = color;
        }

        public void Show(int xp, float size = 1f)
        {
            textUI.text = $"+{xp}";
            gameObject.SetActive(true);
            rectTransform.localScale = Vector3.one * size;

            // Random start and end positions
            var startPos = new Vector2(Random.Range(-50f, 50f), Random.Range(-20f, 20f));
            var endPos = startPos + new Vector2(0f, Random.Range(150f, 250f));

            rectTransform.anchoredPosition = startPos;
            textUI.color = defaultColor;

            // Stop previous coroutine if needed
            if (showCoroutine != null)
            {
                StopCoroutine(showCoroutine);
            }

            showCoroutine = StartCoroutine(AnimateXP(startPos, endPos, time));
        }

        private IEnumerator AnimateXP(Vector2 startPos, Vector2 endPos, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // You can implement easing manually if needed (e.g., Mathf.SmoothStep or custom curve)
                rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
                textUI.color = Color.Lerp(defaultColor, targetColor, t);

                yield return null;
            }

            gameObject.Recycle();
            showCoroutine = null;
        }

        private void OnDisable()
        {
            if (showCoroutine != null)
            {
                StopCoroutine(showCoroutine);
                showCoroutine = null;
            }
        }

        public void Reset()
        {
            gameObject.SetActive(false);
            rectTransform.localScale = Vector3.one;
            textUI.color = defaultColor;
            targetColor = defaultColor;
        }
    }
}
