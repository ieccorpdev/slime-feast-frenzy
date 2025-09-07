using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay;
using _IEC.Features.Gameplay.ObserverEventData;
using DG.Tweening;
using IEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayableController : _BaseBehaviour
{
    private static PlayableController instance;
    [FormerlySerializedAs("gameConfig")] public GamePlayConfig gamePlayConfig;
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

    // public LevelDataContainerSO LevelDataSO;
    //
    // [SerializeField] private GameObject stack;
    [Header("Playable Config")]
    [LunaPlaygroundField("map height", 0, "Config")]
    public float mapHeight = 0;
    [LunaPlaygroundField("fixedTimeStep", 0, "Config")]
    public float fixedTimeStep = 0.12f;
    public int level;
    public PrefabConfig prefabConfig;
    public bool isEndGame = false;
    [LunaPlaygroundField("max Item Spawn", 0, "Config")]
    public int maxItemSpawn = 200;
    public int countShowTutorial;

    public bool PlayableStarted
    {
        get;
        private set;
    }

    public bool InitMoveAnimDone
    {
        get;
        private set;
    }

    public ConfettiGroup ConfettiGroup;

    public InGamePlayGroup InGamePlayGroup;

    public FloatingJoystick Joystick;
    public Material waveWaterMaterial;
    // public LevelDataSO levelData; 
    // public List<ItemDefinition> ListItemDefinition;

    public PlayableTutorial PlayableTutorial;
    [Space]
    [Header("List level to play")]
    public List<ItemDefinition> itemDefinitions;
    [Space]
    public List<LevelDataSO> levelDatas;
    [Space]
    [LunaPlaygroundField("Use Item")] public bool isUseItem = true;
    [LunaPlaygroundField("Levels index")] public int[] levelIndexs;
    public int currentLevelIndex;
    public LevelDataSO CurrentLevel => levelDatas[levelIndexs[currentLevelIndex]];


    public bool IsUseMagnetHelper = false;




#if UNITY_EDITOR
    public GameplayItemPrefabConfig itemPrefabConfig;

    // [Button]
    // public void InitItemDefinition()
    // {
    //     if (ListItemDefinition == null)
    //     {
    //         ListItemDefinition = new List<ItemDefinition>();
    //     }
    //     ListItemDefinition.Clear();
    //     foreach (var itemSpawn in levelData.levelSpawnData.listItemSpawns)
    //     {
    //         var itemDefinition = this.itemPrefabConfig.GetListItemDefinition().Find(x => x.GetId() == itemSpawn.id);
    //         if (itemDefinition == null)
    //         {
    //             Debug.LogError($"Item Definition with id {itemSpawn.id} not found in prefab config.");
    //             continue;
    //         }
    //         ListItemDefinition.Add(itemDefinition);
    //     }
    //     EditorUtility.SetDirty(this);
    // }

    [Button]
    public void InitAllItemDefination()
    {
        if (itemDefinitions == null) itemDefinitions = new();
        itemDefinitions.Clear();
        levelDatas?.ForEach(levelData =>
        {
            if (levelData.levelSpawnData == null)
            {
                Debug.LogError("Không có level Spawn Data rồi bé ơi");
            }
            foreach (var itemSpawn in levelData.levelSpawnData.listItemSpawns)
            {
                var itemDefinition = this.itemPrefabConfig.GetListItemDefinition().Find(x => x.GetId() == itemSpawn.id);
                if (itemDefinition == null)
                {
                    Debug.LogError($"Item Definition with id {itemSpawn.id} not found in prefab config.");
                    continue;
                }
                if (!itemDefinitions.Contains(itemDefinition))
                {
                    itemDefinitions.Add(itemDefinition);
                }
            }
        });
    }

#endif
    // public ItemDefinition GetItemDefinition(string itemId)
    // {
    //     var itemDefinition = this.ListItemDefinition.Find(x => x.GetId() == itemId);
    //     return itemDefinition;
    // }

    public ItemDefinition GetItemDefinition(string itemId)
    {
        var itemDefinition = this.itemDefinitions.Find(x => x.GetId() == itemId);
        return itemDefinition;
    }

    public void Awake()
    {
        instance = this;
#if UNITY_EDITOR
        InitAllItemDefination();
#endif
        Application.targetFrameRate = 60;
        if (fixedTimeStep > 0)
            Time.fixedDeltaTime = fixedTimeStep;
    }

    private void Start()
    {
        LoadLevel(currentLevelIndex);
        if(!isUseItem)
        {
            PlayableTutorial.HideTut();
        }
    }


    public void OnMagnetButtonClicked()
    {
        IsUseMagnetHelper = true;
        this.RaiseEvent(EventList.GamePlay.RESUME_GAME);
        this.RaiseEvent(EventList.GamePlay.USER_USE_HELPER, new UserUseHelper()
        {
            helperId = HelperId.Magnet
        });
    }
    public void OnExpandButtonClicked()
    {
        print("OnExpandButtonClicked");
        this.RaiseEvent(EventList.GamePlay.RESUME_GAME);
        this.RaiseEvent(EventList.GamePlay.USER_USE_HELPER, new UserUseHelper()
        {
            helperId = HelperId.ScaleUp
        });
    }

    public void LoadLevel(int level)
    {
        this.level = level;
        EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
    }

    public void DoneInitMoveAnim()
    {
        InitMoveAnimDone = true;
    }

    public void OnEndOfTime()
    {
        ShowEndCard();
    }
    public void OnEndOfMove()
    {
        DOVirtual.DelayedCall(0.75f, ShowEndCard);
    }
    public void OnWin()
    {
        ShowEndCard();
    }

    public void OnLose()
    {
        ShowEndCard();
    }

    public void ShowEndCard()
    {
        screenLevelWin.Show();
        isEndGame = true;
    }
    public void DownLoad()
    {
        Luna.Unity.Playable.InstallFullGame();
        Luna.Unity.LifeCycle.GameEnded();
        Debug.Log("Download started");
    }


    public void NextLevel()
    {
        StartCoroutine(NextLevelCouroutine());
    }
    public void StopShowConfetti()
    {
        ConfettiGroup.StopConfetti();
    }

    private IEnumerator NextLevelCouroutine()
    {
        yield return new WaitUntil(() => !GroupLevelObjectives.Instance.IsCollecting);
        ConfettiGroup.ShowConfetti();
        Invoke(nameof(StopShowConfetti), 3f);
        if (currentLevelIndex < levelIndexs.Length)
        {
            currentLevelIndex++;
            LoadLevel(currentLevelIndex);
            EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
        }
    }    
}
