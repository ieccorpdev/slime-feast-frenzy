using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        public Color[] waterColor;
        public Color targetColorDefault;

        public Material[] targetMats;
        public Material targetDefaultMat;

        public List<TextAsset> ListLevels;

        public List<TutorialConfig> listTuts;

        public GameObject effectLeavesObj;

        public Color[] colorBSP;

        public Material[] matBSP;

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

        public Material GetWaterClassicMat(bool isMultiClassicColor, int color)
        {
            if (isMultiClassicColor)
            {
                return matBSP[color];
            }
            else
            {
                return targetDefaultMat;
            }
        }

        public Material GetWaterMat(int color)
        {
            var colorIndex = Mathf.Clamp(color - 1, 0, targetMats.Length - 1);
            return targetMats[colorIndex];
        }


    }
}