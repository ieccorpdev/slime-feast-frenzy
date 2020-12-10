using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupRating : MonoBehaviour
    {
        [SerializeField]
        private Button m_BtnClose, m_BtnNoThank, m_BtnLoveIt;

        public void OnInit()
        {
            m_BtnClose.onClick.AddListener(Hide);
            m_BtnNoThank.onClick.AddListener(OnClickNoThank);
            m_BtnLoveIt.onClick.AddListener(OnClickRate);

            Hide();
        }

        private void OnClickNoThank()
        {
            Controllers.Get<ServicesController>().PushEvent(new IEC.TrackingEvent.PopupRatingNoThank());
            Pool.Get<PlayerSecret>().haveAlreadyRate = true;
            Hide();
        }

        private void OnClickRate()
        {
            Controllers.Get<ServicesController>().PushEvent(new IEC.TrackingEvent.PopupRatingLoveIt());
            Pool.Get<PlayerSecret>().haveAlreadyRate = true;

#if UNITY_IOS
        var result = UnityEngine.iOS.Device.RequestStoreReview();
        if(!result) Application.OpenURL("itms-apps://itunes.apple.com/app/id1494648714");
#else
            Application.OpenURL("market://details?id=" + Application.identifier);
#endif
            Hide();
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }

}
