using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class TexturePreProcess : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        TextureImporter texture = (TextureImporter)assetImporter;
        texture.textureCompression = TextureImporterCompression.Uncompressed;
    }
}
