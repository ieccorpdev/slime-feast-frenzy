#if UNITY_EDITOR
#if ITEM_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using IEC;

[InitializeOnLoad]
public static class ParentSelector
{
    private static readonly string targetSceneName = "Level Editor"; // Replace with the name of your scene

    static ParentSelector()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        if (!IsInTargetScene())
            return;

        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            FilterSelection();
        }
    }

    private static void FilterSelection()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        HashSet<GameObject> newSelection = new HashSet<GameObject>();

        foreach (var obj in selectedObjects)
        {
            Transform current = obj.transform;
            while (current != null)
            {
                if (current.GetComponent<SpawnSettings>() != null)
                {
                    newSelection.Add(current.gameObject);
                    break;
                }
                current = current.parent;
            }
        }

        // Update the selection to include only the parent objects
        Selection.objects = newSelection.ToArray();
    }

    private static bool IsInTargetScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        return activeScene.name == targetSceneName;
    }
}
#endif
#endif