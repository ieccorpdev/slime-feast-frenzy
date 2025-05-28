using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay.ObserverEventData;
using DG.Tweening;
using IEC;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _IEC.Features.Gameplay
{
    public class GroupLevelObjectives : _BaseBehaviour
    {
        public List<LevelObjectiveUI> levelObjectiveUis;
        [SerializeField] private Transform holder;
        [SerializeField] private Transform vfxHolder;
        private readonly List<LevelObjectiveUI> m_ActiveObjectiveUIs = new List<LevelObjectiveUI>();
        [SerializeField] private List<LevelObjectiveUI> specialObjectiveUI;
        public GameObject specialObjectiveParent;
        public TextMeshProUGUI goldTextTutorial;
        private bool canPlaySFX = true;
        
        private List<Sequence> sequences = new List<Sequence>();
        private List<LevelObjectiveVfx> listObjectiveVfx = new List<LevelObjectiveVfx>();

        public LevelObjectiveUI specialBottleUI;

        // private void Awake()
        // {
        //     this.RegisterListener(EventList.GamePlay.RESUME_GAME, OnGameResume);
        // }
        //
        // private void OnDestroy()
        // {
        //     this.UnregisterListener(EventList.GamePlay.RESUME_GAME, OnGameResume);
        // }
        // void OnGameResume()
        // {
        //     if (Pool.Get<LevelController>().GetLevelData().levelType == LevelType.Coin) return;
        //     foreach (var objectiveUI in levelObjectiveUis)
        //     {
        //         if(objectiveUI == null || !objectiveUI.gameObject.activeInHierarchy) continue;
        //         var objectiveData = Pool.Get<LevelController>().GetLevelData().levelObjectives.Find(x => x.itemId == objectiveUI.itemId);
        //         if (objectiveData == null)
        //         {
        //             Debug.LogError($"Missing objective??? {objectiveUI.itemId}");
        //             continue;
        //         }
        //         
        //         if(objectiveUI.IsComplete() || objectiveUI.CurrentNumber == objectiveData.number) continue;
        //         objectiveUI.SetNumber(objectiveData.number);
        //     }
        // }

        public void CollectAObject(CollectItemData itemData, Vector2 startAnchorPosition,bool canPlaySFX)
        {
            var objectiveUI = GetLevelObjectiveUI(itemData);
            //if (objectiveUI.IsComplete()) return;
            //cache the value to make sure UI is updated correctly after delaying
            bool isCompleted = IsObjectiveCompleted(itemData.itemID);
            int newNumber = GetRemainingObjectiveNumber(itemData.itemID);
            EffectSpawnObjectTive(itemData, startAnchorPosition, objectiveUI, () =>
            {
                objectiveUI.SetNumber(newNumber.ToString(), isCompleted);
            },true);
        }

        private void EffectSpawnObjectTive(CollectItemData itemData, Vector2 startAnchorPosition, LevelObjectiveUI objectiveUI,Action flyComplete, bool playSFX)
        {
            var objectiveVfx = Config.prefabConfig.levelObjectiveVfx.Spawn(vfxHolder);
            objectiveVfx.Reset();
            objectiveVfx.SetAnchorPosition(startAnchorPosition);
            objectiveVfx.SetSprite(objectiveUI.GetSprite());
            objectiveVfx.gameObject.SetActive(true);
            listObjectiveVfx.Add(objectiveVfx);
            Vector3 startPos = objectiveVfx.transform.position;
            Vector3 endPos = objectiveUI.GetIconPosition();
            Vector3 midPos = (startPos + endPos) / 2 + new Vector3(objectiveUI.GetOffsetItemCurve(), 0, 0); // Add a Y offset to create a curve
            if (!GamePlay.IsWin)
            {
                if (canPlaySFX && playSFX)
                {
                    canPlaySFX = false;
                    SoundController.PlaySound(EventList.Audio.OBJECTIVE_GET);
                    DOVirtual.DelayedCall(SoundController.GetLengthSound(EventList.Audio.OBJECTIVE_GET),
                        () => canPlaySFX = true);
                }
            }

            Sequence temp = DOTween.Sequence();
            temp.Append(objectiveVfx.GetRectTransform()
                    .DOPath(new Vector3[] {startPos, midPos, endPos}, 1f, PathType.CatmullRom))
                .Join(objectiveVfx.GetRectTransform()
                    .DOScale(objectiveUI.GetIconSize().x / objectiveVfx.GetRectTransform().sizeDelta.x * Vector3.one, 1f))
                .AppendCallback(() =>
                {
                    flyComplete?.Invoke();
                    objectiveVfx.Recycle();
                });

            sequences.Add(temp);
        }

        private bool IsObjectiveCompleted(string itemId)
        {
            var levelObjective = Pool.Get<LevelController>().GetLevelData().levelObjectives.Find(x => x.itemId == itemId);
            if (levelObjective == null) return true;
            return levelObjective.number <= 0;
        }
        
        private int GetRemainingObjectiveNumber(string itemId)
        {
            var levelObjective = Pool.Get<LevelController>().GetLevelData().levelObjectives.Find(x => x.itemId == itemId);
            if (levelObjective == null) return 0;

            return levelObjective.number;
        }
        
        public void CollectASpecialObject(CollectItemData itemData,int number,Vector2 startAnchorPosition)
        {
            var objectiveUI = GetLevelObjectiveUI(itemData);
            if (objectiveUI == null) return;
            if(itemData.GetCoinValue() > 0){
                EffectSpawnObjectTive(itemData, startAnchorPosition, objectiveUI, () =>
                {
                    objectiveUI.SetNumber(GamePlay.levelController.GoldCollected.ToString(),false);
                },true);
            }
            else if (itemData.itemID.Equals(EventList.GamePlay.Kingdom_Special))
            {
                objectiveUI.PlayAnim();
                EffectSpawnObjectTive(itemData, startAnchorPosition, objectiveUI, () =>
                {
                    objectiveUI.SetNumber("", false);
                }, true);
                //objectiveUI.PlayAnim();
            }
            else
            {
                int maxSpecialObject = GamePlay.levelController.AllSpecialItems.Count;
                EffectSpawnObjectTive(itemData, startAnchorPosition, objectiveUI, () =>
                {
                    objectiveUI.SetNumber(number + "/" + maxSpecialObject, false);
                }, true);
                //objectiveUI.SetNumber(number+"/"+maxSpecialObject, false);
            }

        }
        public void CollectARelicObject(CollectItemData itemData,int number,Vector2 startAnchorPosition)
        {
            var objectiveUI = GetLevelObjectiveUI(itemData);
            if (objectiveUI == null) return;
            int maxSpecialObject = GamePlay.levelController.AllRelicItems.Count;
            EffectSpawnObjectTive(itemData, startAnchorPosition, objectiveUI, () =>
            {
                objectiveUI.SetNumber(number+"/"+maxSpecialObject, false);
            },true);

        }
        
        public void DisposeAll()
        {
            foreach (var sequence in sequences)
            {
                if(sequence.IsActive())
                    sequence.Kill();
            }
            sequences.Clear();
            
            m_ActiveObjectiveUIs.Clear();

            foreach (var obj in listObjectiveVfx)
            {
                if (obj != null && obj.GetRectTransform() != null)
                {
                    if(DOTween.IsTweening(obj))
                        DOTween.Kill(obj);
                    
                    obj.Recycle();
                }
            }
            
            listObjectiveVfx.Clear();
            
            foreach (var levelObjectiveUi in levelObjectiveUis)
            {
                levelObjectiveUi.gameObject.SetActive(false);
                levelObjectiveUi.Reset();
            }
        }
        

        public LevelObjectiveUI GetLevelObjectiveUI(CollectItemData itemData)
        {
            if (itemData.GetCoinValue() > 0)
            {
                return m_ActiveObjectiveUIs.Find(x => x.itemId == Config.specialObjectConfig.goldSpecialObjective.GetId());
            }
            if (itemData.itemID.Equals(EventList.GamePlay.Kingdom_Special))
            {
                return specialBottleUI;
            }
            return m_ActiveObjectiveUIs.Find(x => x.itemId.ToLower().Equals(itemData.itemID.ToLower()));
        }
        
        public void LoadObjectiveData(List<LevelObjective> levelObjectives,LevelType levelType)
        {
            m_ActiveObjectiveUIs.Clear();
            for (int i = 0; i < levelObjectives.Count; i ++)
            {
                var levelObjective = levelObjectives[i];
                var levelObjectiveUIObject = levelObjectiveUis[i].gameObject;
                if (levelType == LevelType.COIN)
                {
                    levelObjectiveUIObject.SetActive(false);
                    continue;
                }
                levelObjectiveUIObject.SetActive(true);
                var levelObjectiveUI = levelObjectiveUIObject.GetComponent<LevelObjectiveUI>();
                levelObjectiveUI.Reset();
                levelObjectiveUI.LoadData(levelObjective,levelObjective.itemId.ToLower().Contains("coin"));
                //levelObjectiveUis.Add(levelObjectiveUI);
                levelObjectiveUI.transform.parent = holder;
                var rectTransform = levelObjectiveUI.GetComponent<RectTransform>();
                var currentAnchorPos3D = rectTransform.anchoredPosition3D;
                currentAnchorPos3D.z = 0;
                rectTransform.localScale = Vector3.one;
                m_ActiveObjectiveUIs.Add(levelObjectiveUI);
                rectTransform.anchoredPosition3D = currentAnchorPos3D;
            }
            for (int i = 0; i < specialObjectiveUI.Count; i ++)
            {
                specialObjectiveUI[i].gameObject.SetActive(false);
                specialObjectiveUI[i].Reset();
            }
            HandleObjectiveUIForSpecialObject(levelType);
            goldTextTutorial.gameObject.SetActive(levelType == LevelType.COIN);
            //if (levelType == LevelType.COIN)
            //{
            //    HandleObjectiveUIForSpecialObject();
            //}
            //else
            //{
            //    HideSpecialObjective();
            //}
        }

        public LevelObjectiveUI GetSpecialObjectiveEnable()
        {
            var result = specialObjectiveUI.Find(x => !x.IsLoaded);
            result.gameObject.SetActive(true);
            return result;
        }
        private void HandleObjectiveUIForSpecialObject(LevelType levelType)
        {
            HandleGoldSpecialObjective(levelType);
            HandleOceanGiftObject();
            HandleKingdomSpecialObjective();
            HandleRelicObject();
            specialObjectiveParent.gameObject.SetActive(specialObjectiveUI.Find(x=>x.gameObject.activeSelf) !=null);
            StartCoroutine(RebuildNextFrame());
        }

        public void HandleKingdomSpecialObjective()
        {
            var dataKingdom = Config.specialObjectConfig.kingdomSpecialObjective;
            var kingdomObjective = new LevelObjective();
            int kingdomCount = GamePlay.levelController.AllkingdomSpecialItems.Count;
            kingdomObjective.itemId = dataKingdom.GetId();
            kingdomObjective.number = kingdomCount;
            LevelObjectiveUI kingdomObjectiveContain = specialBottleUI;
            kingdomObjectiveContain.LoadData(kingdomObjective, true);
            m_ActiveObjectiveUIs.Add(kingdomObjectiveContain);
            kingdomObjectiveContain.gameObject.SetActive(kingdomCount > 0);
        }

        IEnumerator RebuildNextFrame()
        {
            yield return null; // wait 1 frame
            LayoutRebuilder.ForceRebuildLayoutImmediate(specialObjectiveParent.GetComponent<RectTransform>());
        }

        public void HandleGoldSpecialObjective(LevelType levelType)
        {
            if (levelType == LevelType.COIN)
            {
                var dataSpecial = Config.specialObjectConfig.goldSpecialObjective;
                var specialObjective = new LevelObjective();
                int count = 0;
                specialObjective.itemId = dataSpecial.GetId();
                specialObjective.number = count;
                LevelObjectiveUI objectiveContain = GetSpecialObjectiveEnable();
                objectiveContain.LoadData(specialObjective,true);
                m_ActiveObjectiveUIs.Add(objectiveContain);
            }
        }
        public void HandleOceanGiftObject()
        {
            var dataSpecial = Config.specialObjectConfig.GetSpecialItemDefinition(PlayerData.currentSpecialItemIndex);
            var specialObjective = new LevelObjective();
            int count = GamePlay.levelController.AllSpecialItems.Count;
            specialObjective.itemId = dataSpecial.GetId();
            specialObjective.number = count;
            LevelObjectiveUI objectiveContain = GetSpecialObjectiveEnable();
            objectiveContain.LoadData(specialObjective,true);
            m_ActiveObjectiveUIs.Add(objectiveContain);
            objectiveContain.gameObject.SetActive(count > 0);
        }
        public void HandleRelicObject()
        {
            var dataSpecial = Config.relicObjectConfig.GetRelicItemDefinition(PlayerData.relicData.relicItemIndex);
            var relicObjective = new LevelObjective();
            int count = GamePlay.levelController.AllRelicItems.Count;
            relicObjective.itemId = dataSpecial.GetId();
            relicObjective.number = count;
            LevelObjectiveUI objectiveContain = GetSpecialObjectiveEnable();
            objectiveContain.LoadDataRelic(relicObjective,true);
            m_ActiveObjectiveUIs.Add(objectiveContain);
            objectiveContain.gameObject.SetActive(count > 0);
        }
        private void HideSpecialObjective()
        {
            foreach (var objSpecial in specialObjectiveUI)
            {
                objSpecial.gameObject.SetActive(false);
            }
        }
    }
}