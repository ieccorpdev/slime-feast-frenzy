using IEC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Features.Gameplay
{
    public class GroupUnlockMapProgressUI : _BaseBehaviour
    {
        [SerializeField] private Image progressImage;
        [SerializeField] private Image mapIcon;
        [SerializeField] private Image lockIcon;
        [SerializeField] private TextMeshProUGUI txtProgress;

        public void SetData(int process, int target, Sprite icon)
        {
            progressImage.fillAmount = (float) process / target;
            txtProgress.text = $"{process}/{target}";
            mapIcon.sprite = icon;

            if (process == target)
            {
                lockIcon.gameObject.SetActive(false);
                mapIcon.color = Color.white;
            }
            else
            {
                lockIcon.gameObject.SetActive(true);
                mapIcon.color = Color.gray;
            }
        }
    }
}