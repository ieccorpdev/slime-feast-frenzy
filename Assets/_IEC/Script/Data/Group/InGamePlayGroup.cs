using _IEC.Features.Gameplay;
using UnityEngine;

namespace IEC
{
    public class InGamePlayGroup : MonoBehaviour, IGroup
    {
        public ScreenUI GameState { get; private set; }

        public Camera mainCamera;
        public Camera uiCamera;
        public LevelController levelController;
        
        public bool IsInLevelHard { get; private set; } = false;
        private ILevelController m_CurrentLevelController;
        public void Awake()
        {

        }
        public ILevelController GetLevelController()
        {
            return levelController;
        }

        public void Start()
        {
        }

        public void Init()
        {
            
        }

        public void LocateGroup()
        {
            Pool.Set(this);
        }

        public void SetGameState(ScreenUI screen)
        {
            GameState = screen;
        }

        public void SetNewLevel(int level)
        {
            levelController.LoadLevel(0);
        }
        public LevelType GetCurrentLevelType()
        {
            return m_CurrentLevelController.GetLevelData().levelType;
        }
        

        public void SetFinishPlay()
        {
            EventController.RaiseEvent(EventList.GamePlay.LEVEL_WIN);
        }

        public void StartGamePlay()
        {
            GetLevelController().StartPlay();
        }
        public void SetNewLevelHard()
        {

        }
        public void ClearCurrentLevel()
        {
            if (m_CurrentLevelController != null)
                m_CurrentLevelController.Dispose();
        }
        public void SetWin()
        {
            EventController.RaiseEvent(EventList.GamePlay.LEVEL_WIN);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }
    }
}