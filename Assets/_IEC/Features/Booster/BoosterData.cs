using System;
using System.Collections.Generic;
using System.Linq;
using _IEC.Features.Common.Extension;
using IEC;
using UnityEngine;

namespace _IEC.Features.Booster
{
    [System.Serializable]
    public class BoosterData
    {
        public List<BoosterDataModel> boosterDataModels;

        public BoosterDataModel GetDataModelById(BoosterId boosterId)
        {
            return boosterDataModels.FirstOrDefault(x => x.boosterId == boosterId);
        }
        
        public void BuyBooster(BoosterId boosterId,int number)
        {
            var model = GetDataModelById(boosterId);
            model.number += number;
            Observer.Notify(EventList.GamePlay.UPDATE_BOOSTER_UI, boosterId);
        }
    }
    
    [System.Serializable]
    public class BoosterDataModel
    {
        public int number;
        public BoosterId boosterId;
        public DateTime unlimitedExpiredTime;
        public bool isCompleteTutorial;
        public void ConsumeBooster()
        {
            number--;
            if (number < 0)
            {
                number = 0;
            }
        }

        public void CompleteTutorial()
        {
            isCompleteTutorial = true;
        }

        public void AddFreeNumber(int numberAdded)
        {
            number += numberAdded;
        }
        
        public void AddUnlimitedTime(float seconds)
        {
            if (unlimitedExpiredTime < DateTime.Now)
            {
                unlimitedExpiredTime = DateTime.Now;
            }
            unlimitedExpiredTime = unlimitedExpiredTime.Add(TimeSpan.FromSeconds(seconds));
        }

        public bool IsInUnlimitedTime()
        {
            return DateTime.Now < unlimitedExpiredTime;
        }
    }
}