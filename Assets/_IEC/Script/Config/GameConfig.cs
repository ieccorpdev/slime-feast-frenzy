using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace IEC
{
    /// <summary>
    /// object contain references to all other configs (hard fixed data)
    /// </summary>
    [CreateAssetMenu(menuName = "Game/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public _EmptyConfig emptyConfig;
        public ComponentConfig componentConfig;

        public GameObject effectLeavesObj;

        public List<TutorialConfig> listTuts;

        //Water
        public Color[] waterColor;
        public Color targetColorDefault;

        public Material[] targetMats;
        public Material[] targetGradientMats;
        public Material targetDefaultMat;

        public Color[] colorBSP;
        public Material[] matBSP;
        public Material[] matBSPGradient;

        //Effect config
        public float startEffectMoveTime;

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
    }
}