using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class SharkRacePlayerDataUI : GameEventPlayerDataUI
    {
        public Image filledImage;
        public Image playerDisplayImage;
        public Image shadow;

        public void UpdateUI(float progressPercent)
        {
            filledImage.fillAmount = progressPercent;
            float fillAmount = filledImage.fillAmount;
            RectTransform targetRect = filledImage.GetComponent<RectTransform>();
            Vector2 newPos = Vector2.zero;

            // Calculate based on fill method
            switch (filledImage.fillMethod)
            {
                case Image.FillMethod.Horizontal:
                    float xDir = (filledImage.fillOrigin == 0) ? 1 : -1;
                    newPos = new Vector2((fillAmount - 0.5f) * 2f * xDir * targetRect.rect.width / 2f, 0);
                    break;

                case Image.FillMethod.Vertical:
                    float yDir = (filledImage.fillOrigin == 0) ? 1 : -1;
                    newPos = new Vector2(0, (fillAmount - 0.5f) * 2f * yDir * targetRect.rect.height / 2f);
                    break;

                // Optional: handle Radial (90/180/360) separately if needed
            }

            playerDisplayImage.transform.localPosition = newPos;
        }

        protected override void UpdatePlayerRank()
        {
            base.UpdatePlayerRank();
            shadow.sprite = rewardInfoButtonImage.sprite;
        }
    }
}
