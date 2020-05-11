using System;
using System.Collections.Generic;
namespace IEC
{
    public enum SdkState { NotInit, Initializing, Initialized, FailInit }

    public interface ISetUpSdkService
    {
        void Init(Action<bool> callback);
        SdkState GetState();
        bool IsReady();
    }
}