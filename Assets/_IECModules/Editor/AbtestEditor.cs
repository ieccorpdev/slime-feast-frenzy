using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

public class AbtestEditor : EditorWindow
{

    public static string ProjectPath => System.IO.Directory.GetCurrentDirectory();
    public static string ScriptPath => GetThisFilePath();

    public static string FolderScriptPath => Path.GetDirectoryName(ScriptPath);
    public static string ABtesetPath => ProjectPath + "/Assets/_IEC/Script/Controller/ABTestController.cs";
    public static string ABtesetSamplePath => FolderScriptPath + "/AbtestSample.txt";

    private static string GetThisFilePath([CallerFilePath] string path = null)
    {
        return path;
    }


    string m_abName;

    [MenuItem("IEC/AbtestEditor", priority = 11)]
    private static void CallCreateWindow()
    {
        // Get existing open window or if none, make a new one:
        AbtestEditor window = (AbtestEditor)EditorWindow.GetWindow(typeof(AbtestEditor));
        window.autoRepaintOnSceneChange = true;
        window.titleContent.text = "AbtestEditor";
        window.Show();


    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins);
        m_abName = EditorGUILayout.TextField(m_abName);
        if (GUILayout.Button("Test", EditorStyles.miniButton))
        {
            Debug.LogError(FolderScriptPath);
            string abSampleContent = File.ReadAllText(ABtesetSamplePath);
            Debug.LogError(abSampleContent);
            string abContent = File.ReadAllText(ABtesetPath);
            List<int> indexFound = AllIndexesOf(abContent, " ResultAb");
            foreach (var item in indexFound)
            {
                Debug.LogError(abContent.Substring(item, 17));
            }
            List<int> indexFound2 = AllIndexesOf(abContent, "}");
            int indexOfEndClass = indexFound2[indexFound2.Count - 2];
            abContent = abContent.Remove(indexOfEndClass, abContent.Length - indexOfEndClass);

            abSampleContent = abSampleContent.Replace("{0}", m_abName);
            abContent += abSampleContent;
            //abContent += "\n}\n}";
            File.WriteAllText(ABtesetPath, abContent);
            AssetDatabase.Refresh();
            // Debug.LogError(abContent);
        }
        EditorGUILayout.EndVertical();
    }

    public static List<int> AllIndexesOf(string str, string value)
    {
        if (String.IsNullOrEmpty(value))
            throw new ArgumentException("the string to find may not be empty", "value");
        List<int> indexes = new List<int>();
        for (int index = 0; ; index += value.Length)
        {
            index = str.IndexOf(value, index);
            if (index == -1)
                return indexes;
            indexes.Add(index);
        }
    }
}
