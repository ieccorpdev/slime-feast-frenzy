using UnityEditor;
using UnityEngine;

public static class ToggleSymbolMenu
{
    private const string HIDE_COLLIDER_SYMBOL = "HIDE_COLLIDER_IN_LEVEL_EDITOR";
    private const string ITEM_EDITOR_SYMBOL = "ITEM_EDITOR";

    [MenuItem("City Swept/Toggle Hide Collider")]
    public static void ToggleHideCollider()
    {
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        if (symbols.Contains(HIDE_COLLIDER_SYMBOL))
        {
            symbols = symbols.Replace(HIDE_COLLIDER_SYMBOL, "");
        }
        else
        {
            symbols += ";" + HIDE_COLLIDER_SYMBOL;
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
        Debug.Log($"Toggled {HIDE_COLLIDER_SYMBOL}. Current symbols: {symbols}");
    }

    [MenuItem("City Swept/Toggle Item Editor")]
    public static void ToggleItemEditor()
    {
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        if (symbols.Contains(ITEM_EDITOR_SYMBOL))
        {
            symbols = symbols.Replace(ITEM_EDITOR_SYMBOL, "");
        }
        else
        {
            symbols += ";" + ITEM_EDITOR_SYMBOL;
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
        Debug.Log($"Toggled {ITEM_EDITOR_SYMBOL}. Current symbols: {symbols}");
    }
}