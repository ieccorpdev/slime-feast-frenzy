using System;
using System.Collections.Generic;
using System.Linq;
using IEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    [CreateAssetMenu(menuName = "Gameplay/ThemeConfig")]
    public class ThemeConfig : ScriptableObject
    {
        public List<GroundThemeConfigModel> groundThemeConfigModels;

        public List<GroundThemeModel> groundThemeModels;

        public List<WaterThemeConfig> waterThemeConfigs;
        public int stepChangeTheme = 25;
        [SerializeField] private string themeNeedToFind;

        public GroundElement GetLevelMapTile(string id, string themeId)
        {
            if (id == "H2O") return null;
            var themeModel = groundThemeConfigModels.Find(x => x.themeId == themeId);
            if (themeModel == null)
            {
                //Debug.LogError("Cant find theme: " + themeId);
                themeModel = groundThemeConfigModels[0];
            }

            var prefab = themeModel.GetElementById(id);
            if (prefab == null)
            {
                //Debug.LogError("id: " + id + " not found! at theme: " + themeId);
                return null;
            }

            return prefab;
        }

        [Button]
        public void FindLevelTheme()
        {
            var config = Pool.Get<GameConfig>();
            var levelIds = new List<int>();

            for (int i = 0; i < config.levelConfig.levelConfigDataEdited.Count; i++)
            {
                var mapData = config.levelConfig.levelConfigDataEdited[i].GetMapData();
                if (mapData.groupSkinId == themeNeedToFind)
                {
                    levelIds.Add(i + 1);
                }
            }

            Debug.LogError(new JsonDotNetModule().ToJson(levelIds));
        }

        [Button]
        public void GetGroundThemeModel()
        {
            foreach (var groundThemeModel in groundThemeModels)
            {
                groundThemeModel.materials.Clear();
                groundThemeModel.textures.Clear();
                var themeConfigModel = groundThemeConfigModels.Find(x => x.themeId == groundThemeModel.themeId);
                foreach (var groundElement in themeConfigModel.groundElements)
                {
                    var meshes = groundElement.GetComponentsInChildren<MeshRenderer>();
                    foreach (var mesh in meshes)
                    {
                        foreach (var mat in mesh.sharedMaterials)
                        {
                            if (mat == null) continue;
                            if (!groundThemeModel.materials.Contains(mat))
                            {
                                groundThemeModel.materials.Add(mat);
                            }

                            Texture texture = null;

                            // Check for each texture property and assign to texture variable if exists
                            if (mat.HasProperty("_MainTex"))
                            {
                                texture = mat.GetTexture("_MainTex");
                            }
                            else if (mat.HasProperty("MainTexture"))
                            {
                                texture = mat.GetTexture("MainTexture");
                            }
                            else if (mat.HasProperty("_Diffuse"))
                            {
                                texture = mat.GetTexture("_Diffuse");
                            }

                            // Add the found texture to the groundThemeModel if it exists
                            if (texture != null && !groundThemeModel.textures.Contains(texture as Texture2D))
                            {
                                groundThemeModel.textures.Add(texture as Texture2D);
                            }
                        }
                    }
                }
            }
        }   
        public WaterThemeConfig GetWaterThemeById(int id)
        {
            var result = waterThemeConfigs.Find(x => x.themeId == id);
            if (result == null)
            {
                return waterThemeConfigs[0];
            }
            return waterThemeConfigs.Find(x => x.themeId == id);
        }
        public WaterThemeConfig GetWaterThemeForLevel(int level)
        {
            level =level + 1;// ADd 1 level for bug Task #69952, theme change from 1-25
            if (level < stepChangeTheme)
                return waterThemeConfigs[0];

            var listThemeNormal = waterThemeConfigs.FindAll(t => t.themeType != WaterThemeType.Hard);
            int normalCount = listThemeNormal.Count;

            int index = level / stepChangeTheme;

            if (index < normalCount)
                return listThemeNormal[index];
            
            int cycleLength = normalCount;
            int cycleIndex = (index - normalCount) % cycleLength;
            
            int pseudoRandomIndex = (cycleIndex * 997 + 313) % normalCount;

            return listThemeNormal[pseudoRandomIndex];
        }


        [Button]
        public void TestSceneTheme()
        {
            listTest.Clear();
            for (int i = 0; i < 1000; i += 25)
            {
                listTest.Add(new TestLevelTheme(i, GetWaterThemeForLevel(i).themeId));
            }
        }

        public List<TestLevelTheme> listTest = new List<TestLevelTheme>();
    }

    [Serializable]
    public class TestLevelTheme
    {
        public TestLevelTheme(int level, int themeID)
        {
            this.level = level;
            this.themeID = themeID;
        }

        public int level;
        public int themeID;
    }

    [Serializable]
    public class GroundThemeConfigModel
    {
        public string themeId;
        public List<GroundElement> groundElements;

        public GroundElement GetElementById(string id)
        {
            return groundElements.Find(x => x.Id == id);
        }
    }

    [Serializable]
    public class GroundThemeModel
    {
        public string themeId;
        public List<Material> materials;
        public List<Texture2D> textures;
    }

    [Serializable]
    public class WaterThemeConfig
    {
        public int themeId;
        public WaterThemeType themeType;
        public Material waterSeaMaterial;
        public GameObject whirlPoolHole;
        public Sprite bgSprite;
        public ParticleSystem splashEffect;
        public Color baseItemColor = Color.white;
    }

    public enum WaterThemeType
    {
        Day,
        Night,
        Hard
    }
}