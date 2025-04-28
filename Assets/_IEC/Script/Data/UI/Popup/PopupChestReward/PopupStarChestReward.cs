using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class PopupStarChestReward : PopupChestReward
    {
        public override void Show()
        {
            isPlayAudioPopup = false;
            base.Show();
            EventController.RaiseEvent(EventList.Audio.CHEST_OPEN);
        }

        public override void Hide()
        {
            base.Hide();
            EventController.RaiseEvent(EventList.Audio.CHEST_DROP);
        }
    }
}
