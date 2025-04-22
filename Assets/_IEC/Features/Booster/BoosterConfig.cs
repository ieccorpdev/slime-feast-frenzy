using System.Collections.Generic;
using System.Linq;
using _IEC.Features.Common;
using IEC;
using UnityEngine;

namespace _IEC.Features.Booster
{
    [CreateAssetMenu(menuName = "Game/Features/Booster/BoosterConfig")]
    public class BoosterConfig : ScriptableObject
    {
        public int startShowAtLevel;
        public List<BoosterConfigModel> configModels;

        public BoosterConfigModel GetBoosterConfigModelById(BoosterId boosterId)
        {
            return configModels.FirstOrDefault(x => x.boosterId == boosterId);
        }
    }

    [System.Serializable]
    public class BoosterConfigModel
    {
        public BoosterId boosterId;
        public int unlockLevel;
        public PriceModel priceModel;
        public int initFreeNumber;
        public int advantageNumber;
    }

    [System.Serializable]
    public class PriceModel
    {
        public ResourceType currencyType;
        public int number;
        public int result;
        
    }
    
}