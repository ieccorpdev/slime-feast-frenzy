using System.Collections.Generic;
using UnityEngine;

namespace _IEC.Script.Data.UI.Popup.RelicHunt
{
    public class RelicLevelVisual : MonoBehaviour
    {
        public GameObject[] listLevelGO;
        public GameObject[] listStoneLevelGO;

        public void DoShowLevelVisual(int level)
        {
            level--;
            level = Mathf.Clamp(level, 0, listLevelGO.Length - 1);
            foreach (var levelGo in listLevelGO)
            {
                levelGo.SetActive(false);
            }

            for (var index = 0; index < listStoneLevelGO.Length; index++)
            {
                var stoneGo = listStoneLevelGO[index];
                stoneGo.SetActive(index <= level );
            }

            listLevelGO[level].SetActive(true);
        }
    }
}