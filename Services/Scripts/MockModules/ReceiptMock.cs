using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if (PluginUnityIAP)
using UnityEngine.Purchasing;
#endif

namespace IEC
{
    public class ReceiptMock : IReceiptVerifyService
    {
#if (PluginUnityIAP)
        public void CheckReceipt(Product product, Action<bool> callback)
        {
            callback?.Invoke(true);
        }
#endif
    }
}
