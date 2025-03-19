using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IEC;
public static partial class Locator
{
    public static class Game
    {
        public static ScreenInGame ScreenInGame => Pool.Get<ScreenInGame>();
        public static ScreenLevelWin ScreenLevelWin => Pool.Get<ScreenLevelWin>();
        public static ScreenLevelWinHard ScreenLevelWinHard => Pool.Get<ScreenLevelWinHard>();
        public static ScreenLevelLose ScreenLevelLose => Pool.Get<ScreenLevelLose>();
        public static PopupSettingIngame PopupSettingIngame => Pool.Get<PopupSettingIngame>();
    }
}