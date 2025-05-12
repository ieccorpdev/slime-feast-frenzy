// Utils from Luceed Studio - https://luceed.studio

using UnityEngine;

namespace LuceedStudio_Utils
{
    public class CameraDepthMode : MonoBehaviour
    {
        [ContextMenu("Set Camera Depth Mode")]
        private void SetCameraDepthMode()
        {
            if (TryGetComponent(out Camera camera))
            {
                Debug.Log("Setting Camera Depth Texture Mode to: Depth", this);
                camera.depthTextureMode = DepthTextureMode.Depth;
            }
        }

        private void Start()
        {
            SetCameraDepthMode();
        }
    }
}

