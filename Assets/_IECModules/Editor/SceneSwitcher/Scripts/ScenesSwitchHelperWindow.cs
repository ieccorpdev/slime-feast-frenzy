using System;
using System.Collections;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

//using Unity.EditorCoroutines.Editor;
namespace IEC
{
    public class SceneSwitcherWindow : EditorWindow
    {
        private static SerializableScenesContainer __scenesContainer;

        private static SerializableScenesContainer _scenesContainer
        {
            get
            {
                if (__scenesContainer == null)
                    __scenesContainer = initContainer();
                return __scenesContainer;
            }
            set { __scenesContainer = value; }
        }

        private static SerializableScenesContainer _prefabContainer;
        private const string _developerName = "IEC";
        private const string _productName = "Scene Switcher";
        private const string _pointersName = "PointerToSceneSwitcherDB";
        private const string _scenesContainerName = "DefaultScenesContainer.asset";

        private static SerializableScenesContainer initContainer()
        {
            string[] searchResult = AssetDatabase.FindAssets(_pointersName);
            if (searchResult.Length == 0)
            {
                Debug.LogError("[" + _developerName + "][" + _productName + "] " + _pointersName + " wasn't found!");
                return null;
            }

            string pointersGUID = searchResult[0];
            string pathToPointer = AssetDatabase.GUIDToAssetPath(pointersGUID);
            string pathToDB =
                pathToPointer.Substring(0, pathToPointer.Length - _pointersName.Length - ".bytes".Length) +
                _scenesContainerName;
            var result =
                (SerializableScenesContainer) AssetDatabase.LoadAssetAtPath(pathToDB,
                    typeof(SerializableScenesContainer));
            if (result == null)
            {
                result = CreateInstance<SerializableScenesContainer>();
                result.SceneAssets = new List<SceneAsset>();
                for (int i = 0; i < 10; i++)
                    result.SceneAssets.Add(null);
                AssetDatabase.CreateAsset(result, pathToDB);
                AssetDatabase.SaveAssets();
            }

            return result;
        }

        private static void FindWindow()
        {
            var instance = GetWindow<SceneSwitcherWindow>();
            GUIContent title = new GUIContent();
            title.text = _productName;
            instance.titleContent = title;
        }


        private static string CurrentScene
        {
            get => EditorPrefs.GetString("currentScene", null);
            set => EditorPrefs.SetString("currentScene", value);
        }

        //MeoMew rem code : Use hot key to switch and run directly a scene game
        //[MenuItem(_developerName + "/" + _productName + "/" + "Play Game #r")] public static void RunMainScene() {
        //	if (!EditorApplication.isPlaying) {
        //		CurrentScene = SceneManager.GetActiveScene().name;
        //		if (SwitchToGameScene()) {
        //			EditorApplication.EnterPlaymode();
        //		}
        //	}
        //	else {
        //		EditorApplication.ExitPlaymode();
        //		if (!string.IsNullOrEmpty(CurrentScene)) {
        //			EditorWindow editorWindow = CreateInstance(typeof(SceneSwitcherWindow)) as EditorWindow;
        //			//EditorCoroutineUtility.StartCoroutine(IEOpenScene(CurrentScene), editorWindow);
        //		}
        //	}
        //}

        private static string GetPathOfScene(string scene)
        {
            foreach (SceneAsset sceneAsset in _scenesContainer.SceneAssets)
            {
                if (sceneAsset != null)
                {
                    if (sceneAsset.name.Equals(scene))
                    {
                        return AssetDatabase.GetAssetPath(sceneAsset);
                    }
                }
            }

            return null;
        }

        private static IEnumerator IEOpenScene(string scene)
        {
            yield return null;
            while (EditorApplication.isPlaying)
            {
                yield return null;
            }

            string scenePath = GetPathOfScene(scene);
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }
        [MenuItem(_developerName + "/" + _productName + "/Enable SceneSwitch ")]
        public static void ChangeStatusSceneSwitchT()
        {
            _scenesContainer.activeSceneSwitched = true;
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }
        [MenuItem(_developerName + "/" + _productName + "/Disable SceneSwitch ")]
        public static void ChangeStatusSceneSwitchF()
        {
            _scenesContainer.activeSceneSwitched = false;
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }
        
        [MenuItem(_developerName + "/" + _productName + "/Edit #e")]
        public static void Init()
        {
            FindWindow();
        }
        #region Switch Scenes

