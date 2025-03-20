using System.Collections;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay.ObserverEventData;
using Cinemachine;
using IEC;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class CameraController : _BaseBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera mainCamera;
        [SerializeField] private CinemachineVirtualCamera homeCamera;

        public void Awake()
        {
            this.RegisterListener(EventList.GamePlay.UPDATE_CAMERA_TARGET,OnUpdateCameraTarget);
        }
        
        private void OnUpdateCameraTarget(object param)
        {
            var data = (UpdateCameraTarget) param;
            if (data == null) return;
            mainCamera.Follow = data.target;
            var framingTransposer = mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (framingTransposer == null) return;
            framingTransposer.m_CameraDistance = data.cameraDistance;
        }
    }
}