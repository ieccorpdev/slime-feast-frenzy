using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace IEC
{
    [CustomEditor(typeof(LevelConfigData))]
    public class LevelEditor : Editor
    {
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUILayout.Space(30);

            if (GUILayout.Button("Spawn Board"))
            {
                var obj = Selection.activeGameObject;
                if (obj)
                {
                    var levelConfig = obj.GetComponent<LevelConfigData>();
                    levelConfig.DestroyAll();
                    SpawnWalls(levelConfig);
                    SpawnGrids(levelConfig);
                }
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Reset All"))
            {
                var obj = Selection.activeGameObject;
                if (obj)
                {
                    var levelConfig = obj.GetComponent<LevelConfigData>();
                    levelConfig.OnResetAll(true);
                }
            }

            GUILayout.Space(30);
            if (GUILayout.Button("Check"))
            {
                var obj = Selection.activeGameObject;
                if (obj)
                {
                    var levelConfig = obj.GetComponent<LevelConfigData>();
                    levelConfig.OnResetAll(false);
                }
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Spawn Level"))
            {
                var obj = Selection.activeGameObject;
                if (obj)
                {
                    var levelConfig = obj.GetComponent<LevelConfigData>();
                    var data = levelConfig.GetData();
                    var count = TotalLevel();
                    count += 1;
                    string name = "Level_" + count;
                    string path = "Assets/_IEC/Config/Levels/" + name + ".txt";

                    //Write some text to the test.txt file
                    StreamWriter writer = new StreamWriter(path, true);
                    writer.WriteLine(data);
                    writer.Close();
                    AssetDatabase.Refresh();
                }
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Check File"))
            {
                var obj = Selection.activeGameObject;
                if (obj)
                {
                    ClearLog();

                    LevelConfig levelConfig = new LevelConfig();

                    var strLevels = Resources.Load<TextAsset>("levelsList");

                    var file = strLevels.text;
                    var levelDatas = file.Split(new string[] { "\n", "\r", "\r\n" },
                                     System.StringSplitOptions.RemoveEmptyEntries);

                    var leveText = levelDatas[1].Trim().Split(':');

                    var size = int.Parse(leveText[0].Substring(leveText[0].Length - 1)) + 2;
                    var strMode = leveText[1];
                    var strGrid = leveText[3];

                    var gridsData = strGrid.Trim().Split('_');

                    string[,] grids = GetGridValue(gridsData, size, IsLoopLevel(strMode));

                    var str = "";
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            str += grids[i, j] + " ";
                        }

                        str += "\n";
                    }

                    Debug.LogError(str);
                }
            }

            EditorUtility.SetDirty(target);
        }

        private bool IsLoopLevel(string mode)
        {
            return (mode.IndexOf("Loop") > -1);
        }

        private string[,] GetGridValue(string[] strGrids, int size, bool isLoop)
        {
            var grids = new string[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i == 0 || j == 0 || i == size - 1 || j == size - 1)
                    {
                        if (grids[i, j] == null)
                        {
                            grids[i, j] = "1";
                        }
                    }
                    else
                    {
                        var index = (i - 1) * (size - 2) + (j - 1);
                        var gridData = strGrids[index];
                        var gridType = "";

                        if (isLoop)
                        {
                            var startIndex = gridData.IndexOf('(');
                            var endIndex = gridData.IndexOf(')');
                            gridType = gridData.Substring(0, startIndex);

                            var edgeData = gridData.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();
                            var edges = edgeData.Split(',');

                            if (i == 1)
                            {
                                if (edges[0] != "-1")
                                {
                                    grids[0, j] = "2";
                                }
                            }

                            if (j == 1)
                            {
                                if (edges[1] != "-1")
                                {
                                    grids[i, 0] = "2";
                                }
                            }

                            if (i == size - 2)
                            {
                                if (edges[2] != "-1")
                                {
                                    grids[size - 1, j] = "2";
                                }
                            }

                            if (j == size - 2)
                            {
                                if (edges[3] != "-1")
                                {
                                    grids[i, size - 1] = "2";
                                }
                            }
                        }
                        else
                        {
                            gridType = gridData;
                        }

                        grids[i, j] = gridType;
                    }
                }
            }

            return grids;
        }

        public void ClearLog() //you can copy/paste this code to the bottom of your script
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }

        private int TotalLevel()
        {
            var count = Directory.GetFiles(
                Application.dataPath + "/_IEC/Config/Levels/",
                "*.txt", SearchOption.AllDirectories);
            return count.Length;
        }


        private void SpawnWalls(LevelConfigData levelConfig)
        {
            var size = levelConfig.size;
            var wallParents = levelConfig.wallParents;

            var startPos = (size - 1) / -2f;
            var wallPos = (size / 2f) + .18f;

            wallParents[0].localPosition = new Vector3(0f, 0f, wallPos);
            wallParents[1].localPosition = new Vector3(0f, 0f, -wallPos);

            wallParents[2].localPosition = new Vector3(-wallPos, 0f, 0f);
            wallParents[3].localPosition = new Vector3(wallPos, 0f, 0f);

            for (int i = 0; i < 4; i++)
            {
                SpawnWall(wallParents[i], levelConfig.wallObj, size, startPos, (i == 0 || i == 3));
            }
        }

        private void SpawnWall(Transform parent, GameObject wallObj, int count, float startPos, bool isReverse)
        {
            for (int i = 0; i < count; i++)
            {
                var posSpawn = new Vector3(startPos + i * 1, 0f, 0f);

                var wallTrans = Instantiate(wallObj).transform;
                wallTrans.SetParent(parent);
                wallTrans.localPosition = posSpawn;
                wallTrans.localRotation = Quaternion.identity;
                wallTrans.localScale = Vector3.one;

                if (isReverse)
                {
                    wallTrans.GetChild(1).localRotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }
        }

        private void SpawnGrids(LevelConfigData levelConfig)
        {
            var size = levelConfig.size;
            var startPosX = (size - 1) / -2f;
            var startPosZ = (size - 1) / 2f;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    var posSpawn = new Vector3(startPosX + j, 0f, startPosZ - i);
                    var name = "Grid_" + ((i * size) + j);

                    SpawnGrid(levelConfig.gridParent, levelConfig.gridObj, posSpawn, name);
                }
            }
        }

        private void SpawnGrid(Transform parent, GameObject gridObj, Vector3 posSpawn, string name)
        {
            var grid = Instantiate(gridObj).transform;
            grid.SetParent(parent);
            grid.localPosition = posSpawn;
            grid.localRotation = Quaternion.identity;
            grid.localScale = Vector3.one;

            grid.gameObject.name = name;
        }
    }
}
