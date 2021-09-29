using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/Color Config")]
    public class ColorConfig : ScriptableObject
    {
        //Water
        public Color[] waterColor;
        public Color targetColorDefault;

        public Material[] targetMats;
        public Material[] targetGradientMats;
        public Material targetDefaultMat;

        public Color[] colorBSP;
        public Material[] matBSP;
        public Material[] matBSPGradient;

        public Material treeGrayMat, trunkGrayMat;

        public Color GetWaterClassicColor(bool isMultiClassicColor, int color)
        {
            if (isMultiClassicColor)
            {
                return colorBSP[color];
            }
            else
            {
                return targetColorDefault;
            }
        }

        public Color GetWaterColor(int color)
        {
            var colorIndex = Mathf.Clamp(color - 1, 0, waterColor.Length - 1);
            return waterColor[colorIndex];
        }

        public Material GetWaterClassicMat(bool isMultiClassicColor, bool isGradient, int color)
        {
            if (isMultiClassicColor)
            {
                if (isGradient)
                {
                    return matBSPGradient[color];
                }
                else
                {
                    return matBSP[color];
                }
            }
            else
            {
                return targetDefaultMat;
            }
        }

        public Material GetWaterMat(bool isGradient, int color)
        {
            var colorIndex = Mathf.Clamp(color - 1, 0, targetMats.Length - 1);

            if (isGradient)
            {
                return targetGradientMats[colorIndex];
            }
            else
            {
                return targetMats[colorIndex];
            }

        }

        public List<WaterEffectData> listWaterEffectConfig;
    }

    [System.Serializable]
    public class WaterEffectData
    {
        public Texture waterTex, distTex;
        public Vector2 waterTile, distortionTile;
        public Vector4 moveDir;
    }
}