using System;
using System.Collections.Generic;
using System.Linq;
using IEC;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Features.Booster
{
    public class GroupBoosterInLevelFail : _BaseBehaviour
    {
        [SerializeField] private List<ButtonBooster> buttonBoosters;

        public void Show(List<BoosterId> boosterIds)
        {
            foreach (var boosterId in boosterIds)
            {
                var buttonBooster = GetButtonBoosterById(boosterId);
                buttonBooster.OnShow();
            }
            gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            foreach (var button in buttonBoosters)
            {
                button.Dispose();
            }
        }

        public List<BoosterId> GetSelectedBoosters()
        {
            return buttonBoosters.Where(x =>
                x.CurrentState == ButtonBooster.State.Using 
                || x.CurrentState == ButtonBooster.State.Unlimited)
                .Select(x=>x.BoosterId).ToList();
        }

        private ButtonBooster GetButtonBoosterById(BoosterId boosterId)
        {
            return buttonBoosters.FirstOrDefault(x => x.BoosterId == boosterId);
        }
    }
}