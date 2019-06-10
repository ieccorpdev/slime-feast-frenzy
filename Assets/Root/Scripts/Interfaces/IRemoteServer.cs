using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public interface IRemoteServer
    {
        bool GetBoolValue(RemoteConfig config);
        int GetIntValue(RemoteConfig config);
        string GetStringValue(RemoteConfig config);
    }
}
