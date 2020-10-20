using System.Collections;
using System.Collections.Generic;
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
            if (GUILayout.Button("Spawn Level"))
            {
                var obj = Selection.activeGameObject;
                if (obj)
                {
                    var levelConfig = obj.GetComponent<LevelConfigData>();
                    var data = levelConfig.GetData();
                    Debug.LogError("Data: \n" + data);
                }
            }


            EditorUtility.SetDirty(target);
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
                SpawnWall(wallParents[i], levelConfig.wallObj, size, startPos);
            }
        }

        private void SpawnWall(Transform parent, GameObject wallObj, int count, float startPos)
        {
            for (int i = 0; i < count; i++)
            {
                var posSpawn = new Vector3(startPos + i * 1, 0f, 0f);

                var wallTrans = Instantiate(wallObj).transform;
                wallTrans.SetParent(parent);
                wallTrans.localPosition = posSpawn;
                wallTrans.localRotation = Quaternion.identity;
                wallTrans.localScale = Vector3.one;
            }
        }

        private void SpawnGrids(LevelConfigData levelConfig)
        {
            var size = levelConfig.size;
            var startPos = (size - 1) / -2f;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    var posSpawn = new Vector3(startPos + j, 0f, startPos + i);
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
