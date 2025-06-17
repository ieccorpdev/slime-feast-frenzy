using System.IO;
using UnityEditor;
using UnityEngine;

namespace SuperPack
{
#if UNITY_EDITOR
    public class SuperPackRemoveMissingComponentsTab
    {
        [MenuItem("Tools/Super Pack/Remove Missing Components", false, 3),
        MenuItem("GameObject/Super Pack/Remove Missing Components", false, 3)]
        [System.Obsolete]
        static void RemoveMissingComponents()
        {
            RemoveMissingComponentsInPrefabs();
            RemoveMissingComponentsInCurrentScene();
            Debug.LogFormat("<color=green>ALL DONE!</color>");
        }

        [System.Obsolete]
        static void RemoveMissingComponentsInCurrentScene()
        {
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
            int count = 0;
            foreach (GameObject go in allObjects)
            {
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                count++;
            }
        }

        static void RemoveMissingComponentsInPrefabs()
        {
            string[] prefabPaths = Directory.GetFiles("Assets", "*.prefab", SearchOption.AllDirectories);

            foreach (string prefabPath in prefabPaths)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                Transform[] allTransformsInPrefab = prefab.GetComponentsInChildren<Transform>(true);

                foreach (Transform t in allTransformsInPrefab)
                {
                    GameObject go = t.gameObject;
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                }
            }
        }
    }
#endif
}
