using System;

namespace Root
{
    public interface IUserLocation
    {
        void RequestInfo(Action<bool> callback);
        string GetAddressInfo();
        string GetCountryName();
        string GetCountryCode();
        string GetRegion();

        bool IsRegionGDPR();
    }
}

