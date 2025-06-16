using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

namespace IEC
{
    public class SoundController : MonoBehaviour, IEventListener
    {
        public AudioPlayer audioPlayerPrefab;

        private List<AudioPlayer> m_ListMusicAudio = new List<AudioPlayer>();
        private List<AudioPlayer> m_ListSoundAudio = new List<AudioPlayer>();
        public List<AudioInfo> listSoundConfig = new List<AudioInfo>();
        public List<AudioInfo> listMusicConfig = new List<AudioInfo>();


    
        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
   
                case EventList.GamePlay.SETTING_SOUND:
                    break;
                case EventList.GamePlay.SETTING_MUSIC:

                    break;
               case EventList.GamePlay.SET_MUTE_BACKGROUND_MUSIC:
                   SetMuteAllMusic(true);
                    break;
               case EventList.GamePlay.SET_UNMUTE_BACKGROUND_MUSIC_INGAME:
                    SetMuteMusic(EventList.Audio.BACKGROUND_INGAME, false);
                    break;
                case EventList.GamePlay.SET_UNMUTE_BACKGROUND_MUSIC_HOME:
                    SetMuteMusic(EventList.Audio.BACKGROUND_HOME, false);
                    break;
                default:
                    PlayAudio(gameEvent);
                    break;
            }
        }

        private void PlayAudio(string id)
        {
            if (listMusicConfig.Find(x => x.id == id) != null)
            {
                PlayBackGroundMusic(id);
            }
            else if (listSoundConfig.Find(x => x.id == id) != null)
            {
                PlaySound(id);
            }
        }

        private void PlayBackGroundMusic(string id)
        {
            SetMuteAllMusic(true);

            if (id == EventList.Audio.BACKGROUND_INGAME)
            {
                //Reset ingame background music each new game
                ResetMusicLoop(id);
            }
            else
            {
                if (!IsMusicPlaying(id))
                    PlayMusicOnLoop(id);
            }

    
            
            SetMuteMusic(id, false);
        }

    AudioPlayer CreateAudio(string id, bool sound = true)
        {
            var audio = Instantiate(audioPlayerPrefab, this.gameObject.transform);
            if (sound)
            {
                m_ListSoundAudio.Add(audio);
            }
            else
            {
                m_ListMusicAudio.Add(audio);
            }
            return audio;
        }
        
        AudioPlayer GetPlayerSound(string id)
        {
            var audio = m_ListSoundAudio.Find(x => x.Info.id == id);
            if (audio == null)
            {
                audio = CreateAudio(id);
            }
            return audio;
        }
        AudioPlayer GetPlayerMusic(string id)
        {
            var audio = m_ListMusicAudio.Find(x => x.Info.id == id);
            if (audio == null)
            {
                audio = CreateAudio(id,false);
            }
            return audio;
        }

  

        void SetMuteAllMusic(bool isMute)
        {
            foreach (var player in m_ListMusicAudio)
            {
                player.SetMute(isMute);
            }
        }
        
        void SetMuteMusic(string id, bool isMute)
        {
            var data = m_ListMusicAudio.Find(x => x.Info.id == id);
            if (data != null)
            {
          
                data.SetMute(isMute);
            }
        }
        
        bool IsMusicPlaying(string id)
        {
            
            var audioMusicSource = m_ListMusicAudio.Find(x => x.Info.id == id);
            if (audioMusicSource == null)
                return false;

            return audioMusicSource.IsPlaying;
        }
        
        bool IsSoundPlaying(string id)
        {
            
            var audioSoundSource = m_ListSoundAudio.Find(x => x.Info.id == id);
            if (audioSoundSource == null)
                return false;

            return audioSoundSource.IsPlaying;
        }

        public void StopSound(string id)
        {
            var sound = GetPlayerSound(id);

            if (sound != null)
            {
                sound.Stop();
            }
        }
        
        public void PlaySound(string id)
        {
            var sound = GetPlayerSound(id);
            var data = listSoundConfig.Find(x => x.id == id);
            if (data != null && sound.Info.clip != data.clip)
            {
                sound.SetData(data);
            }
            
            
            sound.Play();
        }
 
        
        public void PlayMusicOnLoop(string id)
        {
            if (IsMusicPlaying(id))
                return;
            
            var music = GetPlayerMusic(id);
            var data = listMusicConfig.Find(x => x.id == id);
            if (data != null && music.Info.clip != data.clip)
            {
                music.SetData(data);
            }
            
            
            music.PlayOnLoop();
        }

        public void ResetMusicLoop(string id)
        {
            var music = GetPlayerMusic(id);
            var data = listMusicConfig.Find(x => x.id == id);
            if (data != null && music.Info.clip != data.clip)
            {
                music.SetData(data);
            }
            
            
            music.ResetLoop();
        }


        
        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

    }

 

    [Serializable]public class AudioInfo
    {
        public string id;
        public AudioClip clip;
        public List<AudioClip> listRandomAudioClips = new List<AudioClip>();
        public bool isLoop;
        public float loopInterval;
        public bool isPlayMultiple;
        [Range(0, 1) ]
        public float volume = 1f; 
        public AudioInfo(string id,AudioClip clip, bool isLoop, bool isPlayMultiple, float volume)
        {
            this.id = id;
            this.clip = clip;
            this.isLoop = isLoop;
            this.isPlayMultiple = isPlayMultiple;
            this.volume = volume;
        }
        
        public AudioInfo(AudioInfo info)
        {
            this.id = info.id;
            this.clip = info.clip;
            this.isLoop = info.isLoop;
            this.isPlayMultiple = info.isPlayMultiple;
            this.volume = info.volume;
            this.listRandomAudioClips = new List<AudioClip>(info.listRandomAudioClips);
        }
    }
}
