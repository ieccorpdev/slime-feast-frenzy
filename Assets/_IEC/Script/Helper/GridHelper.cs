using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public static class GridHelper
    {
        public static Material GetTerrainType(int row, int col, List<Material> listTerrainType)
        {
            if (row % 2 == 0)
            {
                if (col % 2 == 0)
                {
                    return listTerrainType[0];
                }
                else
                {
                    return listTerrainType[1];
                }
            }
            else
            {
                if (col % 2 == 0)
                {
                    return listTerrainType[1];
                }
                else
                {
                    return listTerrainType[0];
                }
            }
        }

        public static BaseComponent GetOppositeWall(int Row, int Col, int color, Game Game)
        {
            var maxIndex = Game.size - 1;
            var row = Row;
            var col = Col;

            if (Row == 0)
            {
                row = maxIndex;
            }

            if (Row == maxIndex)
            {
                row = 0;
            }

            if (Col == 0)
            {
                col = maxIndex;
            }

            if (Col == maxIndex)
            {
                col = 0;
            }

            return Game.GetGrid(row, col, color);
        }

        public static int GetColor(List<int> listWaterColor)
        {
            var resultColor = listWaterColor[0];
            if (listWaterColor.Count == 2)
            {
                resultColor = GetMix2Colors(listWaterColor);
            }
            else if (listWaterColor.Count == 3)
            {
                if (listWaterColor.Contains(1) && listWaterColor.Contains(2) && listWaterColor.Contains(3))
                {
                    resultColor = 7;
                }
                else
                {
                    resultColor = GetMix2Colors(listWaterColor);
                }
            }

            return resultColor;
        }

        private static int GetMix2Colors(List<int> listWaterColor)
        {
            var resultColor = 0;

            if (listWaterColor.Contains(1) && listWaterColor.Contains(2))
            {
                resultColor = 4;
            }
            else if (listWaterColor.Contains(2) && listWaterColor.Contains(3))
            {
                resultColor = 5;
            }
            else if (listWaterColor.Contains(1) && listWaterColor.Contains(3))
            {
                resultColor = 6;
            }
            return resultColor;
        }


    }
}
