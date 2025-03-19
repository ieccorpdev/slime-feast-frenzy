using UnityEngine;
using UnityEditor;

public class FBXToAsset : MonoBehaviour
{
    #if UNITY_EDITOR
    [MenuItem("Tools/Convert FBX to Asset")]
    public static void ConvertFBXToAsset()
    {
        // Lấy đối tượng hiện tại đang được chọn
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject == null)
        {
            Debug.LogError("No GameObject selected. Please select a GameObject in the scene.");
            return;
        }

        // Kiểm tra MeshFilter
        MeshFilter meshFilter = selectedObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("Selected GameObject does not contain a MeshFilter component.");
            return;
        }

        // Lấy Mesh từ MeshFilter
        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            Debug.LogError("No mesh found on the selected GameObject.");
            return;
        }

        // Tạo đường dẫn file
        string assetName = selectedObject.name + "_Mesh.asset";
        string assetPath = $"Assets/{assetName}";

        // Kiểm tra nếu file đã tồn tại
        if (AssetDatabase.LoadAssetAtPath<Mesh>(assetPath) != null)
        {
            Debug.LogWarning($"File already exists at path: {assetPath}. Overwriting.");
        }

        // Lưu Mesh thành .asset
        AssetDatabase.CreateAsset(Object.Instantiate(mesh), assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Mesh saved successfully to: {assetPath}");
    }
    #endif
}