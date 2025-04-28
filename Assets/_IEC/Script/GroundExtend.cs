using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IEC
{
    [Serializable]
    public class ThemeGroundConfig
    {
        public string themeId;
        public GameObject ground;
    }

    public class GroundExtend : _BaseBehaviour
    {
        [SerializeField] private float posY = -0.125f;
        [SerializeField] private List<ThemeGroundConfig> grounds;
        [SerializeField] private float increaseSize = 8f;
        [SerializeField] private Transform itemHolder;
        public float cubeSize = 1f;
        public float offset = 1f;
        private List<GameObject> wallBorderPool = new List<GameObject>();
        private void Awake()
        {
            Pool.Set(this);
            foreach (var themeGroundConfig in grounds)
            {
                themeGroundConfig.ground.SetActive(false);
            }

            this.RegisterListener(EventList.GamePlay.ON_LEVEL_LOADED, ResizeLevelGround);
        }

        private void OnDestroy()
        {
            this.UnregisterListener(EventList.GamePlay.ON_LEVEL_LOADED, ResizeLevelGround);
        }

        public void CheatSetGroundForItemDebug()
        {
            foreach (var themeGroundConfig in grounds)
            {
                themeGroundConfig.ground.SetActive(themeGroundConfig.themeId == "Grass");
            }

            var objectToInclude = itemHolder.GetComponentsInChildren<BaseItem>().ToList();
            UpdateSizeForCurrentPlatForm("Grass", objectToInclude);
        }


        public void ResizeLevelGround(object param)
        {
            DestroyAllItemBorder();
            if (!Game.isUsingGroundExtend)
            {
                foreach (var themeGroundConfig in grounds)
                {
                    themeGroundConfig.ground.SetActive(false);
                }

                return;
            }

            var levelData = (EventLevelLoad) param;
            var objectsToInclude = levelData.items;
            if (objectsToInclude == null || objectsToInclude.Count == 0)
            {
                Debug.LogWarning("No objects provided to calculate bounds!");
                return;
            }

            UpdateSizeForCurrentPlatForm(levelData.themeId, objectsToInclude);
        }

        private void UpdateSizeForCurrentPlatForm(string themeId, List<BaseItem> objectsToInclude)
        {
            GameObject platform = null;
            foreach (var themeGroundConfig in grounds)
            {
                if (themeGroundConfig.themeId == themeId)
                {
                    platform = GetGroundObject(themeGroundConfig);
                }
                else
                {
                    themeGroundConfig.ground.SetActive(false);
                }
            }

            if (platform == null)
            {
                Debug.LogError($"No platform assigned! {themeId} => Fallback to default");
                platform = GetGroundObject(grounds[0]);
            }

            if (Game.usingWaterTheme)
            {
                platform.transform.GetChild(0).gameObject.SetActive(false);
                platform.transform.GetChild(1).gameObject.SetActive(false);
                platform.transform.GetChild(2).gameObject.SetActive(false);
            }

            Bounds combinedBounds = CalculateBounds(objectsToInclude);

            Vector3 boundsSize = combinedBounds.size;
            Vector3 boundsCenter = combinedBounds.center;

            Vector3 extensionAmount = new Vector3(1, 0, 1) * increaseSize;
            Vector3 extendedSize = boundsSize + extensionAmount;
            boundsCenter.y = posY;
            platform.transform.position = boundsCenter;

            var platformExtend = platform.transform.Find("PlatformExtend").gameObject; //dont do this
            if (platformExtend != null)
            {
                //MeoMew block
                if (!Application.isPlaying)
                {
                    platformExtend.transform.parent = null;
                }

                platform.transform.localScale = new Vector3(
                    extendedSize.x,
                    platform.transform.localScale.y, // Keep original height
                    extendedSize.z
                );

                var tempExtendScale = new Vector3(1, platformExtend.transform.localScale.y, 1);
                tempExtendScale.x = platform.transform.localScale.x - 0.15f;
                tempExtendScale.z = platform.transform.localScale.z - 0.15f;
                platformExtend.transform.localScale = tempExtendScale;
                platformExtend.transform.parent = platform.transform;

                var vector3 = platformExtend.transform.localPosition;
                vector3.x = 0;
                vector3.z = 0;
                platformExtend.transform.localPosition = vector3;
            }
            // Can change to spawn area rectangle 
            SpawnCubesAroundBorder(boundsCenter, extendedSize);
        }



        public static Bounds CalculateBounds(List<BaseItem> objects)
        {
            if (objects.Count == 0)
                return new Bounds(Vector3.one, Vector3.one);

            Bounds combinedBounds = GetRendererBounds(objects[0].gameObject);

            for (int i = 1; i < objects.Count; i++)
            {
                Bounds objectBounds = GetRendererBounds(objects[i].gameObject);
                combinedBounds.Encapsulate(objectBounds);
            }

            return combinedBounds;
        }
        
        public static Bounds CalculateBounds(List<Collider> objects)
        {
            if (objects.Count == 0)
                return new Bounds(Vector3.one, Vector3.one);

            Bounds combinedBounds = GetRendererBounds(objects[0].gameObject);

            for (int i = 1; i < objects.Count; i++)
            {
                Bounds objectBounds = GetRendererBounds(objects[i].gameObject);
                combinedBounds.Encapsulate(objectBounds);
            }

            return combinedBounds;
        }

        public static Bounds GetRendererBounds(GameObject obj)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0)
            {
                Debug.LogWarning($"No renderer found on object: {obj.name}");
                return new Bounds(obj.transform.position, Vector3.zero);
            }

            Bounds combinedBounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                combinedBounds.Encapsulate(renderers[i].bounds);
            }

            return combinedBounds;
        }

        private GameObject GetGroundObject(ThemeGroundConfig themeGroundConfig)
        {
            var platform = themeGroundConfig.ground;
            platform.SetActive(true);
            return platform;
        }
        #region Bouy Wall Border
        void SpawnCubesAroundBorder(Vector3 center, Vector3 size)
        {
            // Calculate the number of cubes to place on each side (fit along the border)
            int cubesAlongWidth = Mathf.CeilToInt((size.x + offset) / (cubeSize + offset)); // Consider offset
            int cubesAlongDepth = Mathf.CeilToInt((size.z + offset) / (cubeSize + offset)); // Consider offset
            // Front and Back Faces (along X and Z axis)
            for (int i = 0; i < cubesAlongWidth; i++)
            {
                for (int j = 0; j < cubesAlongDepth; j++)
                {
                    if (i == 0 || j == 0 || i == cubesAlongWidth || j == cubesAlongDepth)
                    {
                        Vector3 frontPosition = center + new Vector3(i * (cubeSize + offset) - size.x / 2f, 0, -size.z / 2f);
                        InstantiateCube(frontPosition);
                        Vector3 backPosition = center + new Vector3(i * (cubeSize + offset) - size.x / 2f, 0, size.z / 2f);
                        InstantiateCube(backPosition);
                    }
                }
            }

            // Left and Right Faces (along Z axis)
            for (int i = 0; i < cubesAlongDepth; i++)
            {
                for (int j = 0; j < cubesAlongWidth; j++)
                {
                    if (i == 0 || j == 0 || i == cubesAlongDepth || j == cubesAlongWidth)
                    {
                        Vector3 leftPosition = center + new Vector3(-size.x / 2f, 0, i * (cubeSize + offset) - size.z / 2f);
                        InstantiateCube(leftPosition);

                        Vector3 rightPosition = center + new Vector3(size.x / 2f, 0, i * (cubeSize + offset) - size.z / 2f);
                        InstantiateCube(rightPosition);
                    }
                }
            }
        }

        void InstantiateCube(Vector3 position)
        {
            // Instantiate a cube at the given position
            GameObject cube = Instantiate(Config.prefabConfig.wallBouyOcean, this.transform, true);
            cube.transform.position = position;
            cube.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
            wallBorderPool.Add(cube);
        }

        public void DestroyAllItemBorder()
        {
            for (int i = wallBorderPool.Count-1; i >=0; i--)
            {
                Destroy( wallBorderPool[i]);
            }
            wallBorderPool.Clear();
        }
        #endregion
    }
}