using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using IEC;
using MoreMountains.NiceVibrations;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class BuildCity : _BaseBehaviour
{
    //UI
    public Camera buildCityCamera;
    [Header("holdToBuild")] public GameObject holdToBuild;
    public TextMeshProUGUI nameCityTMP;
    public TextMeshProUGUI currentScrewTMP;
    public Button holdToBuildBtn;
    [Header("cityCompleteReward")] public GameObject cityCompleteReward;
    public Button nextRewardBtn;
    public Image titleComplete;
    [Header("cityContinue")] public GameObject cityContinue;
    public Image cityNameContinue;
    public TextMeshProUGUI nameCityEndingTMP;
    public Button continueBtn;
    [Header("cityContinue new")] public TextMeshProUGUI nameCityEndingTMP_2;
    public Button nextButton;
    public Button backButton;
    [Header("Reward")] public CityBuildRewardPanel cityReward;
    [Header("Data")] public List<IslandModelInfo> listAllIsland; //Move to prefab and create after
    public IslandModelInfo currentIsland;
    public UIBuildCityState currentState;
    public GameObject prefabScrewFly;
    public GameObject poolParent;
    public bool canSpendScrew;
    public List<GameObject> poolingScrewFlying;

    [Header("SelectedCity")] public GameObject poolCity;

    public int idSelectedIsland = 0;
    private float distanceEachIsland = 35;
    private float timeMoveIsland = 0.5f;
    private float defaultTimeEffect = 0.5f;

    private bool inAction;

    public void OnExitBuildCity()
    {
    }

    void Start()
    {
        CreatePool();
        ChangeStateUI();
        currentState = UIBuildCityState.NONE;
        continueBtn.onClick.AddListener(OnContinueClickBtn);
        holdToBuildBtn.onClick.AddListener(OnHoldingBuildCity);
        nextRewardBtn.onClick.AddListener(OnCompleteCity);
        backButton.onClick.AddListener(MoveBackIsland);
        nextButton.onClick.AddListener(MoveNextIsland);
        InitPositionAllIsland();
        ShowScale();
        MoveToCurrentIsland();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            MoveNextIsland();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveBackIsland();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Show();
        }
    }

    public void OnHoldingBuildCity()
    {
        if (PlayerData.screwCityBuild > 0 && currentIsland.maxScrewCanSpend > 0 && canSpendScrew)
        {
            Vector3 target = currentIsland.currentSelectedObj.GetComponent<IslandItem>().GetCenterObject();
            SpawnObjectScrew(GetRandomPositionInSphere(target, 1f));
        }

        if (PlayerData.screwCityBuild <= 0)
        {
            holdToBuildBtn.GetComponent<HoldingButton>().StopHolding();
            currentState = UIBuildCityState.CONTINUECITY;
            ChangeStateUI();
        }
    }

    #region BuildCityCore

    [Button()]
    public void ShowRewardCity()
    {
        currentState = UIBuildCityState.GETREWARD;
        ChangeStateUI();
        BuildCityDataConfig dataConfig = Config.buildCityConfig.dataCity.Find(x => x.idCity == currentIsland.idIsland);
        cityReward.SetupRewardPanel(dataConfig);
        cityReward.ShowingRewardPanel();
    }

    public void SpawnObjectScrew(Vector3 destination)
    {
        PlayerData.screwCityBuild -= 1;
        currentIsland.maxScrewCanSpend -= 1;
        GameObject screwFly = GetFlyScrewInPool();
        screwFly.transform.position = holdToBuildBtn.transform.position;
        screwFly.transform.rotation = UnityEngine.Random.rotation;
        var m_props = new MaterialPropertyBlock();
        m_props.SetVector("_Offset", new Vector4(0.223f * 0, -0.1f * UnityEngine.Random.Range(0, 10), 0, 0));
        screwFly.GetComponent<MeshRenderer>().SetPropertyBlock(m_props);
        bool lastScrew = currentIsland.maxScrewCanSpend == 0;
        screwFly.transform.DOJump(destination, 5f, 1, defaultTimeEffect).OnComplete(() =>
        {
            currentIsland.AddScrewToCurrentObject(lastScrew);
            screwFly.gameObject.SetActive(false);
            //TweenControl.GetInstance().ScaleFromOne(screwFly, 0.1f, () => { screwFly.gameObject.SetActive(false);});
        });
        PlayerData.buildCityData[currentIsland.idIsland].itemCity[currentIsland.currentModalSelected] += 1;
        currentScrewTMP.text = PlayerData.screwCityBuild.ToString();
    }

    public void ChangeAnimIsland(bool stt)
    {
        if (currentIsland)
        {
            currentIsland.StopMovingIsland(stt);
        }
    }

    public void OnClaimReward()
    {
        ChangeAnimIsland(true);
        PlayerData.currentCityBuild += 1;
        currentState = UIBuildCityState.COMPLETECITY;
        ChangeStateUI();
    }

    void OnCompleteCity()
    {
        currentState = UIBuildCityState.CONTINUECITY;
        SpawnCurrentCity();
        ChangeStateUI();
        PlayerData.Vibrate(HapticTypes.Failure);
    }


    public GameObject GetFlyScrewInPool()
    {
        var objScrew = poolingScrewFlying.FirstOrDefault(x => !x.gameObject.activeInHierarchy);
        if (objScrew == null)
        {
            var obj = Instantiate(prefabScrewFly, poolParent.transform, true);
            poolingScrewFlying.Add(obj);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            objScrew.gameObject.SetActive(true);
            return objScrew;
        }
    }

    public void SpawnCurrentCity()
    {
        foreach (var island in listAllIsland)
        {
            island.gameObject.SetActive(false);
        }

        float delayTime = 0f;
        if (currentIsland != null)
        {
            currentIsland.gameObject.SetActive(true);
            delayTime = defaultTimeEffect;
        }

        int idCity = PlayerData.currentCityBuild;
        if (idCity >= listAllIsland.Count - 1) idCity = listAllIsland.Count - 2; // setComingSoon
        TweenControl.GetInstance().DelayCall(gameObject.transform, delayTime,
            () => { listAllIsland[idCity].gameObject.SetActive(true); });
        currentIsland = listAllIsland[idCity];
        idSelectedIsland = idCity;
        SetStatusAnotherIsLand();
        MoveToCurrentIsland();
    }

    public void DeactiveBuildCity()
    {
        gameObject.SetActive(false);
    }


    public void HiddenNewCityName()
    {
        backButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        nameCityEndingTMP_2.gameObject.SetActive(false);
    }

    #endregion

    #region Init

    public void CreatePool()
    {
        poolingScrewFlying = new List<GameObject>();
        for (int i = 0; i < 50; i++)
        {
            var obj = Instantiate(prefabScrewFly, poolParent.transform, true);
            obj.gameObject.SetActive(false);
            poolingScrewFlying.Add(obj);
        }
    }

    void InitPositionAllIsland()
    {
        for (int i = 0; i < listAllIsland.Count; i++)
        {
            listAllIsland[i].gameObject.transform.parent = poolCity.transform;
            listAllIsland[i].gameObject.transform.localPosition = new Vector3(i * -distanceEachIsland, 0, 0);
        }
    }

    #endregion

    #region UI

    public void HiddenUI()
    {
        switch (currentState)
        {
            case UIBuildCityState.NONE:
                break;
            case UIBuildCityState.BUILDCITY:
                holdToBuild.gameObject.SetActive(true);
                break;
            case UIBuildCityState.COMPLETECITY:
                break;
            case UIBuildCityState.CONTINUECITY:
                cityContinue.gameObject.SetActive(true);
                TweenControl.GetInstance().ScaleFromOne(cityNameContinue.gameObject, defaultTimeEffect);
                TweenControl.GetInstance().ScaleFromOne(continueBtn.gameObject, defaultTimeEffect);
                break;
        }

        ChangeAnimIsland(true);
        HiddenNewCityName();
        MoveToCurrentIsland();
    }


    public void ShowScale()
    {
        currentState = UIBuildCityState.NONE;
        SpawnCurrentCity();
        ChangeStateUI();
        TweenControl.GetInstance().ScaleFromZero(currentIsland.gameObject, 1f);
        this.gameObject.SetActive(true);
    }

    public void Show()
    {
        if (PlayerData.screwCityBuild > 0 && PlayerData.currentCityBuild < listAllIsland.Count - 1)
        {
            //Build scene
            currentState = UIBuildCityState.BUILDCITY;
        }
        else
        {
            //Continue Scebe
            currentState = UIBuildCityState.CONTINUECITY;
        }

        ChangeStateUI();
    }

    public void ChangeStateUI()
    {
        inAction = false;
        holdToBuild.gameObject.SetActive(false);
        cityCompleteReward.gameObject.SetActive(false);
        cityContinue.gameObject.SetActive(false);
        cityReward.gameObject.SetActive(false);
        int idCity = PlayerData.currentCityBuild;
        var dataIsland = Config.buildCityConfig;
        var dataConfig = dataIsland.dataCity.Find(x => x.idCity == idCity);
        if (dataConfig == null) dataConfig = dataIsland.dataCity[dataIsland.dataCity.Count - 1];
        ChangeAnimIsland(false);
        switch (currentState)
        {
            case UIBuildCityState.NONE:
                ChangeAnimIsland(true);
                break;
            case UIBuildCityState.BUILDCITY:
                holdToBuild.gameObject.SetActive(true);
                nameCityTMP.text = dataConfig.cityName;
                currentScrewTMP.text = PlayerData.screwCityBuild.ToString();
                break;
            case UIBuildCityState.COMPLETECITY:
                cityCompleteReward.gameObject.SetActive(true);
                TweenControl.GetInstance().ScaleFromZero(nextRewardBtn.gameObject, defaultTimeEffect);
                TweenControl.GetInstance().ScaleFromZero(titleComplete.gameObject, defaultTimeEffect);
                break;
            case UIBuildCityState.GETREWARD:
                cityReward.gameObject.SetActive(true);
                break;
            case UIBuildCityState.CONTINUECITY:
                cityContinue.gameObject.SetActive(true);
                cityNameContinue.gameObject.SetActive(false);
                backButton.gameObject.SetActive(false);
                nextButton.gameObject.SetActive(false);
                nameCityEndingTMP_2.gameObject.SetActive(false);
                nameCityEndingTMP.text = dataConfig.cityName;
                nameCityEndingTMP_2.text = dataConfig.cityName;
                SetButtonLeftRight();
                nameCityEndingTMP_2.gameObject.SetActive(true);
                TweenControl.GetInstance().ScaleFromZero(continueBtn.gameObject, defaultTimeEffect);

                break;
        }
    }

    public void OnContinueClickBtn()
    {
        if (!inAction)
        {
            inAction = true;
            HiddenUI();
            TweenControl.GetInstance().DelayCall(transform, defaultTimeEffect, () =>
            {
                //Back To Home Action
                //UI.screenHome.OnBackFromCityBuild();
                OnExitBuildCity();
            });
            PlayerData.Vibrate(HapticTypes.Failure);
        }
    }

    #endregion

    #region Ulity

    private BuildCityDataConfig ReturnComingSoonData()
    {
        BuildCityDataConfig result = new BuildCityDataConfig();
        result.cityName = "Coming soon";
        result.idCity = listAllIsland.Count - 1;
        return result;
    }

    Vector3 GetRandomPositionInSphere(Vector3 center, float radius)
    {
        Vector3 randomPointInUnitSphere = UnityEngine.Random.insideUnitSphere;
        Vector3 randomPointInSphere = randomPointInUnitSphere * radius;
        Vector3 randomPosition = center + randomPointInSphere;
        return randomPosition;
    }

    #endregion

    #region SelectedAndNextIsland

    void SetStatusAnotherIsLand()
    {
        foreach (var island in listAllIsland)
        {
            island.gameObject.SetActive(true);
            island.LoadDataIsland(island.idIsland <= currentIsland.idIsland);
        }
    }

    void MoveToCurrentIsland()
    {
        idSelectedIsland = currentIsland.idIsland;
        ValidateIdSelectedIsland();
        if (poolCity != null)
            poolCity.transform.DOMoveX((currentIsland.idIsland) * distanceEachIsland, timeMoveIsland);
    }

    void MoveNextIsland()
    {
        idSelectedIsland++;
        ValidateIdSelectedIsland();
        var data = Config.buildCityConfig.dataCity.Find(x => x.idCity == idSelectedIsland);
        SetDataNameIsland(data);
        SetButtonLeftRight();
        PlayerData.Vibrate(HapticTypes.Failure);
    }

    public void ValidateIdSelectedIsland()
    {
        idSelectedIsland = idSelectedIsland >= listAllIsland.Count ? listAllIsland.Count - 1 : idSelectedIsland;
        idSelectedIsland = idSelectedIsland < 0 ? 0 : idSelectedIsland;
    }

    public void SetButtonLeftRight()
    {
        backButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        if (idSelectedIsland <= 0)
        {
            backButton.gameObject.SetActive(false);
        }

        if (idSelectedIsland >= listAllIsland.Count - 1)
        {
            nextButton.gameObject.SetActive(false);
        }
    }

    private void SetDataNameIsland(BuildCityDataConfig data)
    {
        if (data != null)
        {
            nameCityEndingTMP_2.text = data.cityName;
            poolCity.transform.DOMoveX(idSelectedIsland * distanceEachIsland, timeMoveIsland);
        }
        else
        {
            //SetComingSoon
            nameCityEndingTMP_2.text = ReturnComingSoonData().cityName;
            poolCity.transform.DOMoveX(idSelectedIsland * distanceEachIsland, timeMoveIsland);
        }
    }

    void MoveBackIsland()
    {
        idSelectedIsland--;
        ValidateIdSelectedIsland();
        var data = Config.buildCityConfig.dataCity.Find(x => x.idCity == idSelectedIsland);
        SetDataNameIsland(data);
        SetButtonLeftRight();
        PlayerData.Vibrate(HapticTypes.Failure);
    }

    #endregion
}

public enum UIBuildCityState
{
    NONE,
    BUILDCITY,
    COMPLETECITY,
    GETREWARD,
    CONTINUECITY
}