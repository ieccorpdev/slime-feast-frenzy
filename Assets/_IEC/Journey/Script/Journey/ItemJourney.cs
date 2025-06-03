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
        public List<GameObject> lockGroups, unlockGroups;
        public TextMeshProUGUI idTextUnlock, idTextLock;
        public GameObject star;
        public GameObject bgEnd;

        public void SetUpDataItemJourney(RewardByJourney data)
        {
            currentIdItem.text = data.idJourney.ToString();
            regularPass.SetupDataItemPass(data.RewardsNormal, data.idJourney);
            vipPass.SetupDataItemPass(data.RewardsVips, data.idJourney);
            if (data.idJourney > 0)
            {
                idTextUnlock.text = data.idJourney.ToString();
                idTextLock.text = data.idJourney.ToString();
                star.gameObject.SetActive(false);
                idTextLock.gameObject.SetActive(true);
                idTextUnlock.gameObject.SetActive(true);
            }
            else
            {
                idTextLock.gameObject.SetActive(false);
                idTextUnlock.gameObject.SetActive(false);
                star.gameObject.SetActive(true);
            }
            bool check = data.idJourney < PlayerData.journeyData.currentTargetJourney;
            if (data.idJourney == PlayerData.journeyData.currentTargetJourney)
                bgEnd.SetActive(true);
            foreach (var item in lockGroups)
            {
                item.SetActive(!check);
            }
            foreach (var item in unlockGroups)
            {
                item.SetActive(check);
            }
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
