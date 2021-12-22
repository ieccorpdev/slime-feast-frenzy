using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PluginLocalization
using I2.Loc;
#endif

namespace IEC
{
    public static class LocalizationWrapper
    {
        public static void ChangeLanguage(string language)
        {
#if PluginLocalization
            if (!LocalizationManager.HasLanguage(language)) return;
            LocalizationManager.CurrentLanguage = language;
#endif
        }

        public static string GetTranslation(string term)
        {
#if PluginLocalization
            return LocalizationManager.GetTranslation(term);
#else
            return term;
#endif
        }
    }
}