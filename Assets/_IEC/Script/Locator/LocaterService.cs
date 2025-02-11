using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IEC;
public static partial class Locator
{
    public static class Service
    {
        public static PopupSetting PopupSetting => Pool.Get<PopupSetting>();
        public static ScreenLoading ScreenLoading => Pool.Get<ScreenLoading>();
        public static MoneyPanel MoneyPanel => Pool.Get<MoneyPanel>();
        public static GroupFxCoin GroupFxCoin => Pool.Get<GroupFxCoin>();
        public static PopupPurchaseSuccess PopupPurchaseSuccess => Pool.Get<PopupPurchaseSuccess>();
        public static PopupPurchaseFail PopupPurchaseFail => Pool.Get<PopupPurchaseFail>();
        public static PopupRestorePurchaseFail PopupRestorePurchaseFail => Pool.Get<PopupRestorePurchaseFail>();
        public static PopupRestorePurchaseSuccess PopupRestorePurchaseSuccess => Pool.Get<PopupRestorePurchaseSuccess>();

    }
}
