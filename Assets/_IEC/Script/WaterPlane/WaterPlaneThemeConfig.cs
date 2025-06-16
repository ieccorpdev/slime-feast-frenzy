using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _IEC.Features.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Gameplay/WaterThemeConfig")]
    public class WaterPlaneThemeConfig : ScriptableObject
    {
        public ThemeConfig themeConfig;
        public List<WaterConfig> waterConfigs = new List<WaterConfig>();

        private void OnValidate()
        {
            if (themeConfig == null) return;
            foreach (var config in waterConfigs)
            {
                config.SetThemeConfig(themeConfig);
            }
        }
    }

    [Serializable]
    public class WaterConfig
    {
        [ValueDropdown(nameof(GetAllThemes))]
        public string theme;
        public Material waterMaterial;

        private ThemeConfig _themeConfig;

        public void SetThemeConfig(ThemeConfig themeConfig)
        {
            _themeConfig = themeConfig;
        }
        
        private IEnumerable GetAllThemes()
        {
            return _themeConfig.groundThemeConfigModels.Select(x => x.themeId);
        }
    }
}