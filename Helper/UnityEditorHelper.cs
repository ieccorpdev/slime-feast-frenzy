using System.Collections.Generic;
using UnityEngine;

public static class UnityEditorHelper
{
    public static void SetDirtyAndSave(ScriptableObject scriptObj)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(scriptObj);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
}