using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupProfile : BasePopup
    {
        public Button btnEditProfile;
        // Start is called before the first frame update
        void Start()
        {
            SetEvent(btnEditProfile,
                () =>
                {
                    Locator.Main.PopupEditProfile.Show();
                });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
