using System.Collections.Generic;
using IEC;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Features.Gameplay
{
    public class GroupProcess : _BaseBehaviour
    {
        [SerializeField] private List<Image> processImages;
        [SerializeField] private float originalWidth;
        [SerializeField] private float originalHeight;
        
        public void SetAmount(float amount)
        {
            foreach (var processImage in processImages)
            {
                processImage.rectTransform.sizeDelta = new Vector2(originalWidth * amount, originalHeight);
            }
        }

        public void SetStateIndex(int index)
        {
            foreach (var processImage in processImages)
            {
                processImage.gameObject.SetActive(false);
            }
            if (index < 0) index = 0;
            if (index >= processImages.Count) index = processImages.Count - 1;
            processImages[index].gameObject.SetActive(true);
        }
        

        public int GetNumberOfProcessState()
        {
            return processImages.Count;
        }
    }
}