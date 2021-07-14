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
        public void StartGetConsent(ServicesConfig config, Action onGetDone)
        {
            canvasConsent.SetActive(true);
            panelConsent.SetActive(false);
            loadingObj.SetActive(true);

            m_ConsentService = new ConsentMock();
#if (PluginApplovin)
            var applovinConsent = new ApplovinConsentModule();
            applovinConsent.Init(config.ApplovinConfig);
            m_ConsentService = applovinConsent;
#endif

            if (Application.isEditor)
            {
                m_ConsentService = new ConsentMock();
            }

            m_ConsentService.CheckConsent(isHaveConsent =>
            {
                if (isHaveConsent)
                {
                    m_ConsentService.SetAcceptConsent();
                    canvasConsent.SetActive(false);
                    onGetDone?.Invoke();
                }
                else
                {
                    panelConsent.SetActive(true);
                    loadingObj.SetActive(false);

                    btnConsentURL.onClick.RemoveAllListeners();
                    btnConsentURL.onClick.AddListener(() =>
                    {
                        Application.OpenURL("https://privacy.iecglobal.com.au/");
                    });

                    btnAccept.onClick.RemoveAllListeners();
                    btnAccept.onClick.AddListener(() =>
                    {
                        m_ConsentService.SetAcceptConsent();
                        canvasConsent.SetActive(false);
                        onGetDone?.Invoke();
                    });
                }
            });
        }

        private void Update()
        {
            loadingObj.transform.Rotate(Vector3.back, Time.unscaledDeltaTime * 150f);
        }
    }
}
