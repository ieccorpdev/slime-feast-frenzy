using System;
#if (PluginUnityIAP)
using UnityEngine.Purchasing;
#endif

namespace IEC
{
    public interface IReceiptVerifyService
    {
#if (PluginUnityIAP)
        void CheckReceipt(Product product, bool isRestore, Action<bool> callback);
#endif
    }
}