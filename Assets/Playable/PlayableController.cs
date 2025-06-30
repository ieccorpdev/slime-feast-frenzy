using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay;
using _IEC.Features.Gameplay.ObserverEventData;
using _IEC.Features.VipProHackerUITutorial;
using DG.Tweening;
using IEC;
using Sirenix.OdinInspector;
using UnityEditor;
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
    public LevelDataSO levelData;
    public List<ItemDefinition> ListItemDefinition;
#if UNITY_EDITOR
    public GameplayItemPrefabConfig itemPrefabConfig;

    [Button]
    public void InitItemDefinition()
    {
        if (ListItemDefinition == null)
        {
            ListItemDefinition = new List<ItemDefinition>();
        }
        ListItemDefinition.Clear();
        foreach (var itemSpawn in levelData.levelSpawnData.listItemSpawns)
        {
            var itemDefinition = this.itemPrefabConfig.GetListItemDefinition().Find(x => x.GetId() == itemSpawn.id);
            if (itemDefinition == null)
            {
                Debug.LogError($"Item Definition with id {itemSpawn.id} not found in prefab config.");
                continue;
            }
            ListItemDefinition.Add(itemDefinition);
        }
        EditorUtility.SetDirty(this);
    }
#endif
    public ItemDefinition GetItemDefinition(string itemId)
    {
        var itemDefinition = this.ListItemDefinition.Find(x => x.GetId() == itemId);
        return itemDefinition;
    }

    public void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
        if (fixedTimeStep > 0)
            Time.fixedDeltaTime = fixedTimeStep;
    }

    private void Start()
    {

        EventController.RaiseEvent(EventList.Audio.BACKGROUND_INGAME);
        LoadLevel(level);

        ShowTutorial();

        switch (level)
        {
            case 1:
                break;
            case 2:
                if (useHelper)
                {
                    magnetButton.onClick.RemoveAllListeners();
                    magnetButton.onClick.AddListener(OnMagnetButtonClicked);
                    StartCoroutine(IShowHighLight());
                }
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
        }
        

    }

    IEnumerator IShowHighLight()
    {
        yield return new WaitForSeconds(5);
        this.RaiseEvent(EventList.GamePlay.PAUSE_GAME);
    }
    public void OnMagnetButtonClicked()
    {
        this.RaiseEvent(EventList.GamePlay.RESUME_GAME);
        this.RaiseEvent(EventList.GamePlay.USER_USE_HELPER, new UserUseHelper()
        {
            helperId = HelperId.Magnet
        });
    }
    public void OnExpandButtonClicked()
    {
        this.RaiseEvent(EventList.GamePlay.RESUME_GAME);
        this.RaiseEvent(EventList.GamePlay.USER_USE_HELPER, new UserUseHelper()
        {
            helperId = HelperId.ScaleUp
        });
    }
    public Button magnetButton;

    public LevelDataSO GetLevelSpawnData(int level)
    {
        return levelData;
    }

    IEnumerator IE_DelayLoadLevel()
    {
        yield return new WaitForSeconds(3);
    }
    Color HexToColor(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            return color;
        }
        return Color.black; // fallback
    }
    public void LoadLevel(int level)
    {
        this.level = level;
        EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
    }

    public void DoneInitMoveAnim()
    {
        InitMoveAnimDone = true;
        ShowTutorial();
    }

    private void Update()
    {
        //ProcessInput();
        // time += Time.deltaTime;
        // if (time >= tutorialDuration)
        // {
        //     ShowTutorial();
        // }
    }

    private void ShowTutorial()
    {
        // if (currentTutorial >= numTutorial) return;
        // if (isShowingTutorial) return;
        // if (isGameEnded) return;
        // // Vector3 panelPos = LevelGenerator.GetRandomPanelPos();
        // // if(panelPos == Vector3.zero) return;
        //
        // // var tile = LevelGenerator.TilePanels[0].Tiles.FirstOrDefault(tile =>
        // //     tile.ColorType == LevelGenerator.TargetLines[0].TargetPanels[0].ColorType);
        // //
        // // Debug.Log(tile, tile);
        //
        // //Vector3 tilePos = LevelGenerator.TilePanels[0].Tiles[0].transform.position;
        // //Vector3 tilePos = tile.transform.position;
        //
        // //hand.position = tilePos + new Vector3(0.5f, 1,1);
        // hand.position =  new Vector3(0.76f, 1,8.78f);
        // isShowingTutorial = true;
        // hand.eulerAngles = new Vector3(90, 0, -20);
        // sq = DOTween.Sequence();
        // sq.Append(hand.DORotate(new Vector3(90,0,-5),0.4f));
        // sq.AppendInterval(0.1f);
        // sq.SetLoops(-1, LoopType.Yoyo);
        // time = 0;
        // lastTimeTap = Time.time;
        // currentTutorial++;
        // hand.gameObject.SetActive(true);
    }

    private void HideTutorial()
    {
        // if(!isShowingTutorial) return;
        // sq?.Kill();
        // isShowingTutorial = false;
        // hand.gameObject.SetActive(false);
    }
    private void ProcessInput()
    {
        float timeInerval = 5;
        // if (Time.time - lastTimeTap  >= 5)
        // {
        //     ShowTutorial();
        // }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            LoadLevel(level - 1);
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            LoadLevel(level + 1);
        }
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
        // if (isGameEnded) return;
        // HideTutorial();
        // isGameEnded = true;
        // endCard.alpha = 0;
        // icon.localPosition = Vector3.up * 800;
        // playNow.localPosition = Vector3.down * 800;
        // endCard.gameObject.SetActive(true);
        // endCard.alpha = 1;
        // //endCard.DOFade(1, 1f);
        // icon.DOLocalMoveY(200, 1f);
        // playNow.DOLocalMoveY(-300, 1f).OnComplete(() =>
        // {
        //     playNow.DOScale(Vector3.one * 1.1f, 0.75f).SetLoops(-1, LoopType.Yoyo);
        // });
        
        screenLevelWin.Show();
        isEndGame = true;
    }
    public void DownLoad()
    {
        Luna.Unity.Playable.InstallFullGame();
        Luna.Unity.LifeCycle.GameEnded();
        Debug.Log("Download started");
    }

    public void OnMove()
    {
        // time = 0;
        // lastTimeTap = Time.time;
        // HideTutorial();
        //
        // if (!PlayableStarted)
        //     PlayableStarted = true;

    }

    public void ClearCacheData()
    {
        levelData = null;
    }

    
}
