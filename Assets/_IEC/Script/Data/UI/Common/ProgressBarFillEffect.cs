using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ProgressBarFillEffect : _BaseBehaviour
    {
        public float fillAnimDuration = 1f;
        public Image progressFillImage;
        public TextMeshProUGUI progressText;
        public ParticleSystem sparkleEffect;

        private void Awake()
        {
            progressFillImage.fillAmount = 0;
            
            if (sparkleEffect != null)
            {
                sparkleEffect.gameObject.SetActive(false);
            }
        }

        public void SetProgressText(string text)
        {
            progressText.text = text;
        }

        public void SetProgressFillAmount(float fillAmount)
        {
            progressFillImage.fillAmount = fillAmount;
        }

        public void DoFillAnim(float fillAmount)
        {
            if (Mathf.Abs(fillAmount - progressFillImage.fillAmount) < 0.1f)
                return;
            
            if(sparkleEffect != null)
                sparkleEffect.gameObject.SetActive(true);
            
            var progressBarRect = progressFillImage.GetComponent<RectTransform>();
            
            // Get the width of the progress bar
            float barWidth = progressBarRect.rect.width;

            // Calculate min and max local X positions (relative to the progress bar)
            float minX = -barWidth * 0.5f;
            float maxX = barWidth * 0.5f;

            RectTransform sparkleRect = null;

            if (sparkleEffect != null)
            {
                sparkleRect = sparkleEffect.GetComponent<RectTransform>();
                sparkleEffect.Play();
            }

            progressFillImage.DOFillAmount(fillAmount, fillAnimDuration)
                .OnUpdate(
                    () =>
                    {
                        if (sparkleRect != null)
                        {
                            float newX = Mathf.Lerp(minX, maxX, progressFillImage.fillAmount);
                            sparkleRect.localPosition = new Vector2(newX, sparkleRect.anchoredPosition.y);
                        }
                    })
                .OnComplete(
                    () =>
                    {
                        if (sparkleEffect != null)
                        {
                            sparkleEffect.Stop();
                            sparkleEffect.gameObject.SetActive(false);
                        }
                    });
        }
    }
}
