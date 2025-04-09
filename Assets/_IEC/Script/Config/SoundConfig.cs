using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(fileName = "SoundConfig", menuName = "Game/SoundConfig")]
    public class SoundConfig : BaseConfig
    {
        public float delayInterval = 0.15f;
        
        [Header("Module")]
        public AudioClip background;
        public AudioClip unlockNewItemBuildCity;
        public AudioClip unlockNewFeatureGP;
        public AudioClip openChest;
        
        // Add sounds here
        [Header("Power items")] 
        public AudioClip biggerHoleOn;
        public AudioClip biggerHoleOff;
        public AudioClip magnetOn;
        public AudioClip magnetOff;
        public AudioClip magnetActive;
        public AudioClip compassOn;
        public AudioClip compassOff;
        public AudioClip compassActive;
        public AudioClip freezeTimeOn;
        public AudioClip freezeTimeOff;
        public AudioClip freezeTimeActive;

        [Header("Boosters")] 
        public AudioClip boosterChrono;
        public AudioClip boosterLightning;
        public AudioClip littleChrono;
        
        [Header("Gameplay")] 
        public AudioClip normalConsume;
        public AudioClip objectiveConsume;
        public AudioClip objectiveCleared;
        public AudioClip levelUp;
        public AudioClip bombLose;

        [Header("Misc")] 
        public AudioClip buttonClick;
        public AudioClip buttonExit;
        public AudioClip moreLife;
        public AudioClip coin;
        public AudioClip confettiWin;
    }
}