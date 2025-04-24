using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public interface ILevelController
    {
         void Dispose();
         void LoadLevel(int level);
         void StartPlay();
         LevelData GetLevelData();
         Hole GetHolePlayer();
    }
}
