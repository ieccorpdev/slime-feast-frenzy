using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class GameController : _BaseBehaviour, IController, IEventStart, IEventListener, IEventAwake, IEventUpdate
    {
        private List<int> ADJUST_LEVEL_EVENT = new List<int> { 4, 10, 11, 14, 30, 40, 50, 55, 60, 70 };

        private static float RATIO_DEFAULT = 16f / 10;
        private static float RATIO_CAMERA_SIZE_MIN = 1.45f / (21f / 9 - 16f / 10);
        private static float RATIO_CAMERA_SIZE_MAX = 5.6f / (21f / 9 - 16f / 10);

        private float m_ScreenRatio, m_DeltaScreenRatio, m_DeltaSizeMinCam, m_RatioCamSize;

        private Vector3 m_CamDirOrigin, m_CamOriginPos;

        private bool m_IsInitCam;

        private System.DateTime m_TimeStart;

        private List<BaseComponent> m_ListGridHaveWater = new List<BaseComponent>();

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
            
        }

        public void SetupMainCam()
        {
            GamePlay.transMainCam.rotation = Quaternion.identity;
            var posY = Game.isHaveIsLand ? 4.3f : 5.2f;
            var posZ = Game.isHaveIsLand ? -3.85f : -2.16f;

            var rotX = Game.isHaveIsLand ? 55f : 70f;
            GamePlay.mainCamera.transform.localRotation = Quaternion.Euler(rotX, 0f, 0f);
            GamePlay.mainCamera.transform.localPosition = new Vector3(0f, posY, posZ);

            m_CamOriginPos = GamePlay.mainCamera.transform.localPosition;
            m_CamDirOrigin = GamePlay.mainCamera.transform.forward;
        }

        public void BehaviourStart()
        {
            EventController.RaiseEvent(EventList.GamePlay.GAME_SPAWN_LEVEL);
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.GAME_CHECK_CONNECT:
                    if (!Game.isPlaying) return;
                    CheckConnect();
                    break;
                case EventList.GamePlay.GAME_START_PLAY:
                    PrepareGameStart();
                    break;
                case EventList.GamePlay.GAME_CHECK_COMPLETE:
                    if (IsCompleteAll())
                    {
                        EventController.RaiseEventDelay(.5f, EventList.GamePlay.GAME_OVER_UI, new EventParam(this, true));
                    }
                    break;
                case EventList.GamePlay.GAME_UNDO:
                    OnUndo();
                    break;
            }
        }

        public void BehaviourUpdate(float deltaTime)
        {
            Game.listGrids.ForEach(x => x.OnUpdate(deltaTime));
        }

        private void PrepareGameStart()
        {             
            Game.listGrids.ForEach(x => x.AfterInit());
            var min = Game.size * 8;
            var max = min * 2;

            Game.goldReward = RandomHelper.RangeWithSeed(PlayerData.level, min, max);

            Game.SetGameState(GameState.Playing);
            m_TimeStart = System.DateTime.Now;
            
            PushPlayLevelEvent();

            //EventController.RaiseEvent(EventList.GamePlay.GAME_CHECK_TUTORIAL);
            EventController.RaiseEvent(EventList.GamePlay.GAME_CHECK_CONNECT);
        }

        public void SetupCamera(int size, int rotIndex)
        {
            if (!m_IsInitCam)
            {
                m_ScreenRatio = Screen.height / (float)Screen.width;
                m_DeltaScreenRatio = m_ScreenRatio - RATIO_DEFAULT;

                m_DeltaSizeMinCam = RATIO_CAMERA_SIZE_MIN * m_DeltaScreenRatio;
                m_IsInitCam = true;

                var distMax = (Game.isHaveIsLand ? 13.5f : 9f) + (RATIO_CAMERA_SIZE_MAX * m_DeltaScreenRatio);

                m_RatioCamSize = (distMax - m_DeltaSizeMinCam) / 6f;
            }

            var delta = ((size - 5) * m_RatioCamSize) + m_DeltaSizeMinCam;
            var cameraPos = m_CamOriginPos - (delta * m_CamDirOrigin);
            
            GamePlay.mainCamera.transform.localPosition = cameraPos;

            var rotY = rotIndex * 90f;
            if(Game.isHaveIsLand)
            {
                rotY = 45f;
            }
            GamePlay.transMainCam.localRotation = Quaternion.Euler(0f, rotY, 0f);
        }

        private void CheckConnect()
        {
            Game.listGrids.ForEach(x => x.PrepareCheckConnect());

            var sourceCount = Game.listSource.Count;
            var totalGrid = Game.size * Game.size;

            for (int i = 0; i < sourceCount; i++)
            {
                var source = Game.listSource[i];

                m_ListGridHaveWater.Clear();
                m_ListGridHaveWater.Add(source);

                bool isChanged = true;

                var totalGridHaveWater = m_ListGridHaveWater.Count;
                var startIndex = 0;

                while (totalGridHaveWater < totalGrid && isChanged)
                {
                    var count = m_ListGridHaveWater.Count;
                    for (int j = startIndex; j < count; j++)
                    {
                        var grid = m_ListGridHaveWater[j];
                        var listGrids = grid.GetGridsCanConnect(source.WaterColorIndex, false);
                        startIndex += 1;
                        var listGridNearCount = listGrids.Count;
                        if (listGrids.Count > 0)
                        {
                            for (int x = 0; x < listGridNearCount; x++)
                            {
                                var gridNearConnect = listGrids[x];

                                if (!m_ListGridHaveWater.Contains(gridNearConnect))
                                {
                                    m_ListGridHaveWater.Add(gridNearConnect);
                                    gridNearConnect.SetConnect(source.WaterColorIndex, grid.DelayStart);
                                }
                            }
                        }
                    }

                    totalGridHaveWater = m_ListGridHaveWater.Count;
                    if (totalGridHaveWater == count)
                    {
                        isChanged = false;
                    }
                }
            }

            Game.listGrids.ForEach(x => x.AfterCheckConnect());

            if (IsWin())
            {
                GameOver(true);
            }
        }

        public void GameOver(bool isWin)
        {
            Game.isGameWin = isWin;
            Game.SetGameState(GameState.GameOver);

            Game.timePlay = (int)(System.DateTime.Now - m_TimeStart).TotalSeconds;
            Services.PushEvent(new IEC.TrackingEvent.GameOver());

            if (isWin)
            {
                PlayerData.countPlayThisLevel = 0;
                PlayerData.level += 1;
            }
            else
            {
                EventController.RaiseEventDelay(.5f, EventList.GamePlay.GAME_OVER_UI, new EventParam(this, false));
            }

        }

        private bool IsWin()
        {
            var count = Game.listTarget.Count;
            for (int i = 0; i < count; i++)
            {
                if (!Game.listTarget[i].IsDone) return false;
            }

            return true;
        }

        private bool IsCompleteAll()
        {
            var count = Game.listTarget.Count;
            for (int i = 0; i < count; i++)
            {
                if (!Game.listTarget[i].IsComplete) return false;
            }
            return true;
        }

        private void OnUndo()
        {
            var rotConfig = Game.GetLastUndo();
            var grid = Game.grids[rotConfig.Row, rotConfig.Col];
            grid.OnUndo();
        }

        public void PushPlayLevelEvent()
        {
            PlayerData.countPlayThisLevel += 1;
            var level = PlayerData.level;
            Services.PushEvent(new IEC.TrackingEvent.PlayLevel());

            if (PlayerData.levelFirstTime != level)
            {
                PlayerData.levelFirstTime = level;
                Services.PushEvent(new IEC.TrackingEvent.PlayLevelFirstTime());
                
                if (level < 15)
                {
                    Services.PushEvent($"play_level_first_time_{level}");
                }
            }

            var isMislestonePushLevel = level % Mathf.FloorToInt(Mathf.Pow(10, level.ToString().Length - 1)) == 0;

            if (isMislestonePushLevel || ADJUST_LEVEL_EVENT.Contains(level))
                Services.PushEvent($"play_level_{level}");
        }
    }
}
