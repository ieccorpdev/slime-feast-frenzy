using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System.Linq;
using System.Reflection;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;
using static IEC.EventList;
using DG.Tweening;

namespace IEC
{
    public class SoundController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
        public AudioPlayer audioPlayerPrefab;

        private List<AudioPlayer> m_ListMusicAudio = new List<AudioPlayer>();
        private List<AudioPlayer> m_ListSoundAudio = new List<AudioPlayer>();
        public List<AudioInfo> listSoundConfig;
        public List<AudioInfo> listMusicConfig;
        private Dictionary<string, int> m_DicAudioConfig = new Dictionary<string, int>();

#if UNITY_EDITOR
        [Sirenix.OdinInspector.Button]
        public void AddSoundConfig()
        {
            var audioFields = typeof(EventList.Audio).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            foreach (var field in audioFields)
            {
                if (field.FieldType == typeof(string))
                {
                    var audioId = (string)field.GetValue(null);
                    var newSound = new AudioInfo(audioId, null, false, 1f);
                    listSoundConfig.Add(newSound);
                }
            }
        }
#endif
        public void LocateController()
        {
            Pool.Set(this);
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                // case EventList.GamePlay.SETTING_MUSIC:
                //     SetMuteAllMusic(!PlayerData.isMusicOn);
                //     break;
                case EventList.GamePlay.SETTING_SOUND:
                    SetMuteAllSound(!PlayerData.isSoundOn);
                    break;
                case EventList.GamePlay.SETTING_MUSIC:
                    SetMuteAllMusic(!PlayerData.isMusicOn);
                    if (PlayerData.isMusicOn)
                    {
                        if (GamePlay.GameState == Locator.Game.ScreenInGame || GamePlay.GameState == Locator.Game.PopupSettingIngame)
                        {
                            EventController.RaiseEvent(EventList.Audio.BACKGROUND_INGAME);
                        }
                        else
                        {
                            EventController.RaiseEvent(EventList.Audio.BACKGROUND_HOME);
                        }
                    }
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
                #if UNITY_IOS
                case EventList.GamePlay.SHOW_FULL_SCREEN_BY_MEDIATION_ADS:
                    AudioSettings.Mobile.StopAudioOutput();
                    break;
                case EventList.GamePlay.HIDE_FULL_SCREEN_BY_MEDIATION_ADS:
                    AudioSettings.Mobile.StartAudioOutput();
                    break;
                #endif
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

            if (!PlayerData.isMusicOn)
                return;
            
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

        void SetMuteAllSound(bool isMute)
        {
            foreach (var player in m_ListSoundAudio)
            {
                player.SetMute(isMute);
            }
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
            // if(!IsMusicPlaying(id))
            //     PlayMusic(id);
            var data = m_ListMusicAudio.Find(x => x.Info.id == id);
            if (data != null)
            {
                if (!isMute && !PlayerData.isMusicOn)
                {
                    isMute = true;
                }
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
            if (data != null && data.clip == null)
            {
                Debug.LogError($"Sound {id} is null");  
                return;
            }
            sound.gameObject.SetActive(true);
            if (data != null && sound.Info.clip != data.clip)
            {
                //PlaySound(data);
                sound.SetData(data);
            }
            
            sound.SetMute(!PlayerData.isSoundOn);
            
            sound.Play();
        }

        public float GetLengthSound(string id)
        {
            var sound = GetPlayerSound(id);
            var data = listSoundConfig.Find(x => x.id == id);
            if (data != null && data.clip == null)
            {
                Debug.LogError($"Sound {id} is null");
                return 0;
            }
            if (data != null && sound.Info.clip != data.clip)
            {
                //PlaySound(data);
                sound.SetData(data);
            }

            return sound.audioSource.clip.length;
        }

        public void PlayMusic(string id)
        {
            if (IsMusicPlaying(id))
                return;
            
            var music = GetPlayerMusic(id);
            var data = listMusicConfig.Find(x => x.id == id);
            if (data != null && data.clip == null)
            {
                Debug.LogError($"Sound {id} is null");
                return;
            }
            if (data != null && music.Info.clip != data.clip)
            {
                music.SetData(data);
            }
            
            music.SetMute(!PlayerData.isMusicOn);
            
            music.Play();
        }
        
        public void PlayMusicOnLoop(string id)
        {
            if (IsMusicPlaying(id))
                return;
            
            var music = GetPlayerMusic(id);
            var data = listMusicConfig.Find(x => x.id == id);
            if (data != null && data.clip == null)
            {
                Debug.LogError($"Sound {id} is null");
                return;
            }
            if (data != null && music.Info.clip != data.clip)
            {
                music.SetData(data);
            }
            
            music.SetMute(!PlayerData.isMusicOn);
            
            music.PlayOnLoop();
        }

        public void ResetMusicLoop(string id)
        {
            var music = GetPlayerMusic(id);
            var data = listMusicConfig.Find(x => x.id == id);
            if (data != null && data.clip == null)
            {
                Debug.LogError($"Sound {id} is null");
                return;
            }
            if (data != null && music.Info.clip != data.clip)
            {
                music.SetData(data);
            }
            
            music.SetMute(!PlayerData.isMusicOn);
            
            music.ResetLoop();
        }

        public void SetVolumeAllSound(float volume)
        {
            volume = Mathf.Clamp(volume, 0, 1);
            foreach (var player in m_ListSoundAudio)
            {
                player.SetVolume(volume);
            }
        }

        public void StopAllSound()
        {
            foreach (var player in m_ListSoundAudio)
            {
                player.Stop();
            }
        }

        public void SetVolumeAllMusic(float volume)
        {
            volume = Mathf.Clamp(volume, 0, 1);
            foreach (var player in m_ListMusicAudio)
            {
                player.SetVolume(volume);
            }
        }
        
        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void BehaviourStart()
        {
            
        }

        public void BehaviourAwake()
        {
            
        }
    }
    
    [Serializable]public class AudioInfo
    {
        [Dropdown("GetAudioEvents")]
        public string id;
        public AudioClip clip;
        public List<AudioClip> listRandomAudioClips = new();
        public bool isLoop;
        public float loopInterval;
        [Range(0, 1) ]
        public float volume = 1f; 
        public AudioInfo(string id,AudioClip clip, bool isLoop, float volume)
        {
            this.id = id;
            this.clip = clip;
            this.isLoop = isLoop;
            this.volume = volume;
        }
        
        public AudioInfo(AudioInfo info)
        {
            this.id = info.id;
            this.clip = info.clip;
            this.isLoop = info.isLoop;
            this.volume = info.volume;
            this.listRandomAudioClips = new List<AudioClip>(info.listRandomAudioClips);
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
