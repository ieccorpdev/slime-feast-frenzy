using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Root
{
    public class BinaryLoadResource : ILoadResourceService
    {
        public Texture2D LoadTexture2D(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            TextAsset bindata = Resources.Load(path) as TextAsset;

            if(bindata == null)
            {
                return null;
            }

            Texture2D tex = new Texture2D(1, 1);
            tex.LoadImage(bindata.bytes);

            return tex;
        }
    }
}
