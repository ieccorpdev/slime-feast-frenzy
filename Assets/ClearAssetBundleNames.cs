#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ClearAssetBundleNames : MonoBehaviour
{
    [MenuItem("Tools/AssetBundles/Clear All Bundle Names")]
    static void ClearAllAssetBundleNames()
    {
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();

        int clearedCount = 0;

        foreach (string path in allAssetPaths)
        {
            if (!path.StartsWith("Assets/")) continue;

            AssetImporter importer = AssetImporter.GetAtPath(path);
            if (importer != null && !string.IsNullOrEmpty(importer.assetBundleName))
            {
                importer.assetBundleName = string.Empty;

                // Chỉ đặt variant nếu name không rỗng
                if (!string.IsNullOrEmpty(importer.assetBundleVariant))
                {
                    importer.assetBundleVariant = string.Empty;
                }

                clearedCount++;
            }
        }

        Debug.Log($"✅ Cleared bundle names from {clearedCount} assets.");

        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.SaveAssets();
    }
}
#endif