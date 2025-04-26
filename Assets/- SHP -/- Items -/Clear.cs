using UnityEditor;

public class ClearAssetBundleNames
{
    [MenuItem("Tools/Clear All AssetBundle Names")]
    static void ClearAll()
    {
        var names = AssetDatabase.GetAllAssetBundleNames();
        foreach (var name in names)
        {
            AssetDatabase.RemoveAssetBundleName(name, true); // true = force remove
        }
        AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        UnityEngine.Debug.Log("Cleared all AssetBundle names!");
    }
}
