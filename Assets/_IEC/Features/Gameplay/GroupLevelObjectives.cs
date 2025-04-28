using System;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using DG.Tweening;
using IEC;
using TMPro;
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
        public GameObject specialObjectiveParent;
        public TextMeshProUGUI goldTextTutorial;
        private bool canPlaySFX = true;

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

        public void CollectAObject(string itemId, Vector2 startAnchorPosition)
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
            
            Vector3 startPos = objectiveVfx.transform.position;
            Vector3 endPos = objectiveUI.GetIconPosition();
            Vector3 midPos = (startPos + endPos) / 2 + new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f),0 , 0); // Add a Y offset to create a curve
            if (!GamePlay.IsWin)
            {
                if (canPlaySFX)
                {
                    canPlaySFX = false;
                    SoundController.PlaySound(EventList.Audio.OBJECTIVE_GET);
                    DOVirtual.DelayedCall(0.3f, () => canPlaySFX = true);
                }
            }
            DOTween.Sequence()
                .Append(objectiveVfx.GetRectTransform().DOPath(new Vector3[] { startPos, midPos, endPos }, 1f, PathType.CatmullRom))
                .Join(objectiveVfx.GetRectTransform().DOScale(objectiveUI.GetIconSize().x / objectiveVfx.GetRectTransform().sizeDelta.x * Vector3.one, 1f))
                .AppendCallback(() =>
                {
                    objectiveUI.SetNumber(newNumber.ToString(), isCompleted);
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
            int maxSpecialObject = GamePlay.levelController.AllSpecialItems.Count;

            //objectiveUI.SetNumber(maxSpecialObject, false);
            objectiveUI.SetNumber(number+"/"+maxSpecialObject, false);
        }
        
        public void DisposeAll()
        {
            m_ActiveObjectiveUIs.Clear();
            foreach (var levelObjectiveUi in levelObjectiveUis)
            {
                levelObjectiveUi.gameObject.SetActive(false);
                levelObjectiveUi.Reset();
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
            HandleObjectiveUIForSpecialObject();
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
        
        private void HandleObjectiveUIForSpecialObject()
        {
            var dataSpecial = Config.specialObjectConfig.GetSpecialItemDefinition(PlayerData.currentSpecialItemIndex);
            var specialObjective = new LevelObjective();
            int count = GamePlay.levelController.AllSpecialItems.Count;
            specialObjective.itemId = dataSpecial.GetId();
            specialObjective.number = count;
            specialObjectiveUI.LoadData(specialObjective,true);
            m_ActiveObjectiveUIs.Add(specialObjectiveUI);
            specialObjectiveUI.gameObject.SetActive(count > 0);
            specialObjectiveParent.gameObject.SetActive(count > 0);
            //specialObjectiveUI.SetNumber(0, false);
        }

        private void HideSpecialObjective()
        {
            specialObjectiveUI.gameObject.SetActive(false);
        }
    }
}