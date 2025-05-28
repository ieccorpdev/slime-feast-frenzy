using System.Collections.Generic;
using UnityEngine;

namespace _IEC.Script.Data.UI.Popup.RelicHunt
{
    public class RelicLevelVisual : MonoBehaviour
    {
        public GameObject[] listLevelGO;

        public void DoShowLevelVisual(int level)
        {
            if(level <= 0 || level > listLevelGO.Length) return;
            foreach (var levelGo in listLevelGO)
            {
                levelGo.SetActive(false);
            }
            listLevelGO[level - 1].SetActive(true);
        }
    }
}