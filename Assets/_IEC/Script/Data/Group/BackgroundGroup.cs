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

        [SerializeField]
        private GameObject m_TopBgObj, m_BotBgObj;

        public void SetBackground(Sprite sprite, bool isDefault)
        {
            m_ImgBackground.sprite = sprite;

            m_TopBgObj.SetActive(isDefault);
            m_BotBgObj.SetActive(isDefault);
        }
    }
}