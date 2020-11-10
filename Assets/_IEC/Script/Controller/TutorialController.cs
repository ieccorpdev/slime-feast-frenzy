using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace IEC
{
    public class TutorialController : _BaseBehaviour, IController, IEventListener
    {
        private Tween m_TweenTut;

        public TutorialConfig tutorialConfig;
        public RotTutorialConfig rotTutConfig;

        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.GAME_SPAWN_LEVEL:
                    DoneTutorial();
                    break;

                case EventList.GamePlay.GAME_CHECK_TUTORIAL:
                    if (Game.isHaveTut)
                    {
                        tutorialConfig = IsHaveLevelTut();
                        Game.isTutorial = tutorialConfig != null;

                        if (Game.isTutorial)
                        {
                            StartTutotial();
                        }
                    }
                    break;
                case EventList.GamePlay.GAME_GRID_CLICK_TUTORIAL:
                    SetupTut();
                    break;
            }
        }

        private TutorialConfig IsHaveLevelTut()
        {
            var count = Config.listTuts.Count;
            for (int i = 0; i < count; i++)
            {
                var levelTut = Config.listTuts[i];
                if(levelTut.level == PlayerData.level)
                {
                    return levelTut;
                }
            }

            return null;
        }

        private void StartTutotial()
        {
            rotTutConfig = null;
            Game.gridRotCount = 0;
            Game.gridRotCount = 0;

            SetupTut();
        }

        private void SetupTut()
        {
            if (rotTutConfig != null)
            {
                Game.gridRotCount += 1;
                if (Game.gridRotCount >= rotTutConfig.rotCount)
                {
                    Game.tutCount += 1;
                    if (Game.tutCount >= tutorialConfig.listRotConfigs.Count)
                    {
                        DoneTutorial();
                        return;
                    }
                    Game.gridRotCount = 0;
                    rotTutConfig = tutorialConfig.listRotConfigs[Game.tutCount];
                    ShowTut(rotTutConfig.row, rotTutConfig.col);
                }
            }
            else
            {
                rotTutConfig = tutorialConfig.listRotConfigs[Game.tutCount];
                ShowTut(rotTutConfig.row, rotTutConfig.col);
            }
        }

        private void DoneTutorial()
        {
            m_TweenTut?.Kill(true);
            GamePlay.transTutHand.gameObject.SetActive(false);
        }

        private void ShowTut(int row, int col)
        {
            m_TweenTut?.Kill(true);

            var grid = Game.grids[row + 1, col + 1];
            Game.gridTutId = grid.ID;

            var posOrigin = grid.CurrentPos;
            var targetDir = (-Vector3.forward + Vector3.right).normalized;

            var posSpawn = posOrigin + targetDir * .75f;
            var posTarget = posOrigin + targetDir * .25f;
            posSpawn.y = posTarget.y = .4f;

            GamePlay.transTutHand.position = posSpawn;
            GamePlay.transTutHand.gameObject.SetActive(true);
            m_TweenTut = GamePlay.transTutHand.DOMove(posTarget, .5f).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
// Level 1, 5, 15, 