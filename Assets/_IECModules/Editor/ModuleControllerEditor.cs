using UnityEditor;
using UnityEngine;

namespace _IECModules
{
    public class ModuleControllerEditor : Editor
    {
        [MenuItem("IEC/Create Module Controller", false, 0)]
        public static void CreateModuleController()
        {
            const int ModuleControllerExecOrder = -30000;

            var modulePrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_IECModules/IEC Module Controller.prefab");
            var moduleGameObject = (GameObject) PrefabUtility.InstantiatePrefab(modulePrefab);
            moduleGameObject.name = "IEC Module Controller";
            moduleGameObject.transform.SetAsLastSibling();

            var moduleController = moduleGameObject.GetComponent<ModuleController>();
            var monoScript = MonoScript.FromMonoBehaviour(moduleController);
            if (MonoImporter.GetExecutionOrder(monoScript) != ModuleControllerExecOrder)
            {
                MonoImporter.SetExecutionOrder(monoScript, ModuleControllerExecOrder);
            }
        }

        [MenuItem("IEC/Player Prefs Editor", false, 1)]
        public static void CreatePlayerPrefsEditor()
        {
            PlayerPrefsEditor.Init();
        }

        [MenuItem("IEC/Create Prototype Controller", false, 0)]
        public static void CreatePrototypeController()
        {
            var modulePrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_IECModules/_PrototypeControllers.prefab");
            var moduleGameObject = (GameObject)PrefabUtility.InstantiatePrefab(modulePrefab);
            moduleGameObject.name = "_PrototypeControllers";
            moduleGameObject.transform.SetAsFirstSibling();
        }
    }
}