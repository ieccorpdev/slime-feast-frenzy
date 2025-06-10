using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class CharacterShardTab : _BaseBehaviour
    {
        public List<CharacterShardPack> listCharacterShardPacks = new();
        public void Setup()
        {
            for (int i = 0; i < listCharacterShardPacks.Count; i++)
            {
                listCharacterShardPacks[i].Setup(Config.characterShopConfig.listShardSlotData[i]);
            }
        }

        public void LoadData()
        {
            for (int i = 0; i < listCharacterShardPacks.Count; i++)
            {
                listCharacterShardPacks[i].LoadData(PlayerData.listCharacterPackData[i]);
            }
        }
    }
}
