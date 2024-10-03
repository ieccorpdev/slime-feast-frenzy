using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;

public static partial class Locator
{
    public static class Main
    {
        public static ScreenMain ScreenMain => Pool.Get<ScreenMain>();
    }
}
