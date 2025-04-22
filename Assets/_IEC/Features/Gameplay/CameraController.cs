using System;
using System.Collections;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay.ObserverEventData;
using IEC;
using IEC.Common.Event;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class CameraController : _BaseBehaviour
    {

        public void Awake()
        {
            // this.RegisterListener(EventList.GamePlay.UPDATE_CAMERA_TARGET,OnUpdateCameraTarget);
            // if (mainCamera == null)
            // {
            //     mainCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            // }
        }

        public void OnDestroy()
        {
            this.UnregisterListener(EventList.GamePlay.UPDATE_CAMERA_TARGET,OnUpdateCameraTarget);
        }

        private void OnUpdateCameraTarget(object param)
        {
            // var data = (UpdateCameraTarget) param;
            // if (data == null) return;
            // if (mainCamera == null)
            // {
            //     mainCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            // }
            // mainCamera.Follow = data.target;
            // var framingTransposer = mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            // if (framingTransposer == null) return;
            // framingTransposer.m_CameraDistance = data.cameraDistance;
        }
        
        
        public void OnUpdateCameraTargetEditMode(object param)
        {

        }
    }
}