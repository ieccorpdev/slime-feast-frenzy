#if PluginAdverty
using UnityEngine;
using Adverty;

namespace IEC
{
    public class AdvertyModule
    {
        public void Init()
        {
            UserData userData = new UserData(AgeSegment.Unknown, Gender.Unknown);
            AdvertySDK.Init(userData);
        }

        public void SetMainCamera(Camera camera)
        {
            AdvertySettings.SetMainCamera(camera);
        }

        public void UpdateUserData(UserData userData)
        {
            AdvertySDK.UpdateUserData(userData);
        }
    }
}
#endif