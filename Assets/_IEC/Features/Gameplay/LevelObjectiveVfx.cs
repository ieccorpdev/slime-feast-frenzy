using System;
using _IEC.Features.Common.Extension;
using IEC;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Features.Gameplay
{
    public class LevelObjectiveVfx : MonoBehaviour
    {
        [SerializeField] private Image imgIcon;
        [SerializeField] private float baseWidth;
        [SerializeField] private RectTransform rectTransform;

        public RectTransform GetRectTransform()
        {
            return rectTransform;
        }
        public void ReachToTarget(Vector2 targetAnchorPos, Action onComplete)
        {
            
        }

        public void SetSprite(Sprite sprite)
        {
            imgIcon.sprite = sprite;
            imgIcon.rectTransform.ForceToWidth(sprite,baseWidth);
        }

        public void SetAnchorPosition(Vector2 anchorPos)
        {
            rectTransform.anchoredPosition = anchorPos;
        }
        public void Reset()
        {
            transform.localScale = Vector3.one;
        }
    }
}