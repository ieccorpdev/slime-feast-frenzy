using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;

namespace IEC
{
    public class AudioPlayer : _BaseBehaviour
    {
        public AudioInfo Info;
        public AudioSource audioSource;

        private List<AudioClip> _listRandomAudioClips = new();
        
        public bool IsLoop { get; private set; }

        public bool IsPlayingOnLoop { get; private set; } = false;

        private Coroutine _playLoopCoroutine;

        public void SetData(AudioInfo info)
        {
            Info = new AudioInfo(info);
            audioSource.clip = Info.clip;
            _listRandomAudioClips = new List<AudioClip>(Info.listRandomAudioClips);
            SetLoop(Info.isLoop);
            SetVolume(Info.volume);
        }

        public bool IsPlaying => audioSource.isPlaying;

        public void Play()
        {
            gameObject.SetActive(true);
            audioSource.clip = GetRandomAudioClip();
            if (!audioSource.isPlaying)
                audioSource.Play();
        }

        public void PlayOnLoop()
        {
            if (!IsLoop)
                return;

            if (IsPlayingOnLoop)
                return;

            IsPlayingOnLoop = true;
            
            _playLoopCoroutine = StartCoroutine(IEPlayClipOnLoop());
        }

        public void ResetLoop()
        {
            if(_playLoopCoroutine != null)
                StopCoroutine(_playLoopCoroutine);

            IsPlayingOnLoop = true;
            
            _playLoopCoroutine = StartCoroutine(IEPlayClipOnLoop());
        }

        IEnumerator IEPlayClipOnLoop()
        {
            while (true)
            {
                Play();
                yield return new WaitForSeconds(audioSource.clip.length + Info.loopInterval);
            }

            IsPlayingOnLoop = false;
            
            StopCoroutine(_playLoopCoroutine);
            _playLoopCoroutine = null;
        }
        

        private AudioClip GetRandomAudioClip()
        {
            if (_listRandomAudioClips.Count == 0)
                return Info.clip;

            return _listRandomAudioClips[Random.Range(0, _listRandomAudioClips.Count)];
        }

        public void Stop()
        {
            audioSource.Stop();
            gameObject.SetActive(false);
        }

        public void SetMute(bool isMute)
        {
            if (!isMute)
            {
                audioSource.Stop();
                audioSource.Play();
            }
            audioSource.mute = isMute;
        }

        public void SetLoop(bool isLoop)
        {
            if (Info.loopInterval == 0)
                audioSource.loop = isLoop;
            IsLoop = isLoop;
        }

        /// <summary>
        /// Set volume
        /// </summary>
        /// <param name="volume">volume between 0 and 1</param>
        public void SetVolume(float volume)
        {
            volume = Mathf.Clamp(volume, 0, 1);
            audioSource.volume = volume;
        }
    }
}