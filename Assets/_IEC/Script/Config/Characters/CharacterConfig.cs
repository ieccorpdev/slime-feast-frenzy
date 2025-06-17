using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/Character/CharacterConfig")]
    public class CharacterConfig : ScriptableObject
    {
        public bool isDefaultCharacter;
        public string characterName;
        public int comboCountToChange;
        public string characterDescription;
        [Header("UI")]
        public Sprite characterSelectionSprite;
        public Sprite characterLockedSprite;
        public Sprite characterMainSprite;
        public Sprite characterShardSpriteNormal;
        public Sprite characterShardSpriteSmall;
        public Sprite characterRecommendBarSprite;
        
        [Header("Character Anim")]
        public float defaultCharacterPosY;
        public float characterAnimSpeed = 1f;
        public Animator characterHomePrefab;
        public Animator characterControlPrefab;
        public Animator characterBitePrefab;
        
        [Header("Character Stat")]
        public List<CharacterUpgrade> listCharacterUpgrade = new();
        public List<CharacterTrait> listCharacterTraits;
        public List<CharacterStat> listCharacterStat;
        public List<CharacterPower> listCharacterPowers;

        [Header("Character unlock")] 
        public int levelUnlock;
        public int characterLevelOnUnlock;

        public CharacterPower GetPower(CharacterPowerType powerType, ResourceType resourceAffected = default)
        {
            return listCharacterPowers.Find(power =>
                power.powerType == powerType && power.resourceAffected == resourceAffected);
        }

        public CharacterPower GetPower(int index)
        {
            if (listCharacterPowers.Count == 0)
                return null;
            
            index = Mathf.Clamp(index, 0, listCharacterPowers.Count - 1);

            return listCharacterPowers[index];
        }

        public CharacterStat GetStat(CharacterStatType statType)
        {
            return listCharacterStat.Find(stat => stat.statType == statType);
        }

        public CharacterTrait GetTrait(CharacterTraitType traitType)
        {
            return listCharacterTraits.Find(trait => trait.traitType == traitType);
        }

        public CharacterUpgrade GetCharacterUpgrade(int index)
        {
            if (listCharacterUpgrade.Count == 0)
                return null;

            if (index < 0 || index >= listCharacterUpgrade.Count)
                return null;
            
            return listCharacterUpgrade[index];
        }

        public int GetTotalShardForMaxLevel()
        {
            return listCharacterUpgrade.Sum(upgrade => upgrade.shardCost);
        }
    }

    [System.Serializable]
    public class CharacterUpgrade
    {
        public int shardCost;
        public int goldCost;
    }

    [System.Serializable]
    public class CharacterPower
    {
        public CharacterPowerType powerType;
        public ResourceType resourceAffected;
        public List<CharacterPowerValue> listCharacterPowerValue = new();

        public CharacterPowerValue GetCharacterPowerValue(int characterLevel)
        {
            characterLevel = Mathf.Clamp(characterLevel, 0, listCharacterPowerValue.Count);

            return listCharacterPowerValue[characterLevel];
        }
    }

    [System.Serializable]
    public class CharacterStat
    {
        public CharacterStatType statType;
        [Range(0,1)]public float statValue;
    }

    [System.Serializable]
    public class CharacterPowerValue
    {
        public int conditionValueForPower; // for example item xp to apply power
        public float buffAmount;
        public string overrideDescriptionString;
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
        BonusXpOnSmallItems,
        IncreaseHelperTime,
        CompassOceanGiftItem,
        IncreaseCameraSizeOnHelper,
        BombShieldOnHelper,
        IncreaseSizeOnIdle,
        AddTimerOnCombo,
        SpeedBoostOnSpawn
    }

    public enum CharacterStatType
    {
        Speed,
        Size,
        Transform,
    }
}
