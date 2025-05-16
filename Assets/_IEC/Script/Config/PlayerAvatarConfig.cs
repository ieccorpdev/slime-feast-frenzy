using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/PlayerAvatarConfig")]
    public class PlayerAvatarConfig : ScriptableObject
    {
        public List<Sprite> listAvatarSprites = new();
        public List<Sprite> listAvatarFrameSprites = new();

        public Sprite GetAvatarSprite(string spriteName)
        {
            if (listAvatarSprites.Count == 0)
            {
                Debug.LogError("No player avatar available!");
                return null;
            }
            
            
            Sprite sprite = listAvatarSprites.Find(sprite => sprite.name.Equals(spriteName));

            if (sprite == null)
            {
                sprite = listAvatarSprites[0];
            }
            
            return sprite;
        }
        
        public Sprite GetAvatarFrameSprite(string spriteName)
        {
            if (listAvatarFrameSprites.Count == 0)
            {
                Debug.LogError("No player avatar available!");
                return null;
            }
            
            Sprite sprite = listAvatarFrameSprites.Find(sprite => sprite.name.Equals(spriteName));

            if (sprite == null)
            {
                sprite = listAvatarFrameSprites[0];
            }
            
            return sprite;
        }

        public Sprite GetRandomAvatarSprite()
        {
            int index = Random.Range(0, listAvatarSprites.Count);

            return listAvatarSprites[index];
        }
        
        public Sprite GetRandomAvatarFrameSprite()
        {
            int index = Random.Range(0, listAvatarFrameSprites.Count);

            return listAvatarFrameSprites[index];
        }
    }
}
