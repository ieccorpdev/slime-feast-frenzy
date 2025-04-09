using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Gameplay;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace IEC
{
    public class ItemSpawnUtilities : SerializedMonoBehaviour
    {
#if UNITY_EDITOR
        [Header("Spawn Areas")]
        [ReadOnly] public SpawnArea spawnArea;
        
        [Header("Config")]
        [HideInInspector] public string itemId;
        [HideInInspector] public int width = 20;
        [HideInInspector] public int length = 20;
        [HideInInspector] public float spacing = 1.5f;

        [Space] [Header("Reference")] [SerializeField]
        private Transform levelHolder;
        [SerializeField] private GameplayItemPrefabConfig itemPrefabConfig;

        private readonly string _folderPathItemInGame = "_IECProject/Items";
        private GameObject _previewGameObject;

        // [Button]
        public void CreatePreview()
        {
            if (spawnArea == null) return;
            if (_previewGameObject)
            {
                DestroyImmediate(_previewGameObject);
                _previewGameObject = null;
            }

            _previewGameObject = spawnArea.CreatePreview(itemPrefabConfig, _folderPathItemInGame, itemId);
        }

        public void Apply()
        {
            if(_previewGameObject == null) return;
            if (EditorUtility.DisplayDialog("Apply?", "", "Yes", "No"))
            {
                _previewGameObject.transform.parent = levelHolder.transform;
                _previewGameObject = null;
            }
        }

        // [Button]
        // [ShowIf("@_previewGameObject != null")]
        public void Clear()
        {
            if(_previewGameObject == null) return;
            if (EditorUtility.DisplayDialog("Cancel?", "", "Yes", "No"))
            {
                DestroyImmediate(_previewGameObject);
                _previewGameObject = null;
            }
        }
        
        // [Button]
        void ClearExistingMaps()
        {
            if (EditorUtility.DisplayDialog("Remove all items on map?", "Remove all items on map", "Yes", "No"))
            {
                while (levelHolder.childCount > 0)
                {
                    DestroyImmediate(levelHolder.GetChild(0).gameObject);
                }
            }
        }
#endif
    }
}