        [MenuItem(_developerName + "/" + _productName + "/Switch to Game Scene #g")]
        public static bool SwitchToGameScene()
        {
            if (!_scenesContainer.activeSceneSwitched) return false;
            int indexScene = 0;
            if (!HasScene(indexScene))
            {
                Debug.LogError("[ScenesSwitch] Cannot find scene!. Please add this scene!");
                return false;
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(_scenesContainer.SceneAssets[indexScene]),
                OpenSceneMode.Single);
            return true;
        }

        [MenuItem(_developerName + "/" + _productName + "/Switch to Game Scene 01 #1")]
        public static void SwitchToGameScene01()
        {
            if (!_scenesContainer.activeSceneSwitched) return;
            int indexScene = 1;
            if (!HasScene(indexScene))
            {
                Debug.LogError("[ScenesSwitch] Cannot find scene!. Please add this scene!");
                return;
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(_scenesContainer.SceneAssets[indexScene]),
                OpenSceneMode.Single);
        }

        [MenuItem(_developerName + "/" + _productName + "/Switch to Game Scene 02 #2")]
        public static void SwitchToGameScene02()
        {
            if (!_scenesContainer.activeSceneSwitched) return;
            int indexScene = 2;
            if (!HasScene(indexScene))
            {
                Debug.LogError("[ScenesSwitch] Cannot find scene!. Please add this scene!");
                return;
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(_scenesContainer.SceneAssets[indexScene]),
                OpenSceneMode.Single);
        }

        [MenuItem(_developerName + "/" + _productName + "/Switch to Game Scene 03 #3")]
        public static void SwitchToGameScene03()
        {
            if (!_scenesContainer.activeSceneSwitched) return;
            int indexScene = 3;
            if (!HasScene(indexScene))
            {
                Debug.LogError("[ScenesSwitch] Cannot find scene!. Please add this scene!");
                return;
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(_scenesContainer.SceneAssets[indexScene]),
                OpenSceneMode.Single);
        }

        [MenuItem(_developerName + "/" + _productName + "/Switch to Game Scene 04 #4")]
        public static void SwitchToGameScene04()
        {
            if (!_scenesContainer.activeSceneSwitched) return;
            int indexScene = 4;
            if (!HasScene(indexScene))
            {
                Debug.LogError("[ScenesSwitch] Cannot find scene!. Please add this scene!");
                return;
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(_scenesContainer.SceneAssets[indexScene]),
                OpenSceneMode.Single);
        }

        [MenuItem(_developerName + "/" + _productName + "/Switch to Game Scene 05 #5")]
        public static void SwitchToGameScene05()
        {
            if (!_scenesContainer.activeSceneSwitched) return;
            int indexScene = 5;
            if (!HasScene(indexScene))
            {
                Debug.LogError("[ScenesSwitch] Cannot find scene!. Please add this scene!");
                return;
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(_scenesContainer.SceneAssets[indexScene]),
                OpenSceneMode.Single);
        }

        [MenuItem(_developerName + "/" + _productName + "/Switch to Game Scene 06 #6")]
        public static void SwitchToGameScene06()
        {
            if (!_scenesContainer.activeSceneSwitched) return;
            int indexScene = 6;
            if (!HasScene(indexScene))
            {
                Debug.LogError("[ScenesSwitch] Cannot find scene!. Please add this scene!");
                return;
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(_scenesContainer.SceneAssets[indexScene]),
                OpenSceneMode.Single);
        }

        [MenuItem(_developerName + "/" + _productName + "/Switch to Game Scene 07 #7")]
        public static void SwitchToGameScene07()
        {
            if (!_scenesContainer.activeSceneSwitched) return;
            int indexScene = 7;
            if (!HasScene(indexScene))
            {
                Debug.LogError("[ScenesSwitch] Cannot find scene!. Please add this scene!");
                return;
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(_scenesContainer.SceneAssets[indexScene]),
                OpenSceneMode.Single);
        }

        [MenuItem(_developerName + "/" + _productName + "/Switch to Game Scene 08 #8")]
        public static void SwitchToGameScene08()
        {
            if (!_scenesContainer.activeSceneSwitched) return;
            int indexScene = 8;
            if (!HasScene(indexScene))
            {
                Debug.LogError("[ScenesSwitch] Cannot find scene!. Please add this scene!");
                return;
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(_scenesContainer.SceneAssets[indexScene]),
                OpenSceneMode.Single);
        }

        [MenuItem(_developerName + "/" + _productName + "/Switch to Game Scene 09 #9")]
        public static void SwitchToGameScene09()
        {
            if (!_scenesContainer.activeSceneSwitched) return;
            int indexScene = 9;
            if (!HasScene(indexScene))
            {
                Debug.LogError("[ScenesSwitch] Cannot find scene!. Please add this scene!");
                return;
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(_scenesContainer.SceneAssets[indexScene]),
                OpenSceneMode.Single);
        }

