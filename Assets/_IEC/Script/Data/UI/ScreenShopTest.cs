using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ScreenShopTest : ScreenUI
    {
        public Button btnTestPurchase;
        public const string TEST_PRODUCT_ID = "com.iec.test.product";

        private void Start()
        {
            btnTestPurchase.onClick.AddListener(() =>
            {
                Services.IAP().PurchaseProduct(TEST_PRODUCT_ID);
            });
        }

        public void SetCallbackIAP()
        {
            Services.IAP().SetReceiveProductCallback(TEST_PRODUCT_ID, ReceivePurchaseItem);
        }

        void ReceivePurchaseItem(bool isRestore)
        {
            Debug.LogWarningFormat("ReceivePurchaseItem {0} {1}", TEST_PRODUCT_ID, isRestore);
        }
    }
}