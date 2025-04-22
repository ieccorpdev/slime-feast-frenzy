using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/AllChestConfig")]
public class AllChestConfig : ScriptableObject
{
    public List<ChestConfig> listAllChestConfig = new();

    public ChestConfig GetChestConfig(ChestType chestType)
    {
        return listAllChestConfig.FirstOrDefault(config => config.chestType == chestType);
    }
}
