using UnityEngine;

namespace _IEC.Script.Data.UI.Popup.RelicHunt
{
    public class LevelProgressNode : MonoBehaviour
    {
        public GameObject lockGO;
        public GameObject unlockGO;
        public float vitrualPercent;

        public void SetLock(bool locked)
        {
            lockGO.SetActive(locked);
            unlockGO.SetActive(!locked);
        }
    }
}