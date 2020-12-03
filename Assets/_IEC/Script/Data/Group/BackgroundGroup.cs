using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class BackgroundGroup : MonoBehaviour
    {
        [SerializeField]
        private Image m_ImgBackground;

        public void SetBackground(Sprite sprite)
        {
            m_ImgBackground.sprite = sprite;
        }
    }
}