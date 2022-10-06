using UnityEngine;

namespace MobileFuseAds
{
    public static class MobileFuseSettings
    {
        public static void SetOverride(string setting, string value)
        {
            MobileFuse.SetSettingOverride(setting, value);
        }
    }
}