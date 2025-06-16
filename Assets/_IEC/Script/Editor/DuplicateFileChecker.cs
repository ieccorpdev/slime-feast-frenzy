using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class DuplicateFileChecker : EditorWindow
{
    private string selectedFolder = "Assets/EncryptedAsset";
    private Dictionary<string, List<string>> fileMap = new Dictionary<string, List<string>>();
    private Vector2 scrollPos;
    private bool hasChecked = false;
    private bool hasDuplicates = false;
    private string statusMessage = "";

    [MenuItem("IEC/Encrypted Asset/Duplicate File Checker")]
    public static void ShowWindow()
    {
        GetWindow<DuplicateFileChecker>("Duplicate File Checker");
    }

    private void OnEnable()
    {
        if (!Directory.Exists("Assets/EncryptedAsset"))
        {
            EditorUtility.DisplayDialog("Warning", "Folder 'Assets/EncryptedAsset' not found! Please select another folder.", "OK");
            selectedFolder = "Assets";
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Duplicate File Checker", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Select Folder"))
        {
            selectedFolder = EditorUtility.OpenFolderPanel("Select Folder", Application.dataPath, "");
            if (!string.IsNullOrEmpty(selectedFolder) && selectedFolder.StartsWith(Application.dataPath))
            {
                selectedFolder = "Assets" + selectedFolder.Substring(Application.dataPath.Length);
            }
        }
        
        GUILayout.Label("Selected Folder: " + selectedFolder, EditorStyles.label);

        if (GUILayout.Button("Check for Duplicates"))
        {
            CheckForDuplicates();
            hasChecked = true;
        }

        if (hasChecked)
        {
            GUILayout.Label(statusMessage, EditorStyles.boldLabel);
            if (hasDuplicates)
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                foreach (var entry in fileMap)
                {
                    if (entry.Value.Count > 1)
                    {
                        GUILayout.Label("Duplicate: " + entry.Key, EditorStyles.boldLabel);
                        foreach (var path in entry.Value)
                        {
                            GUILayout.Label("   " + path, EditorStyles.label);
                        }
                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }
    }

    private void CheckForDuplicates()
    {
        fileMap.Clear();
        hasDuplicates = false;
        string[] files = Directory.GetFiles(selectedFolder, "*", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            if (!fileMap.ContainsKey(fileName))
            {
                fileMap[fileName] = new List<string>();
            }
            fileMap[fileName].Add(file);
        }
        
        foreach (var entry in fileMap)
        {
            if (entry.Value.Count > 1)
            {
                hasDuplicates = true;
                break;
            }
        }
        
        statusMessage = hasDuplicates ? "Duplicate files found:" : "No duplicate files found.";
    }
}
