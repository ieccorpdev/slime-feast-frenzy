using System.Collections;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay;
using _IEC.Features.Gameplay.ObserverEventData;
using _IEC.Features.VipProHackerUITutorial;
using DG.Tweening;
using IEC;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayableController : _BaseBehaviour
{
    private static PlayableController instance;
    [FormerlySerializedAs("gameConfig")] public GamePlayConfig gamePlayConfig;
    public GameplayItemPrefabConfig GameplayItemPrefabConfig;
    public ScreenInGame ScreenInGame;
    public bool useHelper;
    public static PlayableController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayableController>();
            }
            return instance;
        }
    }
    public ScreenLevelWin screenLevelWin;

     public int level;

     public PrefabConfig prefabConfig;

    

    public SoundController SoundController;

    public ConfettiGroup ConfettiGroup;

    public InGamePlayGroup InGamePlayGroup;

    public FloatingJoystick Joystick;
    public LevelConfig levelConfig;
    public Material waveWaterMaterial;

    public void Awake()
    {
        instance = this;
    }
    
    private void Start()
    {

        EventController.RaiseEvent(EventList.Audio.BACKGROUND_INGAME);
        LoadLevel(level);
    }

    public LevelDataSO GetLevelSpawnData(int level)
    {
        return levelConfig.levelData.listAllLevelData[level];
    }
    
    public void LoadLevel(int level)
    {
        this.level = level;
        EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
    }


    public void ShowEndCard()
    {
        screenLevelWin.Show();
    }
    public void DownLoad()
    {
        Luna.Unity.Playable.InstallFullGame();
        Luna.Unity.LifeCycle.GameEnded();
    }
}
