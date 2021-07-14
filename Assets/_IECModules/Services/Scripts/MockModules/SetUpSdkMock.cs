using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class SetUpSdkMock : ISetUpSdkService
    {
        public SdkState GetState()
        {
            return SdkState.FailInit;
        }

        public void Init(Action<bool> callback)
        {
            if (callback != null)
            {
                callback.Invoke(false);
            }
        }

        public bool IsReady()
        {
            return false;
        }
    }

}