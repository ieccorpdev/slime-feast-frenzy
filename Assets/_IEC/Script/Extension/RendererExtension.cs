using UnityEngine;

namespace _IEC.Features.Common.Extension
{
    public static class RendererExtension
    {
        public static bool IsInClipView(this Renderer renderer, Camera camera)
        {
            if (camera == null) return false;
            Bounds bounds = renderer.bounds;
            Vector3 screenPoint = camera.WorldToViewportPoint(bounds.center);
            
            if (screenPoint.z > 0 
                && screenPoint.x > 0
                && screenPoint.x < 1 
                && screenPoint.y > 0 
                && screenPoint.y < 1)
            {
                return true;
            }
            return false;
        }
    }
}