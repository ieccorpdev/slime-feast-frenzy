using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/PlayerAvatarConfig")]
    public class PlayerAvatarConfig : ScriptableObject
    {
        public List<Sprite> listAvatarSprites = new();
        public List<Sprite> listAvatarFrameSprites = new();
        public List<Sprite> listSpecialAvatarFrameSprites = new();
        public List<AvatarNFrameUnlockConfig> listUnlockConfig = new();

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

            List<Sprite> listAllFrame = new();
            listAllFrame.AddRange(listAvatarFrameSprites);
            listAllFrame.AddRange(listSpecialAvatarFrameSprites);
            
            Sprite sprite = listAllFrame.Find(sprite => sprite.name.Equals(spriteName));

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

        public AvatarNFrameUnlockConfig GetUnlockConfig(string spriteName)
        {
            return listUnlockConfig.FirstOrDefault(config => config.mainSprite.name.Equals(spriteName));
        }
    }

    [System.Serializable]
    public class AvatarNFrameUnlockConfig
    {
        public AvatarUnlockConditionType conditionType;
        public int conditionAmount;
        public Sprite mainSprite;
        public Sprite lockFrameSprite;
        
    }
}
