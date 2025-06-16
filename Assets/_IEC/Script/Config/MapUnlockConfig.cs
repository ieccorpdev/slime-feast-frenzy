using System.Collections.Generic;
using UnityEngine;

namespace _IEC.Features.Common.Config
{
    [CreateAssetMenu(menuName = "Game/Features/Common/MapUnlockConfig")]
    public class MapUnlockConfig : ScriptableObject
    {
        public List<MapUnlockConfigModel> mapUnlockConfigData;

        public MapUnlockConfigModel GetNextMapUnlockModel(int level)
        {
            MapUnlockConfigModel currentUnlockModel = null;
            for (int i = 0; i < mapUnlockConfigData.Count; i++)
            {
                var model = mapUnlockConfigData[i];
                if (model.unlockLevel > level)
                {
                    currentUnlockModel = model;
                    break;
                }
            }
            return currentUnlockModel;
        }

        public (int process,int target) GetProcessAndTarget(int level)
        {
            var currentModel = GetNextMapUnlockModel(level);
            var indexOfCurrentModel = mapUnlockConfigData.IndexOf(currentModel);
            int formerLevel = 1;
            if (indexOfCurrentModel > 0)
            {
                formerLevel = mapUnlockConfigData[indexOfCurrentModel - 1].unlockLevel;
            }

            int target = currentModel.unlockLevel - formerLevel;
            int process = level - formerLevel;
            return (process, target);
        }
    }
}