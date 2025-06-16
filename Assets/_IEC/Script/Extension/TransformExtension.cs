using UnityEngine;

namespace _IEC.Features.Common.Extension
{
    public static class TransformExtension
    {
        public static void ForceToWidth(this RectTransform rectTransform,Sprite sprite, float width)
        {
            if (sprite == null) return;
            var spriteRect = sprite.rect;
            var nativeWidth = spriteRect.width;
            var nativeHeight = spriteRect.height;
            var delta = nativeHeight / nativeWidth;
            rectTransform.sizeDelta = new Vector2(width, width * delta);
        }  
        public static Vector2 GetLocalAnchorPos(this Vector3 position,RectTransform parentTransform, Camera cam3D, Camera cam2D )
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam3D, position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentTransform, screenPos, cam2D, out Vector2 localPoint);
            return localPoint;
        }
        
        public static void ClearAllChild(this Transform transform)
        {
            var childCount = transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                Object.Destroy(child.gameObject);
            }
        }
    }
}