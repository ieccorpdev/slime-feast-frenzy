using UnityEngine;
using UnityEditor;


public class DuplicateColliderObject : EditorWindow
{
    [MenuItem("Tools/Duplicate Wall Collider Object", true)]
    private static bool ValidateMenuItem()
    {
        // Only enable menu item when in prefab mode
        return UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null;
    }

    [MenuItem("Tools/Duplicate Wall Collider Object")]
    public static void DuplicateColliderInCurrentPrefab()
    {
        var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage == null)
        {
            Debug.LogWarning("Please open a prefab in Prefab Mode first!");
            return;
        }

        GameObject prefabRoot = prefabStage.prefabContentsRoot;
        ProcessPrefab(prefabRoot);
    }

    private static void ProcessPrefab(GameObject prefabRoot)
    {
        // Find first object with collider in prefab
        Collider[] colliders = prefabRoot.GetComponentsInChildren<Collider>();
        
        if (colliders.Length == 0)
        {
            Debug.LogWarning("No colliders found in the prefab!");
            return;
        }

        // Get the first gameobject with collider
        GameObject objectWithCollider = colliders[0].gameObject;
        
        // Create a duplicate
        GameObject duplicate = Instantiate(objectWithCollider);
        
        // Set the duplicate's parent to be the same as the original
        duplicate.transform.SetParent(objectWithCollider.transform.parent);
        
        // Set the same position as original
        duplicate.transform.position = objectWithCollider.transform.position;
        duplicate.transform.rotation = objectWithCollider.transform.rotation;
        duplicate.transform.localScale = objectWithCollider.transform.localScale;
        
        // Rename with [Wall] prefix
        duplicate.name = "[Wall]" + objectWithCollider.name;
        duplicate.layer = LayerMask.NameToLayer("Wall");
        
        Component[] components = duplicate.GetComponents<Component>();
        foreach (Component component in components)
        {
            // Skip if it's a Transform or Collider
            if (component is Transform || component is Collider)
                continue;

            // Register the component destruction for undo
            Undo.DestroyObjectImmediate(component);
        }

        string prefabName = prefabRoot.name.ToLower();
        if (prefabName.Contains("straight"))
        {
            duplicate.transform.localPosition = objectWithCollider.transform.localPosition + new Vector3(0f, 0f, -0.85f);
        }
        else if(prefabName.Contains("corner")) //ONLY DO THIS ON OUTER
        {
            // Keep original position if not "straight"
            duplicate.transform.localPosition = objectWithCollider.transform.localPosition + new Vector3(-0.85f, 0f, -0.85f);
        }
        
        Selection.activeObject = duplicate;
        
        // Register the creation for undo
        Undo.RegisterCreatedObjectUndo(duplicate, "Duplicate Collider Object");
        
        Debug.Log($"Created duplicate object: {duplicate.name}");
    }
}