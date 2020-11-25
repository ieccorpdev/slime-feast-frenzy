using System;
using UnityEngine;

namespace IEC
{
    [Serializable]
    public class LevelConfig
    {
        public int size;
        public int[] topWall, botWall, leftWall, rightWall;
        public string[,] gridTypes;
        public int[,] gridHints;

        public void SetSize(int size, string[,] gridTypes, string[,] gridHints)
        {
            this.size = size;

            topWall = new int[size];
            botWall = new int[size];
            leftWall = new int[size];
            rightWall = new int[size];

            this.gridTypes = gridTypes;
            this.gridHints = new int[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    var gridData = gridTypes[i, j].Trim();
                    var tempValue = int.Parse(gridData);

                    var gridHint = gridHints[i, j].Trim();

                    if (i != 0 && i != size - 1 && j != 0 && j != size - 1)
                    {
                        var rot = gridHint.Substring(2, 1);
                        this.gridHints[i, j] = int.Parse(rot);
                    }

                    if (i == 0)
                    {
                        topWall[j] = tempValue;
                    }
                    if (i == size - 1)
                    {
                        botWall[j] = tempValue;
                    }

                    if (j == 0)
                    {
                        leftWall[i] = tempValue;
                    }

                    if (j == size - 1)
                    {
                        rightWall[i] = tempValue;
                    }
                }
            }
        }
    }

    public static class LevelHelper
    {
        public static string[,] GetGridValue(string[] strGrids, int size, bool isLoop)
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

        public static bool IsLoopLevel(string mode)
        {
            return (mode.IndexOf("Loop") > -1);
        }

        public static bool IsMultiColor(string mode)
        {
            return (mode.IndexOf("colors") > -1);
        }

        public static bool IsSourceGird(int gridType)
        {
            return (gridType == 51 || gridType == 11 || gridType == 21 || gridType == 31);
        }

        public static bool IsTargetGrid(int gridType)
        {
            return (gridType == 52 || gridType == 12 || gridType == 22 || gridType == 32);
        }

        public static int GetSourceStyle(int input)
        {
            if (input == 11)
            {
                return 1;
            }
            else if (input == 21)
            {
                return 2;
            }
            else if (input == 31)
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }

        public static int GetTargetStyle(int input)
        {
            if (input == 12)
            {
                return 1;
            }
            else if (input == 22)
            {
                return 2;
            }
            else if (input == 32)
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }

        public static GridType GetType(int gridType)
        {
            GridType type = GridType.NONE;
            switch (gridType)
            {
                case 11:
                case 21:
                case 31:
                case 51:
                    type = GridType.GRID_SOURCE;
                    break;
                case 52:
                case 22:
                case 32:
                case 12:
                    type = GridType.GRID_TARGET;
                    break;
                case 10:
                    type = GridType.GRID_STRAIGHT;
                    break;
                case 30:
                    type = GridType.GRID_CROSS;
                    break;
                case 20:
                    type = GridType.GRID_CURVE;
                    break;
            }

            return type;
        }

        public static GameObject GetGridObj(ComponentConfig ComponentConfig, int gridType)
        {
            GameObject obj = null;
            switch (gridType)
            {
                case 11:
                case 21:
                case 31:
                case 51:
                    obj = ComponentConfig.sourceObj;
                    break;
                case 52:
                case 12:
                case 22:
                case 32:
                    obj = ComponentConfig.targetObj;
                    break;
                case 10:
                    obj = ComponentConfig.straightObj;
                    break;
                case 30:
                    obj = ComponentConfig.crossObj;
                    break;
                case 20:
                    obj = ComponentConfig.curveObj;
                    break;
                default:
                    Debug.LogError("gridType: " + gridType);
                    obj = ComponentConfig.straightObj;
                    break;
            }

            return obj;
        }

        public static int CalculatorRot(int rotCurrent, int reduce)
        {
            return (rotCurrent - reduce + 4) % 4;
        }

        public static int GetLevelSpawn(int matrix, int seed)
        {
            if(matrix == 3)
            {
                return RandomHelper.RangeWithSeed(seed, 2, 14);
            }
            else if (matrix == 4)
            {
                return RandomHelper.RangeWithSeed(seed, 14, 25);
            }
            else if (matrix == 5)
            {
                return RandomHelper.RangeWithSeed(seed, 25, 40);
            }
            else if (matrix == 6)
            {
                return RandomHelper.RangeWithSeed(seed, 40, 69);
            }
            else if (matrix == 7)
            {
                return RandomHelper.RangeWithSeed(seed, 69, 98);
            }
            else if (matrix == 7)
            {
                return RandomHelper.RangeWithSeed(seed, 98, 116);
            }
            else
            {
                return RandomHelper.RangeWithSeed(seed, 116, 1135);
            }
        }
    }
}

