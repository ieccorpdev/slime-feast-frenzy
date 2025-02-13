using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using IEC;
using IEC.Linq;
using UnityEngine;

public class IslandModelInfo : _BaseBehaviour
{
    public int idIsland;
    // Start is called before the first frame update
    public List<IslandItem> listObjectIsland;
    public int direction = 1;
    private int speedRotate = 2;
    public int blockLeft = 30;
    public int blockRight = 330;
    public int offset = 30;
    public bool isDebug = false;
    public bool rotate = true;
    public int currentModalSelected = -1;
    public int maxScrewCanSpend = -1;
    public GameObject currentSelectedObj;
    public BuildCityDataConfig dataConfig;
    public static string FillAmount = "_FillAmount";
    public List<MeshRenderer> islandMesh;
    public List<SkinnedMeshRenderer> islandSkinMesh;
    private Tween punchTween;

    private int _amountNeedToCompleteIsland => idIsland == 0 ? 150 : (200 + (idIsland) * 150); //idIsland starts from 0
    private List<int> _amountNeedEachObject = new List<int>();
    
    void Start()
    {
        //LoadDataIsland();
    }

    public void StopMovingIsland(bool stt )
    {
        rotate = stt;
        foreach (var islandItem in listObjectIsland)
        {
            islandItem.StopAnimBuilding(stt);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (rotate)
        {
            transform.Rotate(Vector3.up*Time.deltaTime*speedRotate*direction);
            if (transform.eulerAngles.y > blockLeft && transform.eulerAngles.y <(blockLeft+offset) && direction > 0)
            {
                direction = -1;
            }

            if (transform.eulerAngles.y <blockRight &&  transform.eulerAngles.y >(blockRight-offset)  && direction < 0)
            {
                direction = 1;
            }
        }
    }

    public void LoadDataIsland(bool islandUnlock = true)
    {
        //Debug.LogError("LoadDataIsland");

        if (listObjectIsland.Count > 0)
        {
            _amountNeedEachObject.Clear();
            _amountNeedEachObject = SplitNumberToEvenList(_amountNeedToCompleteIsland, listObjectIsland.Count);
        }

        var dataIsland = Config.buildCityConfig;
        dataConfig = dataIsland.dataCity.Find(x => x.idCity == idIsland);
        List<float> listPercentage = new List<float>();
        for (int i = 0; i < listObjectIsland.Count; i++)
        {
            var percentageComplete = PercentageComplete(i);
            listObjectIsland[i].SetFloatMaterialItemIsland(isDebug ?1 : percentageComplete);
            if (percentageComplete >= 1)
            {
                listObjectIsland[i].panelInfoItem.gameObject.SetActive(false);
            }
            else
            {
                listObjectIsland[i].panelInfoItem.gameObject.SetActive(true);
                listObjectIsland[i].panelInfoItem.UpdateNumberScrewNeed(NumberScrewNeedComplete(i));
            }
            listPercentage.Add(percentageComplete);
        }

        SetIslandActiveMaterial(islandUnlock);
        bool selected = false;
        for (int i = 0; i < listObjectIsland.Count; i++)
        {
            if (!islandUnlock)
            {
                listObjectIsland[i].gameObject.SetActive(false); 
                continue;
            }

            if (!selected)
            {
                if ((i + 1) < listObjectIsland.Count)
                {
                    if (listPercentage[i + 1] == 0 && listPercentage[i]<1)
                    {
                        SelectedModelBuild(i);
                        selected = true;
                    }
                }
                else
                {
                    //last
                    SelectedModelBuild(i);
                }
            }

            if (islandUnlock)
            {
                listObjectIsland[i].gameObject.SetActive(isDebug || i<=currentModalSelected || listPercentage[i] >0);
            }
            else
            {
                listObjectIsland[i].gameObject.SetActive(false);
            }
        }
        Locator.Main.HomeScreenTab.buildCityUI.canSpendScrew = true;
    }

    void SetIslandActiveMaterial(bool active)
    {
        foreach (var mrd in islandMesh)
        {
            mrd.sharedMaterial = active ? idIsland <=2 ? Config.buildCityConfig.islandActive :  Config.buildCityConfig.islandActive_2 : Config.buildCityConfig.islandOff;
        }      
        foreach (var mrd in islandSkinMesh)
        {
            mrd.sharedMaterial = active ? idIsland <=2 ? Config.buildCityConfig.islandActive :  Config.buildCityConfig.islandActive_2  :  Config.buildCityConfig.islandOff;
        }
    }
    private void SelectedModelBuild(int i)
    {
        currentModalSelected = i;
        currentSelectedObj = listObjectIsland[i].gameObject;
        //maxScrewCanSpend = dataConfig.numberScrewNeedUnlockItem[i] - PlayerData.buildCityData[idIsland].itemCity[i];
        maxScrewCanSpend = NumberScrewNeedComplete(i);
    }

    private float PercentageComplete(int indexModel)
    {
        int NumScrew = PlayerData.buildCityData[idIsland].itemCity[indexModel];
        //int targetScrew = dataConfig.numberScrewNeedUnlockItem[indexModel];
        int targetScrew = _amountNeedEachObject[indexModel];
        float percentageComplete = (float) NumScrew / (float) targetScrew;
        return percentageComplete;
    }
    private int NumberScrewNeedComplete(int indexModel)
    {
        int NumScrew = PlayerData.buildCityData[idIsland].itemCity[indexModel];
        //int targetScrew = dataConfig.numberScrewNeedUnlockItem[indexModel];
        int targetScrew = _amountNeedEachObject[indexModel];
        return targetScrew-NumScrew;
    }
    void StartPunchScaleLoop(GameObject obj)
    {
        if (punchTween == null || !punchTween.IsPlaying())
        {
            Vector3 currentScale = obj.transform.localScale;
            //punchTween = obj.gameObject.transform.DOPunchScale((Vector3.down + Vector3.left)*0.05f, 0.1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo); // Loop infinitely with a Yoyo type
            punchTween = obj.gameObject.transform.DOPunchScale(Vector3.one *0.1f, 0.15f).SetEase(Ease.InBounce); // Loop infinitely with a Yoyo type
        }
    }
    public void AddScrewToCurrentObject(bool lastScrew)
    {
        StartPunchScaleLoop(listObjectIsland[currentModalSelected].gameObject);
        listObjectIsland[currentModalSelected].SetFloatMaterialItemIsland(isDebug ?1 : PercentageComplete(currentModalSelected));
        listObjectIsland[currentModalSelected].panelInfoItem.UpdateNumberScrewNeed(NumberScrewNeedComplete(currentModalSelected));
        if (lastScrew)
        {
            listObjectIsland[currentModalSelected].panelInfoItem.gameObject.SetActive(false);
            listObjectIsland[currentModalSelected].BuildingCompleteEffect();
            CompleteItem();
        }
    }

    public void CompleteItem()
    {
        //Effect
     if (currentModalSelected == listObjectIsland.Count-1)
     {
         //CompleteIsland
         TweenControl.GetInstance().DelayCall(transform, 1.0f, () =>
         {
             Locator.Main.HomeScreenTab.buildCityUI.ShowRewardCity();
         });

     }
     else
     {
         LoadDataIsland();
         listObjectIsland[currentModalSelected].gameObject.SetActive(false);
         Locator.Main.HomeScreenTab.buildCityUI.canSpendScrew = false;
         Vector3 targetScale = listObjectIsland[currentModalSelected].gameObject.transform.localScale;
         listObjectIsland[currentModalSelected].gameObject.transform.localScale = Vector3.zero;
         listObjectIsland[currentModalSelected].gameObject.SetActive(true);
         TweenControl.GetInstance().Scale(listObjectIsland[currentModalSelected].gameObject,targetScale,0.5f, () =>
         {
             if (currentModalSelected >= listObjectIsland.Count)
             {
                 //NextIsland
                 Locator.Main.HomeScreenTab.buildCityUI.ShowRewardCity();
             }
             else
             {
                 Locator.Main.HomeScreenTab.buildCityUI.canSpendScrew = true;
             }
         });
     }
    }
    
    private List<int> SplitNumberToEvenList(int totalAmount, int objectCount)
    {
        List<int> result = new List<int>(objectCount);
        if(totalAmount == 0)
            return result;

        // Integer division spelled in JavaScript
        var q = Mathf.FloorToInt(totalAmount / objectCount);
        var r = totalAmount % objectCount;
        var c = objectCount - r;
            
        var a = 1;
        var b = 1;
  
        for(int i = 0; i < objectCount; i++)
        {
            // Make the ratio a/b tend towards c/r
            if((a * r) < (b * c))
            {
                a++;
                result.Add(q);
            }
            else
            {
                b++;
                result.Add(q + 1);
            }
        }

        return result;
    }
}
