using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace IEC
{
    public class PopupPurchaseSuccess : BasePopup
    {
        public string idPack;
        public Button btnContinue;
        public Action<List<ResourceType>> onClose;
        private List<ResourceType> resourceCallBack = new List<ResourceType>();

        public override void Show()
        {
            base.Show();
            DOVirtual.DelayedCall(3f, () =>
            {
                ClosePopup();
                Hide();
            });
        }

        public PopupPurchaseSuccess AddActionClose(Action<List<ResourceType>> actionCallback)
        {
            onClose = actionCallback;
            return this;
        }

        public PopupPurchaseSuccess SetResourceCallBack(List<ResourceType> resourceType)
        {
            resourceCallBack.Clear();
            resourceCallBack.AddRange(resourceType);
            return this;
        }

        protected override void Awake()
        {
            base.Awake();
            SetEvent(btnContinue, () =>
            {
                ClosePopup();
                Hide();
            });
            SetEvent(btnClose, () =>
            {
                ClosePopup();
                Hide();
            });
        }

        void ClosePopup()
        {
            onClose?.Invoke(resourceCallBack);
            resourceCallBack.Clear();
        }

        public override void Hide()
        {
            base.Hide();
            if (Locator.Main.HomeScreenTab != null)
            {
                Locator.Main.HomeScreenTab.UpdateMainHome();
            }
        }

        // public static ProductCatalogItem GetProductById(string id)
        // {
        //     var productCatalog = ProductCatalog.LoadDefaultCatalog();
        //     return productCatalog.allProducts.FirstOrDefault(x => x.id == id);
        // }
    }
}