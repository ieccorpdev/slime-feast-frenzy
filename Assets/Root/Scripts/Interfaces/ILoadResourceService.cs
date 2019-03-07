using System;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public interface ILoadResourceService
    {
        Texture2D LoadTexture2D(string path);
    }
}
