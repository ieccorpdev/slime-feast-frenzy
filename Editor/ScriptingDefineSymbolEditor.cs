using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace _IECModules
{
    public class ScriptingDefineSymbolEditor
    {
        public static bool ContainDefineSymbol(string symbol)
        {
            return GetListDefineSymbol().Contains(symbol);
        }

        public static void AddDefineSymbol(string symbol)
        {
            var allDefines = GetListDefineSymbol();
            if(allDefines.Contains(symbol)) return;
            allDefines.Add(symbol);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));
        }

        public static List<string> GetListDefineSymbol()
        {
            var definesString =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            return definesString.Split(';').Select(x => x.Trim()).ToList();
        }
    }
}