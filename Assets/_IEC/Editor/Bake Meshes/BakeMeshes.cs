using System.IO;
using UnityEditor;
using UnityEngine;

namespace _IEC.Editor.Bake_Meshes
{
    public class BakeMeshes : EditorWindow
    {
        private bool readWriteEnabled = true;

        [MenuItem("Tools/Model Read/Write Settings")]
        public static void ShowWindow()
        {
            GetWindow<BakeMeshes>("Model Read/Write Settings");
        }

        void OnGUI()
        {
            GUILayout.Label("Set Read/Write Enabled for Selected Models", EditorStyles.boldLabel);
            
            readWriteEnabled = EditorGUILayout.Toggle("Read/Write Enabled", readWriteEnabled);
            // Nút để áp dụng thay đổi
            if (GUILayout.Button("Apply to Selected Models"))
            {
                ModifyFbxMetaFiles(Application.dataPath,true);
            }
        }

        private void ModifyFbxMetaFiles(string folderPath, bool isReadable)
        {
            // Tìm tất cả các file .fbx.meta trong thư mục được chỉ định
            string[] metaFiles = Directory.GetFiles(folderPath, "*.fbx.meta", SearchOption.AllDirectories);

            foreach (string metaFile in metaFiles)
            {
                string[] lines = File.ReadAllLines(metaFile);
                bool modified = false;

                // Tìm và thay đổi giá trị của isReadable
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("  isReadable:"))
                    {
                        // Chỉnh sửa dòng chứa isReadable
                        //lines[i] = "  isReadable: " + (isReadable ? "1" : "0");
                        lines[i] = lines[i].Replace("isReadable: 0","isReadable: 1");
                        modified = true;
                        break;
                    }
                }

                // Nếu đã thay đổi, ghi lại nội dung mới vào file meta
                if (modified)
                {
                    File.WriteAllLines(metaFile, lines);
                    Debug.Log("Modified: " + metaFile);
                }
                else
                {
                    Debug.LogWarning("No isReadable property found in: " + metaFile);
                }
            }

            // Refresh lại AssetDatabase để cập nhật thay đổi trong Editor
            AssetDatabase.Refresh();
        }
    }
}
