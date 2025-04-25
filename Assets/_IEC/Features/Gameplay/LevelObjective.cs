using System.Collections;
using System.Linq;
using IEC;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    [System.Serializable]
    public class LevelObjective
    {
        [ValueDropdown(nameof(GetAllItemIds))]
        public string itemId;
        public int number;

        public LevelObjective()
        {
            
        }

        public LevelObjective(string id, int num)
        {
            itemId = id;
            number = num;
        }
        
        public LevelObjective Clone()
        {
            return new LevelObjective()
            {
                itemId = itemId,
                number = number
            };
        }
        
        public LevelObjectiveData ToLevelObjectiveData()
        {
            return new LevelObjectiveData(itemId, number);
        }
        
        private static IEnumerable GetAllItemIds()
        {
#if UNITY_EDITOR
            var itemConfigPath = UnityEditor.AssetDatabase.FindAssets("t:GameplayItemPrefabConfig");
            if (itemConfigPath.Length == 0)
            {
                Debug.LogError("GameplayItemPrefabConfig not found");
                return null;
            }

            var itemConfig = UnityEditor.AssetDatabase.LoadAssetAtPath<GameplayItemPrefabConfig>
                (UnityEditor.AssetDatabase.GUIDToAssetPath(itemConfigPath[0]));
            return itemConfig.listItemDefinitionShp.Select(x => x.GetId());
#endif
            return null;
        }
    }
}