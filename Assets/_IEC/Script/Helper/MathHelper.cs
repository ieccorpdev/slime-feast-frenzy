using System;
using System.Collections.Generic;
using UnityEngine;

public class MathHelper
{
    public static int ClampInt(int value, int min, int max)
    {
		value = (value < min) ? min : value;
        value = (value > max) ? max : value;
		return value;
    }
}