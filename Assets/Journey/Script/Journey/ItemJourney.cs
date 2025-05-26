using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ItemJourney : _BaseBehaviour
    {
        public int idItem;
        public TextMeshProUGUI currentIdItem;
        public ItemPassJourney vipPass;
        public ItemPassJourney regularPass;

        public void SetUpDataItemJourney(RewardByJourney data)
        {
            currentIdItem.text = data.idJourney.ToString();
            //SetChestModel
            regularPass.SetupDataItemPass(data,data.RewardsNormal,data.idJourney);
            vipPass.SetupDataItemVipPass(data,data.RewardsVips,data.idJourney);
        }
    }

    [Serializable]
    public class RewardByJourney
    {
        public int idJourney;
        public int fuelNeed;
        public Rewards RewardsVips;
        public Rewards RewardsNormal;
    }
}
