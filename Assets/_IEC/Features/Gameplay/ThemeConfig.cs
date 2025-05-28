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

        public WaterThemeConfig GetWaterThemeConfig()
        {
            int crId = Pool.Get<InGamePlayGroup>().currentThemeSelect;
            var reuslt = waterThemeConfigs.Find(x => x.themeId == crId);
            if (reuslt == null)
            {
                return waterThemeConfigs[0];
            }
            return reuslt;
        }
        public WaterThemeConfig GetWaterThemeConfig(int id )
        {
            var reuslt = waterThemeConfigs.Find(x => x.themeId == id);
            if (reuslt == null)
            {
                return waterThemeConfigs[0];
            }
            return reuslt;
        }
        public WaterThemeConfig GetWaterThemeForLevel(int level)
        {
            //MeoMew LGTM by chatGPT
            if (level < 25 || waterThemeConfigs == null || waterThemeConfigs.Count == 0)
                return waterThemeConfigs[0];

            int groupIndex = (level - 25) / 25; 
            
            WaterThemeType expectedType = (groupIndex % 2 == 0) ? WaterThemeType.Day : WaterThemeType.Night;
            
            var filtered = waterThemeConfigs.Where(c => c.themeType == expectedType).ToList();

            if (filtered.Count == 0)
            {
                //Not day / night
                return waterThemeConfigs[0];
            }
            // Tính số lượt đã qua cho mỗi loại (vì cứ 2 lần mới quay lại cùng 1 loại)
            int roundIndex = groupIndex / 2;

            // Xáo trộn theo index ổn định mà không cần cache
            // Sử dụng phép hoán vị theo seed riêng
            int seed = roundIndex + (int)expectedType * 1000;
            var shuffledIndex = PseudoShuffleIndex(seed, filtered.Count);

            return filtered[shuffledIndex];
        }
        int PseudoShuffleIndex(int seed, int length)
        {
            List<int> indices = Enumerable.Range(0, length).ToList();
            System.Random rng = new System.Random(seed / length); // Cố định seed theo "vòng"
            for (int i = 0; i < length; i++)
            {
                int swapIndex = rng.Next(i, length);
                (indices[i], indices[swapIndex]) = (indices[swapIndex], indices[i]);
            }
            return indices[seed % length];
        }
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
        Night
    }
}