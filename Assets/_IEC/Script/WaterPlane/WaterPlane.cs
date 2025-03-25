using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay;
using UnityEngine;

namespace IEC
{
    public class WaterPlane : _BaseBehaviour
    {
        [SerializeField] private MeshRenderer waterMeshRenderer;
        [SerializeField] private WaterPlaneThemeConfig waterThemeConfig;

        private void Awake()
        {
            this.RegisterListener(EventList.GamePlay.ON_LEVEL_LOADED, UpdateWaterPlaneRenderer);
        }

        private void OnDestroy()
        {
            this.UnregisterListener(EventList.GamePlay.ON_LEVEL_LOADED, UpdateWaterPlaneRenderer);
        }

        void UpdateWaterPlaneRenderer(object param)
        {
            //var levelData = (EventLevelLoad)param;
            //var themeId = levelData.themeId;
            //var waterConfig = waterThemeConfig.waterConfigs.Find(x => x.theme == themeId);
            //if (waterConfig == null)
            //{
            //    Debug.LogError($"Missing material setup for theme {themeId}");
            //    waterConfig = waterThemeConfig.waterConfigs.Find(x => x != null);
            //}
            //waterMeshRenderer.material = waterConfig.waterMaterial;
        }
    }
}