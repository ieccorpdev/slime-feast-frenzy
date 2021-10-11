using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupRating : MonoBehaviour, IUIGroup
    {
        [SerializeField]
        private Button m_BtnClose, m_BtnNoThank, m_BtnLoveIt;

        public void OnInit()
        {
            m_BtnClose.onClick.AddListener(() => {
                Hide(true);
            });
            m_BtnNoThank.onClick.AddListener(OnClickNoThank);
            m_BtnLoveIt.onClick.AddListener(OnClickRate);

            Hide(false);
        }

        private void OnClickNoThank()
        {
            Controllers.Get<ServicesController>().PushEvent(new IEC.TrackingEvent.PopupRatingNoThank());
            Pool.Get<PlayerSecret>().haveAlreadyRate = false;
            Hide(true);
        }

        private void OnClickRate()
        {
            Controllers.Get<ServicesController>().PushEvent(new IEC.TrackingEvent.PopupRatingLoveIt());
            Pool.Get<PlayerSecret>().haveAlreadyRate = true;
            Controllers.Get<ServicesController>().RateUs();
            Hide(true);
        }

        private void Hide(bool isAction)
        {
            gameObject.SetActive(false);
        }
    }

}