        #endregion

        #region Prefab Link

        [MenuItem(_developerName + "/" + _productName + "/Switch to Prefab 1 #&1")]
        public static void SwitchToPrefab_0()
        {
            if (!_scenesContainer.activeSceneSwitched) return;
            int indexPrefab = 0;
            if (!HasPrefab(indexPrefab))
            {
                Debug.LogError("[PrefabSwitch] Cannot find Prefab!. Please add this Prefab!");
                return;
            }

            AssetDatabase.OpenAsset(_scenesContainer.PrefabAsset[indexPrefab]);
        }

        [MenuItem(_developerName + "/" + _productName + "/Switch to Prefab 2 #&2")]
        public static void SwitchToPrefab_1()
        {
            if (!_scenesContainer.activeSceneSwitched) return;
            int indexPrefab = 1;
            if (!HasPrefab(indexPrefab))
            {
                Debug.LogError("[PrefabSwitch] Cannot find Prefab!. Please add this Prefab!");
                return;
            }

            AssetDatabase.OpenAsset(_scenesContainer.PrefabAsset[indexPrefab]);
        }

        [MenuItem(_developerName + "/" + _productName + "/Switch to Prefab 3 #&3")]
        public static void SwitchToPrefab_2()
        {
            if (!_scenesContainer.activeSceneSwitched) return;
            int indexPrefab = 2;
            if (!HasPrefab(indexPrefab))
            {
                Debug.LogError("[PrefabSwitch] Cannot find Prefab!. Please add this Prefab!");
                return;
            }

            AssetDatabase.OpenAsset(_scenesContainer.PrefabAsset[indexPrefab]);
        }

        [MenuItem(_developerName + "/" + _productName + "/Switch to Prefab 4 #&4")]
        public static void SwitchToPrefab_3()
        {
            if (!_scenesContainer.activeSceneSwitched) return;
            int indexPrefab = 3;
            if (!HasPrefab(indexPrefab))
            {
                Debug.LogError("[PrefabSwitch] Cannot find Prefab!. Please add this Prefab!");
                return;
            }

            AssetDatabase.OpenAsset(_scenesContainer.PrefabAsset[indexPrefab]);
        }

        [MenuItem(_developerName + "/" + _productName + "/Switch to Prefab 5 #&5")]
        public static void SwitchToPrefab_4()
        {
            if (!_scenesContainer.activeSceneSwitched) return;
            int indexPrefab = 4;
            if (!HasPrefab(indexPrefab))
            {
                Debug.LogError("[PrefabSwitch] Cannot find Prefab!. Please add this Prefab!");
                return;
            }

            AssetDatabase.OpenAsset(_scenesContainer.PrefabAsset[indexPrefab]);
        }

        [MenuItem(_developerName + "/" + _productName + "/Switch to Prefab 6 #&6")]
        public static void SwitchToPrefab_5()
        {
            if (!_scenesContainer.activeSceneSwitched) return;
            int indexPrefab = 5;
            if (!HasPrefab(indexPrefab))
            {
                Debug.LogError("[PrefabSwitch] Cannot find Prefab!. Please add this Prefab!");
                return;
            }

            AssetDatabase.OpenAsset(_scenesContainer.PrefabAsset[indexPrefab]);
        }

        [MenuItem(_developerName + "/" + _productName + "/Switch to Prefab 7 #&7")]
        public static void SwitchToPrefab_6()
        {
            if (!_scenesContainer.activeSceneSwitched) return;
            int indexPrefab = 6;
            if (!HasPrefab(indexPrefab))
            {
                Debug.LogError("[PrefabSwitch] Cannot find Prefab!. Please add this Prefab!");
                return;
            }

            AssetDatabase.OpenAsset(_scenesContainer.PrefabAsset[indexPrefab]);
        }

        [MenuItem(_developerName + "/" + _productName + "/Switch to Prefab 8 #&8")]
        public static void SwitchToPrefab_7()
        {
            if (!_scenesContainer.activeSceneSwitched) return;
            int indexPrefab = 7;
            if (!HasPrefab(indexPrefab))
            {
                Debug.LogError("[PrefabSwitch] Cannot find Prefab!. Please add this Prefab!");
                return;
            }

            AssetDatabase.OpenAsset(_scenesContainer.PrefabAsset[indexPrefab]);
        }

