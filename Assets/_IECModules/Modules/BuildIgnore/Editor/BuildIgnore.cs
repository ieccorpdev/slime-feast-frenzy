using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace IEC.BuildIgnore
{
    public class BuildIgnore
    {
        public List<string> folder = new List<string>();
        public List<string> files = new List<string>();

        List<string> movingFiles = new List<string>();

        public void FindFiles()
        {
            folder.Clear();
            files.Clear();
            List<string> assets = new List<string>();
            assets.AddRange(AssetDatabase.FindAssets("t:Folder", new[] { "Assets/_IEC" }).ToList<string>());

            foreach (var guid in assets)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = System.IO.Path.GetFileName(path);
                if (fileName.ToLower().Equals("demo"))
                {
                    folder.Add(path);
                }
            }
            string[] lDemo = AssetDatabase.FindAssets("l:Demo", new[] { "Assets/_IEC" });
            foreach (var guid in lDemo)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                files.Add(path);
            }

        }

        public void HideFiles()
        {
            movingFiles.Clear();
            movingFiles.AddRange(folder);
            foreach (string item in files)
            {
                movingFiles.Add(item);
                movingFiles.Add(item + ".meta");
            }
            Debug.Log("Hide " + movingFiles.Count);
            for (int i = 0; i < movingFiles.Count; i++)
            {
                if (File.Exists(movingFiles[i] + "~"))
                {
                    Debug.LogError("File exists " + movingFiles[i]);
                    File.Delete(movingFiles[i] + "~");
                }
                if (Directory.Exists(movingFiles[i] + "~"))
                {
                    Directory.Delete(movingFiles[i] + "~");
                }
                File.Move(movingFiles[i], movingFiles[i] + "~");
            }
            AssetDatabase.Refresh();
        }


        public void UnhideFiles()
        {
            movingFiles.Clear();
            movingFiles.AddRange(folder);
            foreach (string item in files)
            {
                movingFiles.Add(item);
                movingFiles.Add(item + ".meta");
            }
            Debug.Log("Unhide " + movingFiles.Count);
            for (int i = 0; i < movingFiles.Count; i++)
            {   //No more META files to consider, so we can use File.Move()
                if (File.Exists(movingFiles[i]))
                {
                    Debug.LogError("File exists " + folder[i]);
                    File.Delete(folder[i]);
                }
                if (Directory.Exists(movingFiles[i]))
                {
                    Directory.Delete(movingFiles[i]);
                }
                File.Move(movingFiles[i] + "~", movingFiles[i]);
            }
            AssetDatabase.Refresh();
        }
    }

}