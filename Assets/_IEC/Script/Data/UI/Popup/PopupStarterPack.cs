using _IEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupStarterPack : BasePopup
    {
        public Action actionOneTimeClosePopup;
        
        public SpineAnimOT anim;
        
        public Button btnX;

        public BundlePackUI configBundle;
        protected override void Awake()
        {
            base.Awake();
            configBundle.OnPurchaseSuccess += () =>
            {
                PlayerData.isBoughtStarterPack = true;
                //Services.SetActiveBanner(AdsEnum.BANNER_BOTTOM, false);//Disable banner Ads
                Locator.Main.HomeScreenTab.UpdateMainHome();
                Hide();
                Services.SaveData();
            };
            SetEvent(btnX, OnHideClick);
        }

        public override void Show()
        {
            base.Show();
            anim.SetAnimation("1-Starter_Pack_In", false, () =>
            {
                anim.SetAnimation("2-Starter_Pack_Idle", true);
            });
        }

        public override void Hide()
        {
            base.Hide();
            if (actionOneTimeClosePopup != null)
            {
                actionOneTimeClosePopup.Invoke();
                actionOneTimeClosePopup = null;
            }
        }
    }
}