        [MenuItem(_developerName + "/" + _productName + "/Switch to Prefab 9 #&9")]
        public static void SwitchToPrefab_8()
        {
            if (!_scenesContainer.activeSceneSwitched) return;
            int indexPrefab = 8;
            if (!HasPrefab(indexPrefab))
            {
                Debug.LogError("[PrefabSwitch] Cannot find Prefab!. Please add this Prefab!");
                return;
            }

            AssetDatabase.OpenAsset(_scenesContainer.PrefabAsset[indexPrefab]);
        }

        [MenuItem(_developerName + "/" + _productName + "/Switch to Prefab 10 #&0")]
        public static void SwitchToPrefab_9()
        {
            if (!_scenesContainer.activeSceneSwitched) return;
            int indexPrefab = 9;
            if (!HasPrefab(indexPrefab))
            {
                Debug.LogError("[PrefabSwitch] Cannot find Prefab!. Please add this Prefab!");
                return;
            }

            AssetDatabase.OpenAsset(_scenesContainer.PrefabAsset[indexPrefab]);
        }

        #endregion

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("Need active to use ",MessageType.Info);
            bool active = _scenesContainer.activeSceneSwitched;
            _scenesContainer.activeSceneSwitched = EditorGUILayout.Toggle("Active Scene Switch", active);
            EditorUtility.SetDirty(_scenesContainer);
            
            if (_scenesContainer == null)
                return;
            EditorGUILayout.LabelField("---Scene Support---");
            for (int i = 0; i < _scenesContainer.SceneAssets.Count; i++)
            {
                string sceneNumber = (i + 1).ToString();
                string label;
                switch (i)
                {
                    case 0:
                        label = "Game Scene (Shift + G)";
                        break;
                    case 1:
                        label = "Game Scene 1 (Shift + 1)";
                        break;
                    case 2:
                        label = "Game Scene 2 (Shift + 2)";
                        break;
                    case 3:
                        label = "Game Scene 3 (Shift + 3)";
                        break;
                    case 4:
                        label = "Game Scene 4 (Shift + 4)";
                        break;
                    case 5:
                        label = "Game Scene 5 (Shift + 5)";
                        break;
                    case 6:
                        label = "Game Scene 6 (Shift + 6)";
                        break;
                    case 7:
                        label = "Game Scene 7 (Shift + 7)";
                        break;
                    case 8:
                        label = "Game Scene 8 (Shift + 8)";
                        break;
                    case 9:
                        label = "Game Scene 9 (Shift + 9)";
                        break;
                    default:
                        label = "";
                        break;
                }

                SceneAsset newAsset = EditorGUILayout.ObjectField(new GUIContent(label),
                    _scenesContainer.SceneAssets[i], typeof(SceneAsset), false) as SceneAsset;
                if (newAsset != _scenesContainer.SceneAssets[i])
                {
                    _scenesContainer.SceneAssets[i] = newAsset;
                    EditorUtility.SetDirty(_scenesContainer);
                }
            }

            EditorGUILayout.LabelField("---Prefab Support---");
            //Prefab Faster.
            for (int i = 0; i < _scenesContainer.PrefabAsset.Count; i++)
            {
                string label = "Prefab " + (i+1) + " (Shift + Alt +" + (i + 1) % 10 + ")";
                ;
                GameObject newAsset = EditorGUILayout.ObjectField(new GUIContent(label),
                    _scenesContainer.PrefabAsset[i], typeof(GameObject), false) as GameObject;
                if (newAsset != _scenesContainer.PrefabAsset[i])
                {
                    _scenesContainer.PrefabAsset[i] = newAsset;
                    EditorUtility.SetDirty(_scenesContainer);
                }
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear"))
            {
                for (int i = 0; i < _scenesContainer.SceneAssets.Count; i++)
                    _scenesContainer.SceneAssets[i] = null;
                for (int i = 0; i < _scenesContainer.PrefabAsset.Count; i++)
                    _scenesContainer.SceneAssets[i] = null;
                EditorUtility.SetDirty(_scenesContainer);
            }

            GUILayout.EndHorizontal();
        }

        private static bool HasScene(int index)
        {
            if (_scenesContainer.SceneAssets.Count < index + 1)
                return false;
            if (_scenesContainer.SceneAssets[index] == null)
                return false;
            return true;
        }

        private static bool HasPrefab(int index)
        {
            if (_scenesContainer.PrefabAsset.Count < index + 1)
                return false;
            if (_scenesContainer.PrefabAsset[index] == null)
                return false;
            return true;
        }
    }
}
#endif