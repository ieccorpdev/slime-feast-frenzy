using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/AllCharactersConfig")]
    public class AllCharactersConfig : ScriptableObject
    {
        public List<CharacterConfig> listAllCharactersConfig;

        public CharacterConfig GetCharacterConfig(string characterName)
        {
            return listAllCharactersConfig.Find(character => character.characterName.Equals(characterName));
        }

        public CharacterConfig GetDefaultCharacterConfig()
        {
            return listAllCharactersConfig.FirstOrDefault(character => character.isDefaultCharacter);
        }

        public CharacterPower GetCharacterPower(string characterName, CharacterPowerType powerType,
            ResourceType resourceAffected = default)
        {
            return GetCharacterConfig(characterName).GetPower(powerType, resourceAffected);
        }
    }
}
