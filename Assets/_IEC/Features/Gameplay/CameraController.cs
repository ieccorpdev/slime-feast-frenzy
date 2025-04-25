using System;
using System.Collections;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay.ObserverEventData;
using IEC;
using IEC.Common.Event;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class CameraController : MonoBehaviour
    {

        [SerializeField] private Camera cameraFollower;
        [SerializeField] Transform target;
        private Vector3 cameraOffset;
        private float fOVOffset = 5;
        [SerializeField]
        private HoleConfig m_settings;
        public void Awake()
        {
            this.RegisterListener(EventList.GamePlay.UPDATE_CAMERA_TARGET,OnUpdateCameraTarget);

            cameraOffset = cameraFollower.transform.position - target.position;
        }

        public void OnDestroy()
        {
            //this.UnregisterListener(EventList.GamePlay.UPDATE_CAMERA_TARGET,OnUpdateCameraTarget);
        }

        private void FixedUpdate()
        {
            cameraFollower.transform.position = target.position + cameraOffset;
        }
        private float CalculateIndices(float baseIndices, float level, float bias)
        {
            return baseIndices + (level - 1) * bias;
        }
        private void OnUpdateCameraTarget(object param)
        {
             var data = (UpdateCameraTarget) param;
             if (data == null) return;
             float d1 = CalculateIndices(m_settings.BaseCameraDistance,data.curLevel,m_settings.CameraDistanceIncreasePerLevel);
             float d2 = CalculateIndices(m_settings.BaseCameraDistance,data.nextLevel,m_settings.CameraDistanceIncreasePerLevel);
             float fov1 = cameraFollower.fieldOfView;
             float fov2 = CalculateFOVbyDistance(d2, d1, fov1);
             StartCoroutine(IE_InscreseFOV(fov2, 0.5f));
             // if (mainCamera == null)
             // {
             //     mainCamera = GetComponentInChildren<CinemachineVirtualCamera>();
             // }
             // mainCamera.Follow = data.target;
             // var framingTransposer = mainCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
             // if (framingTransposer == null) return;
             // framingTransposer.m_CameraDistance = data.cameraDistance;
        }

        IEnumerator IE_InscreseFOV(float targetFOV, float time)
        {
            targetFOV += fOVOffset;
            float startFOV = cameraFollower.fieldOfView;
            float elapsed = 0f;

            while (elapsed < time)
            {
                elapsed += Time.deltaTime;
                cameraFollower.fieldOfView = Mathf.Lerp(startFOV, targetFOV, elapsed / time);
                yield return null;
            }

            cameraFollower.fieldOfView = targetFOV; 
        }
   
        private float CalculateFOVbyDistance(float d1, float d2, float fov1)
        {
            float fov1Rad = fov1 * Mathf.Deg2Rad;
            float tanHalfFOV1 = Mathf.Tan(fov1Rad / 2f);
    
            float tanHalfFOV2 = tanHalfFOV1 * (d1 / d2);
            float fov2Rad = 2f * Mathf.Atan(tanHalfFOV2);
    
            return fov2Rad * Mathf.Rad2Deg;
        }


        
        
        public void OnUpdateCameraTargetEditMode(object param)
        {

        }
    }
}