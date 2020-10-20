using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace IEC
{
    public class LevelConfigData : MonoBehaviour
    {
        [BoxGroup("Prefab")]
        public GameObject wallObj, gridObj;

        [BoxGroup("Group")]
        public Transform[] wallParents;   // TOP, BOT, LEFT, RIGHT
        public Transform gridParent;

        public int size;

        public void DestroyAll()
        {
            for (int i = 0; i < wallParents.Length; i++)
            {
                var child = wallParents[i];
                var childCount = child.childCount;

                for (int j = 0; j < childCount; j++)
                {
                    DestroyImmediate(child.GetChild(0).gameObject);
                }
            }

            var gridCount = gridParent.childCount;
            for (int i = 0; i < gridCount; i++)
            {
                DestroyImmediate(gridParent.GetChild(0).gameObject);
            }
        }

        public void OnResetAll()
        {
            for (int i = 0; i < wallParents.Length; i++)
            {
                var child = wallParents[i];
                var childCount = child.childCount;

                for (int j = 0; j < childCount; j++)
                {
                    var wall = child.GetChild(j);
                    wall.localRotation = Quaternion.identity;
                    wall.GetComponent<GridItem>().color = 0;
                }
            }

            var gridCount = gridParent.childCount;
            for (int i = 0; i < gridCount; i++)
            {
                var grid = gridParent.GetChild(i);
                var childCount = grid.childCount;
                grid.GetComponent<GridItem>().color = 0;
                for (int j = 0; j < childCount; j++)
                {
                    grid.GetChild(j).localRotation = Quaternion.identity;
                }
            }
        }

        public string GetData()
        {
            string data = "" + (size + 2) + "\n";
            for (int i = 0; i < size + 2; i++)
            {
                var strRow = "1";

                if (i == 0)
                {
                    // Wall top
                    for (int j = 0; j < size; j++)
                    {
                        var wallChild = wallParents[0].GetChild(j);
                        var wallItem = wallChild.GetComponent<GridItem>();

                        strRow += " " + wallItem.GetGridType();
                    }
                    strRow += " 1";
                }
                else if (i == size + 1)
                {
                    // Wall bot
                    for (int j = 0; j < size; j++)
                    {
                        var wallChild = wallParents[1].GetChild(j);
                        var wallItem = wallChild.GetComponent<GridItem>();

                        strRow += " " + wallItem.GetGridType();
                    }
                    strRow += " 1";
                }
                else
                {
                    strRow = "";
                    var wallChildLeft = wallParents[2].GetChild(i - 1);
                    var wallItemLeft = wallChildLeft.GetComponent<GridItem>();
                    strRow += wallItemLeft.GetGridType();

                    for (int j = 0; j < size; j++)
                    {
                        var index = ((i- 1) * size) + j;
                        var grid = gridParent.GetChild(index);
                        var gridItem = grid.GetComponent<GridItem>();

                        var gridValue = "" + GetGridType(gridItem.GetGridType());
                        var intValue = int.Parse(gridValue);
                        var rot = grid.localEulerAngles.y / 90;
                        gridValue += "_" + rot;
                        if (intValue == 5 || intValue == 10)
                        {
                            gridValue += "_" + gridItem.color +"_0";
                        }
                        strRow += " " + gridValue;
                    }

                    var wallChildRight = wallParents[3].GetChild(i - 1);
                    var wallItemRight = wallChildRight.GetComponent<GridItem>();
                    strRow += " " + wallItemRight.GetGridType();
                }

                
                data += strRow + "\n";
            }

            return data;
        }

        private string GetGridType(int type)
        {
            if (type == 1)
            {
                return "5";
            }
            else if (type == 2)
            {
                return "10";
            }
            else if (type == 3)
            {
                return "15";
            }
            else if (type == 4)
            {
                return "16";
            }
            else if (type == 5)
            {
                return "17";
            }
            else return "0";
        }

    }
}

