using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sabresaurus.PlayerPrefsExtensions;
using UnityEditor;
using UnityEngine;

namespace IEC
{
    public class ConvertToolEditor : EditorWindow
    {
        private static string m_FileExtStr = ".txt;.json";
        private static string m_EncryptSrcPath, m_EncryptDstPath;
        private static string m_DecryptSrcPath, m_DecryptDstPath;
        private static Vector2 buttonSize = new Vector2(300, 20);
        
        private GUIStyle m_StyleHeader1, m_StyleHeader2;
        [MenuItem("IEC/Convert Tool")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            GetWindow(typeof(ConvertToolEditor), false, $"Convert Tool");
        }

        private void Awake()
        {
            m_DecryptSrcPath = m_EncryptDstPath = Path.Combine(Application.dataPath, "Encrypted");
            m_DecryptDstPath = Path.Combine(Application.dataPath, "Decrypted");
        }

        private void OnGUI()
        {
            InitGuiParams();
           
            CreateGuiLabel("Extension", m_StyleHeader1);
            m_FileExtStr = EditorGUILayout.TextField(m_FileExtStr, GUILayout.Width(EditorGUIUtility.currentViewWidth - 100));
            CreateGuiBreakLine();
            CreateGuiLabel("Path", m_StyleHeader1);
            CreateGuiLabel("Encrypt Source Path", m_StyleHeader2);
            m_EncryptSrcPath = EditorGUILayout.TextField(m_EncryptSrcPath);
            CreateGuiButton("Select", () =>
            {
                m_EncryptSrcPath = EditorUtility.OpenFolderPanel("Select Directory", "", "");
            }, 150);
            CreateGuiBreakLine();
            CreateGuiLabel("Encrypt Dest Path", m_StyleHeader2);
            m_EncryptDstPath = EditorGUILayout.TextField(m_EncryptDstPath);
            CreateGuiButton("Select", () =>
            {
                m_EncryptDstPath = EditorUtility.OpenFolderPanel("Select Directory", "", "");
                m_DecryptSrcPath = m_EncryptDstPath;
            }, 150);
            CreateGuiBreakLine();
            CreateGuiButton("Encrypt", () =>
            {
                var listExt = m_FileExtStr.Split(";").ToList();
                ConvertTool.EncryptEditor(m_EncryptSrcPath, m_EncryptDstPath, listExt);
            }); 
            CreateGuiBreakLine();
            CreateGuiLabel("Test", m_StyleHeader1);
            CreateGuiLabel("Decrypt Source Path", m_StyleHeader2);
            m_DecryptSrcPath = EditorGUILayout.TextField(m_DecryptSrcPath);
            CreateGuiButton("Select", () =>
            {
                m_DecryptSrcPath = EditorUtility.OpenFolderPanel("Select Directory", "", "");
            }, 150);
            CreateGuiBreakLine();
            CreateGuiLabel("Decrypt Dest Path", m_StyleHeader2);
            m_DecryptDstPath = EditorGUILayout.TextField(m_DecryptDstPath);
            CreateGuiButton("Select", () =>
            {
                m_DecryptDstPath = EditorUtility.OpenFolderPanel("Select Directory", "", "");
            }, 150);
            CreateGuiBreakLine();
            CreateGuiButton("Decrypt", () =>
            {
                var listExt = m_FileExtStr.Split(";").ToList();
                ConvertTool.DecryptEditor(m_DecryptSrcPath, m_DecryptDstPath, listExt);
            }); 
        }
        
        private void InitGuiParams()
        {
            m_StyleHeader1 ??= new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold };
            m_StyleHeader2 ??= new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold };
        }
        
        private void CreateGuiLabel(string text, GUIStyle guiStyle)
        {
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            GUILayout.Label(text, guiStyle);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
        }
        
        private void CreateGuiButton(string text, Action onClickButton) =>
            CreateGuiButton(text, onClickButton, buttonSize.x);

        private void CreateGuiButton(string text, Action onClickButton, float buttonWidth)
        {
            if (!GUILayout.Button(text, GUILayout.Width(buttonWidth), GUILayout.Height(buttonSize.y))) return;
            onClickButton?.Invoke();
        }
        
        private void CreateGuiBreakLine()
        {
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
        }
    }

    public class ConvertTool
    {
        public static string EncryptFileName(string text, int shift = 5)
        {
            return Cipher(text, shift);
        }

        public static string DecryptFileName(string text, int shift = 5)
        {
            return Cipher(text, -shift);
        }

        public static string EncryptContent(string content)
        {
            return SecurityPlayerPrefs.Encrypt(content);
        }

        public static string DecryptContent(string content)
        {
            return SecurityPlayerPrefs.Decrypt(content);
        }

        private static string Cipher(string text, int shift = 5)
        {
            var result = new char[text.Length];

            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];

                if (char.IsLetter(c))
                {
                    var offset = char.IsUpper(c) ? 'A' : 'a'; // Đặt điểm bắt đầu (A hoặc a)
                    result[i] = (char)((c + shift - offset + 26) % 26 +
                                       offset); // Dịch chuyển trong vòng tròn 26 chữ cái
                }
                else
                {
                    result[i] = c; // Giữ nguyên nếu không phải chữ cái
                }
            }

            return new string(result);
        }

        // [MenuItem("Tool/Convert")]
        public static void EncryptEditor(string path, string outputPath, List<string> listExtension)
        {
            var files = Directory.GetFiles(path);
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            foreach (var file in files)
            {
                // just read txt file

                //get extension file
                var extension = Path.GetExtension(file);
                if (!listExtension.Contains(extension))
                {
                    continue;
                }

                Debug.LogWarning("Encrypt file:" + file);
                var fileContent = File.ReadAllText(file);
                var encryted = EncryptContent(fileContent);
                // var test = Convert.FromBase64String(encryted);
                // write to new file output path
                var fileName = Path.GetFileNameWithoutExtension(file);
                var encryptedFileName = EncryptFileName(fileName) + extension;
                var outputFilePath = Path.Combine(outputPath, encryptedFileName);
                // File.WriteAllBytes(outputFilePath, test);

                File.WriteAllText(outputFilePath, encryted);
            }

            AssetDatabase.Refresh();
        }

        // [MenuItem("Tool/decrypt")]
        public static void DecryptEditor(string path, string outputPath, List<string> listExtension)
        {
            var files = Directory.GetFiles(path);
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            foreach (var file in files)
            {
                var extension = Path.GetExtension(file);
                if (!listExtension.Contains(extension))
                {
                    continue;
                }

                var fileContent = File.ReadAllText(file);
                var decrypted = DecryptContent(fileContent);
                // var test = Convert.FromBase64String(decrypted);
                // write to new file output path
                var fileName = Path.GetFileNameWithoutExtension(file);
                var decryptedFileName = DecryptFileName(fileName) + extension;
                var outputFilePath = Path.Combine(outputPath, decryptedFileName);
                //write byte to file

                // File.WriteAllBytes(outputFilePath, test);
                File.WriteAllText(outputFilePath, decrypted);
                Debug.LogWarning("Decrypt file:" + file);
            }

            AssetDatabase.Refresh();
        }
    }
}