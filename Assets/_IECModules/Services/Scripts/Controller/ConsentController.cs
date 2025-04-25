using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ConsentController : MonoBehaviour
    {
        public GameObject canvasConsent;
        public GameObject loadingObj;
        public GameObject panelConsent;

        public Button btnConsentURL;
        public Button btnAccept;

        private IConsentService m_ConsentService;

        // Start is called before the first frame update


        private void Update()
        {
            loadingObj.transform.Rotate(Vector3.back, Time.unscaledDeltaTime * 150f);
        }
    }
}
