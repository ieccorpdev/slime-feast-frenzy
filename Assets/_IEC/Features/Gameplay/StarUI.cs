using IEC;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Features.Gameplay
{
    public class StarUI : _BaseBehaviour
    {
        [SerializeField] private Image imgStar;

        public void SetActiveStar(bool isActivated)
        {
            imgStar.gameObject.SetActive(isActivated);
        }
    }
}