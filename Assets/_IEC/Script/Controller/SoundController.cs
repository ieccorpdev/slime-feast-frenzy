using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class SoundController : _BaseBehaviour, IController, IEventListener, IEventAwake
    {
        [SerializeField]
        private AudioSource m_AudioSource;

        [SerializeField]
        private AudioListener m_AudioListener;

        [SerializeField]
        private AudioClip[] m_AudioClip;

        private float m_TimePlaySound;
        private float m_DelayTimeSound = .1f;

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void BehaviourAwake()
        {
            SetupAudio();
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.PLAY_SOUND:
                    PlaySound((SoundName)param.sender);
                    break;
            }
        }

        private void PlaySound(SoundName soundName)
        {
            if ((Time.time - m_DelayTimeSound) < m_TimePlaySound) return;
            m_TimePlaySound = Time.time;
            m_AudioSource.PlayOneShot(m_AudioClip[(int)soundName]);
        }

        private void SetupAudio()
        {
            m_AudioListener.enabled = PlayerData.isSoundOn;
        }

    }

    public enum SoundName
    {
        GRID_CLICK,
        WALL_CLICK,
        WATER_CONNECT,
        TARGET_DONE,
        VICTORY
    }
}