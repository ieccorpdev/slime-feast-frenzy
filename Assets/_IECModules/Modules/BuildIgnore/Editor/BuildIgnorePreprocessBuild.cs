using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace IEC.BuildIgnore
{
    public class BuildIgnorePreprocessBuild : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => default;
        static BuildIgnore buildIgnore = new BuildIgnore();


        public void OnPreprocessBuild(BuildReport report)
        {
            buildIgnore.FindFiles();
            buildIgnore.HideFiles();
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            buildIgnore.UnhideFiles();
        }

        [MenuItem("IEC/Show Build Ignore List", priority = 20)]
        public static void ShowListHide()
        {
            Debug.Log("Ignore List:=========================================");
            buildIgnore.FindFiles();
            Debug.Log("Folder: =======================================");
            foreach (var item in buildIgnore.folder)
            {
                Debug.Log(item);
            }
            Debug.Log("Files: ========================================");
            foreach (var item in buildIgnore.files)
            {
                Debug.Log(item);
            }
            Debug.Log("====================================================");
        }
    }
}
