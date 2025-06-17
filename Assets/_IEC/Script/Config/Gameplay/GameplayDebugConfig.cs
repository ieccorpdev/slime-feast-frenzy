using System.Collections.Generic;
using System.Linq;
using _IEC.Features.ExportLevel.OriginalLevel;
using _IEC.Features.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/GameplayDebugConfig")]
    public class GameplayDebugConfig : BaseConfig
    {
        public LevelConfig levelConfig;
        public GameplayItemPrefabConfig gameplayItemPrefabConfig;
        public List<ItemDefinition> itemDefinitionsInLevelConfig;
        
        public Vector3 startPosition;
        public Vector3 gridOffset = new Vector3(5, 0 , 5);
        public int columns = 5;
        
        [Button]
        public void GetItemDefinitionsInLevelConfig()
        {
            itemDefinitionsInLevelConfig.Clear();
            HashSet<string> itemIds = new HashSet<string>();
            //var levelConfigData = Game.levelVersion == 1 ? Config.levelConfig.levelConfigNoBomb : Config.levelConfig.levelConfigDataEdited;
            var levelConfigData = levelConfig.levelConfigDataEdited.ToList();
            levelConfigData.AddRange(levelConfig.levelConfigDataSHP.ToList());
            foreach (var configModel in levelConfigData)
            {
                if (configModel == null) continue;
                if (configModel.itemInstantiateItemData == null) continue;
                var root = new JsonDotNetModule().FromJson<Root>(configModel.itemInstantiateItemData.text);
                foreach (var instantiateData in root.InstantiateData)
                {
                    itemIds.Add(instantiateData.itemId);
                }
            }

            // foreach (var id in itemIds)
            // {
            //     itemDefinitionsInLevelConfig.Add(gameplayItemPrefabConfig.listItemDefinition.Find(item => item.GetId() == id));
            // }

            var listItems = gameplayItemPrefabConfig.listItemDefinition.ToList();
           // listItems.AddRange(gameplayItemPrefabConfig.listItemDefinitionShp.ToList());
            foreach (var item in listItems)
            {
                if (itemIds.Contains(item.GetId()))
                {
                    itemDefinitionsInLevelConfig.Add(item);
                }
            }
        }
        
    }
}