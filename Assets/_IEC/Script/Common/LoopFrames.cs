using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class LoopFrames : MonoBehaviour
    {
        public int fps = 60;
        public List<Sprite> listFrames;

        private int m_Count = 0;
        private SpriteRenderer m_SpriteRenderer;
        private Image m_Image;
        private float m_LastTime;

        // Start is called before the first frame update
        void Start()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_Image = GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            if (listFrames == null || listFrames.Count == 0) return;

            var fpsTime = 1f / fps;
            var time = Time.time - m_LastTime;

            if (time < fpsTime) return;

            m_LastTime = Time.time;
            m_Count %= listFrames.Count;
            var frame = listFrames[m_Count++];

            if (m_SpriteRenderer != null)
            {
                m_SpriteRenderer.sprite = frame;
            }

            if (m_Image != null)
            {
                m_Image.sprite = frame;
            }
        }
    }
}