using IEC;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class GroupScreenInGameMark : _BaseBehaviour
    {
        [SerializeField] private GameObject groupDangerous;
        [SerializeField] private GameObject groupFrozen;

        public void SetActiveDangerousMark(bool isActive)
        {
            if (groupDangerous.activeSelf == isActive) return;
            groupDangerous.SetActive(isActive);
        }
        
        public void SetActiveFrozenMark(bool isActive)
        {
            groupFrozen.SetActive(isActive);
        }
    }
}