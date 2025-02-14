using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class AvatarUI : _BaseBehaviour
    {
        [SerializeField] Image img;
        [SerializeField] GameObject chosenObj;
        [SerializeField] GameObject tickObj;

        [SerializeField] Button button;

        public int id;

        public void SetData(Sprite sprite, int id, System.Action<AvatarUI> action)
        {
            img.sprite = sprite;
            this.id = id;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                action?.Invoke(this);
                //Observer.Notify(EventList.WindLeaderboard.CHANGE_AVATAR);
                EventController.RaiseEvent(EventList.WindLeaderboard.CHANGE_AVATAR);
            });
        }
        public void SetChosen(bool chosen)
        {
            chosenObj.SetActive(chosen);
            tickObj.SetActive(chosen);
        }
    }
}