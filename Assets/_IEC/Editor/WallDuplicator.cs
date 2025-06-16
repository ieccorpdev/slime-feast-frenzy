using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;

public class WallDuplicator : EditorWindow
{
    //[MenuItem("Tools/Duplicate Wall Objects")]
    //private static void DuplicateWallObjects()
    //{
    //    // Get the current prefab stage
    //    var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
    //    if (prefabStage == null)
    //    {
    //        Debug.LogWarning("Please open a prefab to use this tool!");
    //        return;
    //    }

    //    // Get the root GameObject of the prefab
    //    GameObject prefabRoot = prefabStage.prefabContentsRoot;

    //    // Find the first child with "Wall" in its name
    //    GameObject wallObject = FindWallInChildren(prefabRoot);

    //    if (wallObject == null)
    //    {
    //        Debug.LogWarning("No object containing [Wall] in its name was found in the prefab!");
    //        return;
    //    }

    //    string prefabName = prefabRoot.name.ToLower();
    //    var bias = 1f;
    //    // if (prefabName.Contains("outer"))
    //    // {
    //    //     bias = 0.75f;
    //    // }
        
    //    // Create duplicate with X offset
    //    GameObject xOffsetCopy = Object.Instantiate(wallObject, wallObject.transform.parent);
    //    xOffsetCopy.name = wallObject.name + "_X";
    //    xOffsetCopy.transform.position = wallObject.transform.position + Vector3.right * bias;

    //    // Create duplicate with Z offset
    //    GameObject zOffsetCopy = Object.Instantiate(wallObject, wallObject.transform.parent);
    //    zOffsetCopy.name = wallObject.name + "_Z";
    //    zOffsetCopy.transform.position = wallObject.transform.position + Vector3.forward * bias;

    //    // Register the operation for undo
    //    Undo.RegisterCreatedObjectUndo(xOffsetCopy, "Create X-offset wall");
    //    Undo.RegisterCreatedObjectUndo(zOffsetCopy, "Create Z-offset wall");

    //    // Mark the prefab as dirty to ensure changes are saved
    //    EditorSceneManager.MarkSceneDirty(prefabStage.scene);
    //}

    private static GameObject FindWallInChildren(GameObject parent)
    {
        // Check if the current object has "Wall" in its name
        if (parent.name.Contains("[Wall]"))
            return parent;

        // Recursively search children
        foreach (Transform child in parent.transform)
        {
            GameObject result = FindWallInChildren(child.gameObject);
            if (result != null)
                return result;
        }

        return null;
    }
}