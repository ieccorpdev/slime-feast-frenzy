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
        private int[] matrix3 = new int[] { 0, 1, 2, 3, 5, 6, 8, 9, 10, 12 };
        private int[] matrix4 = new int[] { 13, 16, 17, 20, 21, 23 };

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUILayout.Space(30);

            GUILayout.Space(10);
            if (GUILayout.Button("Get Data"))
            {
                var levels = ((DataTextConfig)AssetDatabase.LoadAssetAtPath("Assets/_IEC/Config/Level Config/ListLevelRemoveMixColorConfig.asset", typeof(DataTextConfig))).listText;
                Debug.LogError("Count: " + levels.Count);


                //var levelSortText = strLevelSort1.text;
                //var levelDifficuty = levelSortText.Split(new string[] { "\n", "\r", "\r\n" },
                //                 System.StringSplitOptions.RemoveEmptyEntries);

                //var str = "";
                //var count = levelDifficuty.Length;
                //for (int i = 0; i < count; i++)
                //{
                //    var leveText = levelDifficuty[i].Trim();
                //    var levelData = leveText.Split(':');
                //    var strMode = levelData[1];
                //    if (strMode.IndexOf("Mix") < 0) continue;

                //    var strGrid = levelData[3];
                //    var gridsDatas = strGrid.Trim().Split('_');
                //    if (IsMixColorLevel(gridsDatas))
                //    {
                //        str += leveText + "\n";
                //    }
                //}

                //string path2 = "Assets/_IEC/Resources/ListSolutionMixColors.txt";
                ////Write some text to the test.txt file
                //StreamWriter writer = new StreamWriter(path2, true);
                //writer.WriteLine(str);
                //writer.Close();
                //AssetDatabase.Refresh();
            }




            //if (GUILayout.Button("Merge level"))
            //{
            //    ClearLog();

            //    List<int> listLevelIndex = new List<int>();

            //    int seed = 0;

            //    for (int i = 0; i < 4; i++)
            //    {
            //        var index = MathHelper.GetRandomIndexByList(listLevelIndex, matrix3.Length, seed);
            //        listLevelIndex.Add(index);
            //        seed += 1;
            //    }

            //    string strName = "ListLevelRemoveMixColor";

            //    var strLevelData = Resources.Load<TextAsset>(strName);
            //    var levelDatas = strLevelData.text.Split(new string[] { "\n", "\r", "\r\n" },
            //                     System.StringSplitOptions.RemoveEmptyEntries);

            //    List<string[,]> listLevel = new List<string[,]>();

            //    for (int i = 0; i < 4; i++)
            //    {
            //        var levelIndex = listLevelIndex[i];

            //        var leveText = levelDatas[levelIndex].Trim();
            //        var levelData = leveText.Split(':');
            //        var gridDatas = levelData[3].Trim().Split('_');

            //        var grids = LevelHelper.SplitLevel(gridDatas, 3);
            //        listLevel.Add(grids);
            //    }

            //    var gridsNewLevel = LevelHelper.MergeLevel(listLevel, 6);

            //    //var newLevel = LevelHelper.GetNewlevel(6, gridsNewLevel);
            //    //Debug.LogError("newLevel: " + newLevel);

            //    //var str = "";
            //    //for (int i = 0; i < 6; i++)
            //    //{
            //    //    for (int j = 0; j < 6; j++)
            //    //    {
            //    //        str += gridsNewLevel[i, j] + " ";
            //    //    }
            //    //    str += "\n";
            //    //}

            //    //Debug.LogError(str);


            //    //DataTextConfig dataTextConfig = CreateInstance<DataTextConfig>();

            //    //int min = 13;
            //    //int max = 24;

            //    //for (int i = min; i < max; i++)
            //    //{
            //    //    var leveText = levelDatas[i].Trim();
            //    //    var levelData = leveText.Split(':');
            //    //    var strMode = levelData[1];

            //    //    if (!LevelHelper.IsLoopLevel(strMode))
            //    //    {
            //    //        dataTextConfig.listText.Add(i.ToString());
            //    //    }
            //    //}

            //    //var count = dataTextConfig.listText.Count;
            //    //dataTextConfig.totalItem = count;

            //    //AssetDatabase.CreateAsset(dataTextConfig, "Assets/_IEC/Config/Level Config/Matrixs/" + fileName + ".asset");
            //    //SetDirty(dataTextConfig);
            //}

            #region Old
            //GUILayout.Space(10);
            //if (GUILayout.Button("Get Data"))
            //{
            //    string strName = "ListLevelRemoveMixColor";
            //    string fileName = "LevelMatrix4";

            //    var strLevelData = Resources.Load<TextAsset>(strName);
            //    var levelDatas = strLevelData.text.Split(new string[] { "\n", "\r", "\r\n" },
            //                     System.StringSplitOptions.RemoveEmptyEntries);

            //    DataTextConfig dataTextConfig = CreateInstance<DataTextConfig>();

            //    int min = 13;
            //    int max = 24;

            //    for (int i = min; i < max; i++)
            //    {
            //        var leveText = levelDatas[i].Trim();
            //        var levelData = leveText.Split(':');
            //        var strMode = levelData[1];

            //        if (!LevelHelper.IsLoopLevel(strMode))
            //        {
            //            dataTextConfig.listText.Add(i.ToString());
            //        }
            //    }

            //    var count = dataTextConfig.listText.Count;
            //    dataTextConfig.totalItem = count;

            //    AssetDatabase.CreateAsset(dataTextConfig, "Assets/_IEC/Config/Level Config/Matrixs/" + fileName + ".asset");
            //    SetDirty(dataTextConfig);
            //}


            //GUILayout.Space(10);
            //if (GUILayout.Button("Sort File"))
            //{
            //    var strLevelData = Resources.Load<TextAsset>("DataDifficutyV4");
            //    var levelSortText = strLevelData.text;
            //    var levelDifficuty = levelSortText.Split(new string[] { "\n", "\r", "\r\n" },
            //                     System.StringSplitOptions.RemoveEmptyEntries);

            //    var str = "";
            //    List<int> listLevel = new List<int>();

            //    var count = levelDifficuty.Length;
            //    for (int i = 0; i < count; i++)
            //    {
            //        bool isHaveResult = false;
            //        var countRequest = 0;
            //        var seed = i;

            //        var size = int.Parse(levelDifficuty[i]);
            //        int level = 0;

            //        while (!isHaveResult && countRequest <= 50)
            //        {
            //            level = LevelHelper.GetLevelSpawn(size, seed);
            //            seed += 1;
            //            countRequest += 1;

            //            if (!listLevel.Contains(level))
            //            {
            //                listLevel.Add(level);
            //                isHaveResult = true;

            //                str += level + "\n";
            //            }
            //            else
            //            {
            //                if(countRequest == 50)
            //                {
            //                    level = -1;
            //                    str += level + "\n";
            //                }
            //            }
            //        }
            //    }



            //    string path2 = "Assets/_IEC/Resources/levelDifficutyV4.txt";
            //    //Write some text to the test.txt file
            //    StreamWriter writer = new StreamWriter(path2, true);
            //    writer.WriteLine(str);
            //    writer.Close();
            //    AssetDatabase.Refresh();
            //}

            //GUILayout.Space(10);
            //if (GUILayout.Button("Get Levels Mix Color"))
            //{
            //    var strLevelSort1 = Resources.Load<TextAsset>("ListSolutionFull");
            //    var levelSortText = strLevelSort1.text;
            //    var levelDifficuty = levelSortText.Split(new string[] { "\n", "\r", "\r\n" },
            //                     System.StringSplitOptions.RemoveEmptyEntries);

            //    var str = "";
            //    var count = levelDifficuty.Length;
            //    for (int i = 0; i < count; i++)
            //    {
            //        var leveText = levelDifficuty[i].Trim();
            //        var levelData = leveText.Split(':');
            //        var strMode = levelData[1];
            //        if (strMode.IndexOf("Mix") < 0) continue;

            //        var strGrid = levelData[3];
            //        var gridsDatas = strGrid.Trim().Split('_');
            //        if (IsMixColorLevel(gridsDatas))
            //        {
            //            str += leveText + "\n";
            //        }
            //    }

            //    string path2 = "Assets/_IEC/Resources/ListSolutionMixColors.txt";
            //    //Write some text to the test.txt file
            //    StreamWriter writer = new StreamWriter(path2, true);
            //    writer.WriteLine(str);
            //    writer.Close();
            //    AssetDatabase.Refresh();
            //}

            #endregion

            EditorUtility.SetDirty(target);
        }

        private static void SetDirty(ScriptableObject scriptableObject)
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(scriptableObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }

        //658

        private bool IsMixColorLevel(string[] gridDatas)
        {
            var count = gridDatas.Length;
            for (int i = 0; i < count; i++)
            {
                var grid = gridDatas[i];
                var color = int.Parse(grid.Substring(3, 1));
                if(color > 3)
                {
                    return true;
                }
            }

            return false;
        }

        private string[,] GetGridValue(string[] strGrids, int size, bool isLoop)
        {
            var grids = new string[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if(i == 0 || j == 0 || i == size - 1 || j == size - 1)
                    {
                        if(grids[i, j] == null)
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
