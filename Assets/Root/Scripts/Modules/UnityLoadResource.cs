using System;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    public class UnityLoadResource : ILoadResourceService
    {
        public Texture2D LoadTexture2D(string path)
        {
            if(string.IsNullOrEmpty(path))
            {
                return null;
            }
            return Resources.Load<Texture2D>(path);
        }
    }
}
