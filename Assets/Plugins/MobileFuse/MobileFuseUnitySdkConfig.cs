// ReSharper disable All
namespace MobileFuseAds
{
    public static class MobileFuseUnitySdkConfig
    {
        public const string UnitySdkVersion   = "1.2.7.0";
        public const string AndroidSdkVersion = "1.2.4.0";
        public const string iOSSdkVersion     = "1.2.7.0";
        public const string MediationAdapter  = "Applovin MAX";

        public new static string ToString()
        {
            return $"MobileFuse Unity SDK v{UnitySdkVersion} {MediationAdapter} - Android {AndroidSdkVersion} - iOS {iOSSdkVersion}";
        }
    }
}
