using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/PlayerAvatarConfig")]
    public class PlayerAvatarConfig : ScriptableObject
    {
        public List<Sprite> listAvatarSprites = new List<Sprite>();

        public Sprite GetAvatarSprite(string spriteName)
        {
            return null;
        }
    }
}
