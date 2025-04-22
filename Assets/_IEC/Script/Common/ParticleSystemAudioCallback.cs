using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IEC
{
    public class ParticleSystemAudioCallback : _BaseBehaviour
    {
        [Dropdown("GetAudioEvents")]
        public string id;

        public float delay = 0f;
        public int maxPlayAmount = 3;
        [Range(0, 1)]
        public float playSoundRate = 0f;
        private ParticleSystem m_particleSystem;
        private ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1000];

        private float lastTime;

        private int _timePlayed = 0;
        
        private void Start()
        {
            m_particleSystem = GetComponent<ParticleSystem>();
            lastTime = m_particleSystem.main.duration;
        }
        
        // Update is called once per frame
        void Update()
        {
            if (m_particleSystem == null)
                return;

            if (!m_particleSystem.isPlaying)
            {
                _timePlayed = 0;
                return;
            }

            if (!gameObject.activeInHierarchy)
            {
                _timePlayed = 0;
                return;
            }

            if (_timePlayed >= maxPlayAmount)
                return;
            
            float currentTime = m_particleSystem.time;

            if (currentTime < delay)
                return;

            // Detect loop reset
            if (currentTime < lastTime)
            {
                //Debug.Log($"New loop started for {GetComponent<ParticleSystem>().name}!");
                if (Random.Range(0, 100) <= playSoundRate * 100)
                {
                    PlaySound();
                    _timePlayed++;
                }
            }

            lastTime = currentTime;
            
        }

        private void PlaySound()
        {
            EventController.RaiseEvent(id);
        }
        
        private static List<string> GetAudioEvents => GetAllAudioEvents();
        
        public static List<string> GetAllAudioEvents()
        {
            var audioType = typeof(EventList.Audio);
            var fields = audioType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            return fields.Select(field => (string)field.GetValue(null)).ToList();
        }
    }
}
