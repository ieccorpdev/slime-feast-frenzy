using System.Collections.Generic;
using System.Linq;
using IEC;
using UnityEngine;

namespace _IEC.Features.Booster
{
    public class GroupBoosterPopup : _BaseBehaviour
    {
        [SerializeField] private List<PopupBuyBooster> popupBuyBoosters;
        //public PopupBoosterInGame popupBoosterInGame;
        public PopupBuyBooster GetPopupBuyBoosterById(BoosterId boosterId)
        {
            return popupBuyBoosters.FirstOrDefault(x => x.BoosterId == boosterId);
        }
    }
}
