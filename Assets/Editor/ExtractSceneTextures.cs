using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ExtractSceneTextures
{
    [MenuItem("Tools/Extract Textures to Images Folder")]
    public static void ExtractTextures()
    {
        string targetFolder = "Assets/Images";
        string editorImagesFolder = "Assets/Editor/eImages";

        // Ensure target folders exist
        if (!AssetDatabase.IsValidFolder(targetFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Images");
        }
        if (!AssetDatabase.IsValidFolder(editorImagesFolder))
        {
            AssetDatabase.CreateFolder("Assets/Editor", "eImages");
        }

        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>(true);
        List<GameObject> validObjects = new List<GameObject>();

        // Lọc GameObject không dùng ScriptableObject
        foreach (GameObject go in allObjects)
        {
            Object[] goDependencies = EditorUtility.CollectDependencies(new Object[] { go });
            bool hasScriptableObject = false;
            foreach (Object dep in goDependencies)
            {
                if (dep is ScriptableObject)
                {
                    hasScriptableObject = true;
                    break;
                }
            }
            if (!hasScriptableObject)
            {
                validObjects.Add(go);
            }
        }

        // Lấy tất cả dependencies từ các GameObject hợp lệ
        Object[] dependencies = EditorUtility.CollectDependencies(validObjects.ToArray());

        HashSet<string> movedPaths = new HashSet<string>();
        string[] validExtensions = { ".png", ".jpg", ".jpeg", ".tga", ".psd", ".gif", ".bmp", ".tiff", ".exr", ".hdr" };

        // Move textures referenced by valid GameObjects to Images or Editor/eImages
        foreach (Object obj in dependencies)
        {
            if (!(obj is Texture2D texture)) continue;

            string assetPath = AssetDatabase.GetAssetPath(texture);
            if (string.IsNullOrEmpty(assetPath)) continue;
            if (!assetPath.StartsWith("Assets/")) continue;

            // Only allow valid image extensions
            string extension = Path.GetExtension(assetPath).ToLowerInvariant();
            if (System.Array.IndexOf(validExtensions, extension) < 0) continue;

            if (!movedPaths.Add(assetPath)) continue;

            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            string guid = AssetDatabase.AssetPathToGUID(assetPath).Substring(0, 6);
            string newFileName = $"{fileName}_{guid}{extension}";
            string destPath = Path.Combine(targetFolder, newFileName);

            if (!File.Exists(destPath))
            {
                string moveResult = AssetDatabase.MoveAsset(assetPath, destPath);
                if (!string.IsNullOrEmpty(moveResult))
                {
                    Debug.LogError($"Failed to move: {assetPath} → {destPath}\n{moveResult}");
                }
            }
            else
            {
                Debug.Log($"Already exists: {destPath}");
            }
        }

        // Now move all remaining Texture2D assets in Assets (not in Images) to Editor/eImages
        string[] allTextureGuids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets" });
        int editorMovedCount = 0;
        foreach (string guid in allTextureGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // Skip if already moved
            if (movedPaths.Contains(assetPath)) continue;
            if (string.IsNullOrEmpty(assetPath)) continue;
            if (!assetPath.StartsWith("Assets/")) continue;

            // Only allow valid image extensions
            string extension = Path.GetExtension(assetPath).ToLowerInvariant();
            if (System.Array.IndexOf(validExtensions, extension) < 0) continue;

            // Skip if already in Images or Editor/eImages
            if (assetPath.StartsWith(targetFolder) || assetPath.StartsWith(editorImagesFolder)) continue;

            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            string shortGuid = guid.Substring(0, 6);
            string newFileName = $"{fileName}_{shortGuid}{extension}";
            string destPath = Path.Combine(editorImagesFolder, newFileName);

            if (!File.Exists(destPath))
            {
                string moveResult = AssetDatabase.MoveAsset(assetPath, destPath);
                if (!string.IsNullOrEmpty(moveResult))
                {
                    Debug.LogError($"Failed to move: {assetPath} → {destPath}\n{moveResult}");
                }
                else
                {
                    editorMovedCount++;
                }
            }
            else
            {
                Debug.Log($"Already exists: {destPath}");
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"Done: Moved {movedPaths.Count} scene textures and {editorMovedCount} remaining textures to Editor/eImages.");
    }
}
