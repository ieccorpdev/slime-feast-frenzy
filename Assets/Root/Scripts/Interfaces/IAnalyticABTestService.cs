using System;

namespace Root
{
    public interface IAnalyticABTestService
    {

        void SetUpTest(ABTestConfig[] configs, int versionCode, Func<bool> funcIsNewInstall);

        void SetLogAnalytic(Action<string> actionLog);

        bool IsActive(string keyNameAB);

        int GetValue(string keyNameAB);

        ABTestConfig GetConfig(string keyNameAB);
        
    }

}

