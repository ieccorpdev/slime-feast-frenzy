using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/AllCharactersConfig")]
    public class AllCharactersConfig : ScriptableObject
    {
        public List<CharacterConfig> listAllCharactersConfig;

        public CharacterConfig GetDefaultCharacter()
        {
            if (listAllCharactersConfig.Count == 0)
                return null;
            
            return listAllCharactersConfig[0];
        }

        public CharacterConfig GetCharacterConfig(string characterName)
        {
            return listAllCharactersConfig.Find(character => character.characterName.Equals(characterName));
        }

        public CharacterPower GetCharacterPower(string characterName, CharacterPowerType powerType,
            ResourceType resourceAffected = default)
        {
            return GetCharacterConfig(characterName).GetPower(powerType, resourceAffected);
        }
    }
}
