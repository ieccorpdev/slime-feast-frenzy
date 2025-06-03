using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/Character/CharacterConfig")]
    public class CharacterConfig : ScriptableObject
    {
        public string characterName;
        public float defaultCharacterPosY;
        public float characterAnimSpeed = 1f;
        public Animator characterControlPrefab;
        public Animator characterBitePrefab;
        public List<CharacterTrait> listCharacterTraits;
        public List<CharacterPower> listCharacterPowers;

        public CharacterPower GetPower(CharacterPowerType powerType, ResourceType resourceAffected = default)
        {
            return listCharacterPowers.Find(power =>
                power.powerType == powerType && power.resourceAffected == resourceAffected);
        }

        public CharacterTrait GetTrait(CharacterTraitType traitType)
        {
            return listCharacterTraits.Find(trait => trait.traitType == traitType);
        }
    }

    [System.Serializable]
    public class CharacterPower
    {
        public CharacterPowerType powerType;
        public ResourceType resourceAffected; //For helper
        public float buffAmount;

        public float ApplyBuff(float value)
        {
            return value * (1 + buffAmount);
        }
    }

    [System.Serializable]
    public class CharacterTrait
    {
        public CharacterTraitType traitType;
        public float buffAmount;
    }

    public enum CharacterTraitType
    {
        IncreaseSpeed,
        IncreaseSize
    }

    public enum CharacterPowerType
    {
        LoseXpInsteadOfDeadOnBomb,
        SpeedBoostOnUseHelper,
        IncreaseHelperTime
    }
}
