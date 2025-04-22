using System;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/ProfileConfig")]
    public class ProfileConfig : BaseConfig
    {
        public List<Sprite> avatarSprites;
        public List<Sprite> frameSprites;

        public const int TOTAL_AVATAR = 11;
        public const int TOTAL_FRAME = 10;

        public Sprite GetAvatarSprite(int index) => avatarSprites[index];
        public Sprite GetFrameSprite(int index) => frameSprites[index];
        
        //public Sprite GetCurrentAvatarSprite() => GetAvatarSprite(Pool.Get<PlayerSecret>().ProfileData.AvatarData.avatarIndex);
        //public Sprite GetCurrentFrameSprite() => GetFrameSprite(Pool.Get<PlayerSecret>().ProfileData.AvatarData.frameIndex);

        //public Sprite GetProfileCustomizationSprite(ProfileCustomizationType type, int index)
        //{
        //    return type switch
        //    {
        //        ProfileCustomizationType.Avatar => GetAvatarSprite(index),
        //        ProfileCustomizationType.Frame => GetFrameSprite(index),
        //        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        //    };
        //}
        
    }
}