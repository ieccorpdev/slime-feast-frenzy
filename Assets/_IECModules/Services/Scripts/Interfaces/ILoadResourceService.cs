using System;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public interface ILoadResourceService
    {
        Texture2D LoadTexture2D(string path);
    }
}
