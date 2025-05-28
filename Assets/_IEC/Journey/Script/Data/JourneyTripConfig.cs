using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IEC.Linq;
using Sirenix.OdinInspector;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/JourneyTripConfig")]
    public class JourneyTripConfig : ScriptableObject
    {
        [SerializeField, Tooltip("Max number of days for the journey")]
        private int maxDayInspector = 15;

        public static int maxDay;
        [Searchable] public List<RewardByJourney> listRewardsByJourneys;
        public RewardByJourney bonusReward;

        private void OnEnable()
        {
            maxDay = maxDayInspector;
        }

        public RewardByJourney GetRewardsByLevelJourney(int idJourney)
        {
            var item = listRewardsByJourneys.Find(x => x.idJourney == idJourney);
            if (item == null)
            {
                item = listRewardsByJourneys.Last();
            }
            return item;
        }
    }

    public enum JourneyItemStatus
    {
        LOCK,
        UNLOCK,
        CANCLAIM,
        CLAIMED
    }
}