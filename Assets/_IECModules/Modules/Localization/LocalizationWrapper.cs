using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if PluginLocalization
using I2.Loc;
#endif

namespace IEC
{
    public class LocalizeContent
    {
        public string content;
        public bool isLocalize;

        public LocalizeContent() { }
        public LocalizeContent(string content, bool isLocalize)
        {
            this.content = content;
            this.isLocalize = isLocalize;
        }
    }

    public static class LocalizationWrapper
    {
        public class TextTransInfo
        {
            public TMPro.TMP_Text textTrans;
            public List<LocalizeContent> listContent = new List<LocalizeContent>();

            public TextTransInfo() { }
            public TextTransInfo(TMPro.TMP_Text textTrans, string term, string prefix, string suffix)
            {
                this.textTrans = textTrans;
                listContent = new List<LocalizeContent> {
                    new LocalizeContent(prefix, false),
                    new LocalizeContent(term, true),
                    new LocalizeContent(suffix, false),
                };
            }

            public TextTransInfo(TMPro.TMP_Text textTrans, List<LocalizeContent> listContent)
            {
                this.textTrans = textTrans;
                this.listContent = listContent;
            }

            public void Localize()
            {
                var str = string.Empty;
                for (int i = 0; i < listContent.Count; i++)
                {
                    var a = listContent[i];
#if PluginLocalization
                    str += a.isLocalize ? LocalizationManager.GetTranslation(a.content) : a.content;

#else
                    str += a.content;
#endif
                }
                textTrans.text = str;
            }
        }

        public static List<TextTransInfo> listTextTransInfo = new List<TextTransInfo>();

        public static string CurrentLanguage
        {
            get
            {
#if PluginLocalization
                return LocalizationManager.CurrentLanguage;
#else
                return "";
#endif
            }
        }

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

        public static void Translation(this TMPro.TMP_Text text, string term, string suffix = "", string prefix = "")
        {
            Translation(text, new List<LocalizeContent>()
            {
                new LocalizeContent { content = prefix, isLocalize = false },
                new LocalizeContent { content = term, isLocalize = true },
                new LocalizeContent { content = suffix, isLocalize = false }
            });
        }

        public static void Translation(this TMPro.TMP_Text text, List<LocalizeContent> listContent)
        {
            var txtTransInfo = listTextTransInfo.Find(x => x.textTrans == text);
            if (txtTransInfo != null)
            {
                txtTransInfo.listContent = listContent;
            }
            else
            {
                txtTransInfo = new TextTransInfo(text, listContent);
                listTextTransInfo.Add(txtTransInfo);

            }
            txtTransInfo.Localize();
        }

        public static void LocalizeAll(bool isForce = false)
        {
#if PluginLocalization
            LocalizationManager.LocalizeAll(isForce);
            foreach (var item in listTextTransInfo)
            {
                item.Localize();
            }
#endif
        }
    }
}