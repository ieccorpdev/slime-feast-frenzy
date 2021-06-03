using System;

namespace IEC
{
    public interface IUserLocation
    {
        void RequestInfo(Action<bool> callback);
        string GetAddressInfo();
        string GetCountryName();
        string GetCountryCode();
        string GetRegion();

        bool IsRegionGDPR();
        bool IsHasInfo();
    }
}

