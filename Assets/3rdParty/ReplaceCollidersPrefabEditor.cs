using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class ReplaceCollidersPrefabEditor : EditorWindow
{
    [MenuItem("Tools/Replace Colliders in Prefabs (BoxCollider)")]
    public static void ShowWindow()
    {
        GetWindow<ReplaceCollidersPrefabEditor>("Replace Colliders");
    }

    void OnGUI()
    {
        GUILayout.Label("Replace Colliders in Selected Prefabs", EditorStyles.boldLabel);

        EditorGUILayout.HelpBox("1. Chọn prefab mà bạn muốn edit trong Project.\n2. Nhấn nút bên dưới để thay Collider bằng BoxCollider.", MessageType.Info);

        if (GUILayout.Button("Replace Colliders in Selected Prefabs"))
        {
            var selected = Selection.objects;

            if (selected == null || selected.Length == 0)
            {
                Debug.LogWarning("⚠️ Bạn chưa chọn prefab nào trong Project.");
                return;
            }

            foreach (Object obj in selected)
            {
                if (obj is GameObject go)
                {
                    string path = AssetDatabase.GetAssetPath(go);

                    if (string.IsNullOrEmpty(path) || !path.EndsWith(".prefab"))
                        continue;

                    GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);

                    ReplaceColliders(prefabRoot);

                    PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
                    PrefabUtility.UnloadPrefabContents(prefabRoot);

                    Debug.Log($"✅ Đã xử lý: {go.name}");
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    static void ReplaceColliders(GameObject root)
    {
        var transforms = root.GetComponentsInChildren<Transform>(true);

        foreach (var t in transforms)
        {
            GameObject go = t.gameObject;

            // Tắt collider cũ
            foreach (var col in go.GetComponents<Collider>())
            {
                col.enabled = false;
            }

            // Thêm BoxCollider nếu có MeshFilter
            MeshFilter mf = go.GetComponent<MeshFilter>();
            if (mf && mf.sharedMesh != null)
            {
                BoxCollider box = go.AddComponent<BoxCollider>();
                box.center = mf.sharedMesh.bounds.center;
                box.size = mf.sharedMesh.bounds.size;
            }
        }
    }
}
#endif