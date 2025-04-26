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

public class PlayableController : MonoBehaviour
{
    private static PlayableController instance;
    [FormerlySerializedAs("gameConfig")] public GamePlayConfig gamePlayConfig;
    public GameplayItemPrefabConfig GameplayItemPrefabConfig;
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
     [LunaPlaygroundField("Level", 0, "Config")]
     public int level;

     public PrefabConfig prefabConfig;

     // [LunaPlaygroundField("Limit Time", 0, "Config")]
    // public float limitTime;
    // [LunaPlaygroundField("Box Move Out", 0, "Config")]
    // public bool boxMoveOut;
    // [LunaPlaygroundField("Unlock All Stack", 0,"Config")]
    // public bool unlockAllStack;
    // [LunaPlaygroundField("Start Pos Tile", 0, "Config")]
    // public int startPosTile = 10;
    // [LunaPlaygroundField("Time Start Move", 0, "Config")]
    // public float timeStartMove = 0.7f;
    // [LunaPlaygroundField("Limit Move",0, "Config")]
    // public int limitMove;
    // [LunaPlaygroundField("Hide Stack",0, "Config")]
    // public bool hideStack;
    // [Header("Tutorial")]
    // [LunaPlaygroundField("Num Tutorial", 0, "Config")]
    // public int numTutorial;
    // private int currentTutorial;
    // [LunaPlaygroundField("Tutorial Duration", 0, "Config")]
    // public float tutorialDuration;
    // private float time;
    // private bool isShowingTutorial;
    //
    //
    // [Header("Game Param")]
    //
    // [LunaPlaygroundField("Use Dark BG", 0, "Config")]
    // public bool isGameUseDarkBG;
    // [LunaPlaygroundField("Do Spawn Anim", fieldSection: "Config")]
    // public bool isDoSpawnAnim;
    // [LunaPlaygroundField("Click Single Tile", fieldSection: "Config")]
    // public bool isGameClickSingleTile;
    // [LunaPlaygroundField("All Tile Hidden", 0, "Config")]
    // public bool isAllTileHidden;
    // [LunaPlaygroundField("Random Tile Hidden", 0, "Config")]
    // public bool isRandomTileHidden;
    // [LunaPlaygroundField("Random Tile Hidden Rate", 0, "Config")]
    // public float randomTileHiddenRate = 0.2f;
    // [LunaPlaygroundField("Amount Tile Start Not Hidden", 0, "Config")]
    // public int amountTileStartNotHidden = 6;
    // [LunaPlaygroundField("Amount Tile Allow CLick",0, "Config")]
    // public int amountTileAllowClick = 8;
    // [LunaPlaygroundField("Amount Spawn From Pos 0", 0, "Config")]
    // public int amountSpawnFromPos0 = 2;
    //
    // [LunaPlaygroundField("Basic Tile Move Speed", 0, "Config")]
    // public float basicTileMoveSpeed = 0.5f;
    // [LunaPlaygroundField("Basic Tile Move Speed",0,"Config")]
    // public float tileMoveSpeedOnRemoveAnotherTile = 10;
    // [LunaPlaygroundField("Shape Id", 0, "Config")]
    // public int shapeId = 0;
    // public Camera MainCamera;
    // [Space(10)]
    //
    // private Sequence sq;
    // public Transform hand;
    // [Header("End Card")] 
    // public CanvasGroup endCard;
    //
    // public RectTransform icon;
    // public RectTransform playNow;
    // [HideInInspector]public bool isGameEnded;
    // private float lastTimeTap;
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

        ShowTutorial();

        switch (level)
        {
            case 1:
                break;
            case 2:
                magnetButton.onClick.RemoveAllListeners();
                magnetButton.onClick.AddListener(OnMagnetButtonClicked);
                StartCoroutine(IShowHighLight());
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
        HighLightCanvas.gameObject.SetActive(true);
        this.RaiseEvent(EventList.GamePlay.PAUSE_GAME);
    }

    private void OnMagnetButtonClicked()
    {
        
        HighLightCanvas.gameObject.SetActive(false);
        this.RaiseEvent(EventList.GamePlay.RESUME_GAME);
        this.RaiseEvent(EventList.GamePlay.USER_USE_HELPER, new UserUseHelper()
        {
            helperId = HelperId.Magnet
        });
    }
    public HighLightCanvas HighLightCanvas;
    public Button magnetButton;

    public LevelDataSO GetLevelSpawnData(int level)
    {
        return levelConfig.levelData.listAllLevelData[level];
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
//        GameController.OnNewGame();
        //levelController.LoadLevel(level);
    }

    public void DoneInitMoveAnim()
    {
        InitMoveAnimDone = true;
        ShowTutorial();
    }

    private void Update()
    {
        ProcessInput();
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
        // float timeInerval = 5;
        // if (Time.time - lastTimeTap  >= 5)
        // {
        //     ShowTutorial();
        // }
        // if(Input.GetKeyDown(KeyCode.LeftArrow))
        // {
        //     LoadLevel(level - 1);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.RightArrow))
        // {
        //     LoadLevel(level + 1);
        // }
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
    }
    public void DownLoad()
    {
        Luna.Unity.Playable.InstallFullGame();
        Luna.Unity.LifeCycle.GameEnded();
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
}
