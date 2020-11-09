using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace IEC
{
    public class TutorialController : _BaseBehaviour, IController, IEventListener, IEventAwake
    {
        private Tween m_TweenTut;

        private int m_MoveCount;

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void BehaviourAwake()
        {
            //ShowTut(2, 2);
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.GAME_CHECK_TUTORIAL:
                    if (Game.isHaveTut)
                    {
                        Game.isTutorial = IsHaveLevelTut();
                    }
                    break;
            }
        }

        private bool IsHaveLevelTut()
        {
            var count = Config.listTuts.Count;
            for (int i = 0; i < count; i++)
            {
                var levelTut = Config.listTuts[i];
                if(levelTut.level == PlayerData.level)
                {
                    return true;
                }
            }

            return false;
        }

        public int Row, Col;

        [NaughtyAttributes.Button]
        private void TestTut()
        {
            Game.isTutorial = true;
            ShowTut(Row, Col);
        }

        private void ShowTut(int row, int col)
        {
            m_TweenTut?.Kill(true);
            var grid = Game.grids[row + 1, col + 1];
            Game.gridTutId = grid.ID;
            var posOrigin = grid.CurrentPos;
            var targetDir = (-Vector3.forward + Vector3.right).normalized * .5f;
            posOrigin.y = targetDir.y = .5f;
            var posSpawn = posOrigin + targetDir;
            GamePlay.transTutHand.position = posSpawn;
            GamePlay.transTutHand.DOMove(posOrigin, .5f).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
// Level 1