using System.Collections;
using System.Collections.Generic;
using _IEC;
using IEC;
using UnityEngine;

public static partial class Locator
{
    public static class Main
    {
        public static ScreenMain ScreenMain => Pool.Get<ScreenMain>();
        //"Home Main"
        public static HomeScreenTab HomeScreenTab => Pool.Get<HomeScreenTab>();
        public static PopupAvatar PopupAvatar=> Pool.Get<PopupAvatar>();
        public static PopupLuckyWheel PopupLuckyWheel=> Pool.Get<PopupLuckyWheel>();
        public static PopupNoAds PopupNoAds=> Pool.Get<PopupNoAds>();
        public static PopupStarterPack PopupStarterPack=> Pool.Get<PopupStarterPack>();
        public static PopupPiggyBank PopupPiggyBank=> Pool.Get<PopupPiggyBank>();
     } 
}
