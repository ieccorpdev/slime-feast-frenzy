using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IEC;
public static partial class Locator
{
    public static PopupSetting PopupSetting => Pool.Get<PopupSetting>();
    public static ScreenLoading ScreenLoading => Pool.Get<ScreenLoading>();
}
