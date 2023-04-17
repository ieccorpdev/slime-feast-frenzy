using System;

namespace IEC
{
    public interface IAnalyticABTestService
    {

        void SetUpTest(ABTestConfig config, int versionCode, bool isNewInstall, Action<string> actionLog);

        bool IsActive(string keyNameAB);

        bool HasValue(string keyNameAB);

        int GetValue(string keyNameAB, int defaultValue);

        void SetValue(string keyNameAB, int value);

        ABTestConfig GetConfig(string keyNameAB);

    }

}

