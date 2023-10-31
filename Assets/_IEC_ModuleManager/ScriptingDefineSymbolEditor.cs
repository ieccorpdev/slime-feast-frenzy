using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace IEC.ModuleManager
{
    public class ScriptingDefineSymbolEditor
    {
        public static bool ContainDefineSymbol(string symbol)
        {
            return GetListDefineSymbol().Contains(symbol);
        }

        public static void AddDefineSymbol(object symbol) => AddDefineSymbol(symbol.ToString());
        public static void RemoveDefineSymbol(object symbol) => RemoveDefineSymbol(symbol.ToString());
        public static void AddDefineSymbol(string symbol)
        {
            var allDefines = GetListDefineSymbol();
            if(allDefines.Contains(symbol)) return;
            allDefines.Add(symbol);
            SetListDefineSymbol(allDefines);
        }

        public static void RemoveDefineSymbol(string symbol)
        {
            var allDefines = GetListDefineSymbol();
            if(!allDefines.Contains(symbol)) return;
            allDefines.Remove(symbol);
            SetListDefineSymbol(allDefines);
        }

        public static void SetListDefineSymbol(List<string> allDefines)
        {
            allDefines.Sort();
            var defineStr = string.Join(";", allDefines.ToArray());
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defineStr);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defineStr);
            AssetDatabase.SaveAssets();
        }
        public static List<string> GetListDefineSymbol()
        {
            var definesString =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            return definesString.Split(';').Select(x => x.Trim()).ToList();
        }
    }
}