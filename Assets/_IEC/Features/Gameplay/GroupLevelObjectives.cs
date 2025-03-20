using System;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using DG.Tweening;
using IEC;
using UnityEngine;
using UnityEngine.Serialization;

namespace _IEC.Features.Gameplay
{
    public class GroupLevelObjectives : _BaseBehaviour
    {
        public List<LevelObjectiveUI> levelObjectiveUis;
        [SerializeField] private Transform holder;
        [SerializeField] private Transform vfxHolder;
        private readonly List<LevelObjectiveUI> m_ActiveObjectiveUIs = new List<LevelObjectiveUI>();
        [SerializeField] private LevelObjectiveUI specialObjectiveUI;

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
        
        public void CollectAObject(string itemId,Vector2 startAnchorPosition)
        {
            var objectiveUI = GetLevelObjectiveUI(itemId);
            //if (objectiveUI.IsComplete()) return;
            var objectiveVfx = Config.prefabConfig.levelObjectiveVfx.Spawn(vfxHolder);
            objectiveVfx.Reset();
            objectiveVfx.SetAnchorPosition(startAnchorPosition);
            objectiveVfx.SetSprite(objectiveUI.GetSprite());
            objectiveVfx.gameObject.SetActive(true);
            
            //cache the value to make sure UI is updated correctly after delaying
            bool isCompleted = IsObjectiveCompleted(itemId);
            int newNumber = GetRemainingObjectiveNumber(itemId);
            
            DOTween.Sequence()
                .Append(objectiveVfx.GetRectTransform().DOMove(objectiveUI.GetIconPosition(), 1f))
                .Join(objectiveVfx.GetRectTransform().DOScale(objectiveUI.GetIconSize().x / objectiveVfx.GetRectTransform().sizeDelta.x * Vector3.one, 1f))
                .AppendCallback(() =>
                {
                    objectiveUI.SetNumber(newNumber, isCompleted);
                    objectiveVfx.Recycle();
                });
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
        
        public void CollectASpecialObject(string itemId,int number,Vector2 startAnchorPosition)
        {
            var objectiveUI = GetLevelObjectiveUI(itemId);
            if (objectiveUI == null) return;
            objectiveUI.SetNumber(number, false);
        }
        
        public void DisposeAll()
        {
            m_ActiveObjectiveUIs.Clear();
            foreach (var levelObjectiveUi in levelObjectiveUis)
            {
                levelObjectiveUi.gameObject.SetActive(false);
            }
        }
        

        public LevelObjectiveUI GetLevelObjectiveUI(string itemId)
        {
            return m_ActiveObjectiveUIs.Find(x => x.itemId.ToLower().Equals(itemId.ToLower()));
        }
        
        public void LoadObjectiveData(List<LevelObjective> levelObjectives,LevelType levelType)
        {
            m_ActiveObjectiveUIs.Clear();
            for (int i = 0; i < levelObjectives.Count; i ++)
            {
                var levelObjective = levelObjectives[i];
                var levelObjectiveUIObject = levelObjectiveUis[i].gameObject;
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

            if (levelType != LevelType.COIN)
                //&& PlayerData.level >= Config.treasureHuntProgressConfig.unlockLevel 
                //&& Game.enableTreasureHunt)
                //&& TreasureHuntController.treasureHuntData.currentProgressIndex < Config.treasureHuntProgressConfig.rewardProgressModels.Count)
            {
                HandleObjectiveUIForSpecialObject();
            }
            else
            {
                HideSpecialObjective();
            }
        }
        
        private void HandleObjectiveUIForSpecialObject()
        {
            //var treasureConfig = Config.treasureHuntProgressConfig;
            //var specialObjective = new LevelObjective();
            //specialObjective.itemId = ProgressRewardHelper.GetCurrentSpecialObjectIdInString();
            //specialObjective.number = 0;
            //specialObjectiveUI.LoadData(specialObjective,true);
            //specialObjectiveUI.gameObject.SetActive(treasureConfig.IsUnlocked(PlayerData.level));
            m_ActiveObjectiveUIs.Add(specialObjectiveUI);
            specialObjectiveUI.gameObject.SetActive(true);
        }

        private void HideSpecialObjective()
        {
            specialObjectiveUI.gameObject.SetActive(false);
        }
    }
}