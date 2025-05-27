using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ChompOffProgressBarUI : _BaseBehaviour
    {
        public LayoutElement playerProgressImg;
        public LayoutElement opponentProgressImg;
        public RectTransform connectorImage;
        
        public void UpdateProgressBarUI(EventPlayerData player, EventPlayerData opponent)
        {
            UpdateProgressBarUI(
                player?.progress ?? 0, 
                opponent?.progress ?? 0);
        }

        private void UpdateProgressBarUI(int playerProgress, int opponentProgress)
        {
            float total = playerProgress + opponentProgress;
            if (total == 0) total = 1;
            
            playerProgressImg.flexibleWidth = playerProgress / total * 100;
            opponentProgressImg.flexibleWidth = opponentProgress / total * 100;
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(playerProgressImg.transform.parent.GetComponent<RectTransform>());
            
            // Get world corners of imageA
            Vector3[] corners = new Vector3[4];
            playerProgressImg.GetComponent<RectTransform>().GetWorldCorners(corners);
            Vector3 rightEdgeWorld = corners[3]; // Top Right

            // Convert world position to local position relative to the parent canvas or connector's parent
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                connectorImage.parent as RectTransform,
                RectTransformUtility.WorldToScreenPoint(null, rightEdgeWorld),
                null,
                out localPoint
            );

            // Set connector position
            connectorImage.anchoredPosition = new Vector2(localPoint.x, connectorImage.anchoredPosition.y);
        }
    }
}
