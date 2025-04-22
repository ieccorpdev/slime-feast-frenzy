using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public interface ILevelController
    {
        public void Dispose();
        public void LoadLevel(int level);
        public void StartPlay();
        public LevelData GetLevelData();
        public Hole GetHolePlayer();
    }
}
