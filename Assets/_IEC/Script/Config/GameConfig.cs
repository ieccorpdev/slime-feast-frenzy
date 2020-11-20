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

        public GameObject effectWallObj;


        public Color GetWaterColor(int color, bool isMultiColor)
        {
            if (isMultiColor)
            {
                var colorIndex = Mathf.Clamp(color - 1, 0, waterColor.Length - 1);
                return waterColor[colorIndex];
            }
            else
            {
                return targetColorDefault;
            }
        }

        public Material GetWaterColorMat(int color, bool isMulticolor)
        {
            if (isMulticolor)
            {
                var colorIndex = Mathf.Clamp(color - 1, 0, targetMats.Length - 1);
                return targetMats[colorIndex];
            }
            else
            {
                return targetDefaultMat;
            }
        }
    }
}