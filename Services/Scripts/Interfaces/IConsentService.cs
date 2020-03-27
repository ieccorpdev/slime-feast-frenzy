using System;
using System.Collections.Generic;

namespace IEC
{
    public interface IConsentService
    {
        void CheckConsent(Action<bool> onConsentState);
        bool IsHaveConsent();
        void SetAcceptConsent();
    }
}