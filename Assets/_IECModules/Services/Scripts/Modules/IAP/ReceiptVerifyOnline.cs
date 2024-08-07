#if (PluginUnityIAP && PluginReceiptVerify)
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Purchasing;

namespace IEC
{
    public class ReceiptVerifyOnline : ReceiptVerifyOffline
    {
        private string urlCheck;
        private MonoBehaviour webBehaviour;
        private IJsonService jsonConverter;

        public class PurchaseReceipt
        {
            public string Store;
            public string TransactionID;
            public string Payload;
        }

        public class VerifyResult
        {
            public int status;
            public bool isValid;
        }

        public ReceiptVerifyOnline(string urlCheck, MonoBehaviour webBehaviour, IJsonService jsonConverter)
        {
            this.urlCheck = urlCheck;
            this.webBehaviour = webBehaviour;
            this.jsonConverter = jsonConverter;
        }

        public override void CheckReceipt(Product product, bool isRestore, Action<bool> callback)
        {
            bool validPurchase = IsValidOffline(product);

            if (validPurchase)
            {
                webBehaviour.StartCoroutine(IECheckReceiptOnline(product, callback));
            }
            else
            {
                callback(false);
            }
        }

        private IEnumerator IECheckReceiptOnline(Product product, Action<bool> callback)
        {
            var productId = product.definition.id;
            var receipt = jsonConverter.FromJson<PurchaseReceipt>(product.receipt);
            var url = urlCheck;
            Debug.Log("CheckReceiptOnline url : " + url);
            Debug.Log("CheckReceiptOnline payload : " + receipt.Payload);

            var form = new WWWForm();
            form.AddField("payload", receipt.Payload);
            form.AddField("transaction", product.transactionID);
            form.AddField("product", product.definition.storeSpecificId);
            form.AddField("bundleID", Application.identifier);

            var www = new WWW(url, form);
            yield return www;

            Debug.Log("CheckReceiptOnline www response : " + www.text);

            bool isValid = false;
            if (www.error == null)
            {
                var result = jsonConverter.FromJson<VerifyResult>(www.text);
                if (result != null)
                {
                    if (result.isValid)
                    {
                        isValid = true;
                    }
                    else
                    {
                        Debug.LogError("CheckReceiptOnline error status : " + result.status);
                    }
                }
                else
                {
                    Debug.LogError("CheckReceiptOnline error response json");
                }
            }
            else
            {
                Debug.LogError("CheckReceiptOnline www error : " + www.error);
            }

            callback(isValid);
        }
    }
}
#endif