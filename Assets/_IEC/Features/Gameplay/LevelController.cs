using System;
using System.Collections.Generic;
using System.Linq;
using _IEC.Features.Booster;
using _IEC.Features.Common;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay.ObserverEventData;
using DG.Tweening;
using IEC;
using UnityEngine;
using BoosterData = IEC.BoosterData;
using Game = IEC.Game;
using Random = UnityEngine.Random;

namespace _IEC.Features.Gameplay
{
    public class EventLevelLoad
    {
        public string themeId;
        public List<BaseItem> items;

        public EventLevelLoad(List<BaseItem> items, string themeId)
        {
            this.themeId = themeId;
            this.items = items;
        }
    }

    public class LevelController : _BaseBehaviour, ILevelController, IGroup
    {
        [SerializeField] private Hole playerHole;
        [SerializeField] private Transform levelObjectHolder;
        [SerializeField] private Transform allItemHolder;


        private bool _isShowEnding = false;
        private State m_CurrentState;
        private LevelData m_CurrentLevelData;
        private float m_MagnetHelperCoolDown;
        private float m_ScaleUpHelperCoolDown;
        private bool m_ScaleUpHelperActive;
        private bool m_MagnetHelperActive;
        private bool m_IsGameplayPause;
        private int m_Level;
        private List<BaseItem> m_ObjectiveItems = new List<BaseItem>();
        private List<BaseItem> m_AllItems = new List<BaseItem>();
        private List<BaseItem> m_AllSpecialItems = new List<BaseItem>();
        private List<BombItem> m_AllBombs = new List<BombItem>();
        private List<BaseItem> m_AllBoosters = new List<BaseItem>();
        //private List<SpecialObject> m_AllSpecialObjects = new List<SpecialObject>();

        private Action addBoosterAction;
        public State CurrentState => m_CurrentState;

        public bool IsShowEnding
        {
            get => _isShowEnding;
            set => _isShowEnding = value;
        }

        public int SpecialProgressRewardObjCollected { get; private set; } = 0;

        private void OnEnable()
        {
            this.RegisterListener(EventList.GamePlay.TIMER_TIME_OUT, OnLevelTimeOut);
            this.RegisterListener(EventList.GamePlay.COLLECT_ITEM, OnCollectItem);
            this.RegisterListener(EventList.GamePlay.USER_USE_HELPER, OnUserUseHelper);
            this.RegisterListener(EventList.GamePlay.PAUSE_GAME, OnPauseGame);
            this.RegisterListener(EventList.GamePlay.RESUME_GAME, OnResumeGame);
            this.RegisterListener(EventList.GamePlay.REVIVE_GAME, ReviveGame);
            this.RegisterListener(EventList.GamePlay.USE_BOOSTER, OnUseBooster);
            this.RegisterListener(EventList.GamePlay.ON_BOOSTER_COLLECTED, OnCollectedBoosterItem);
            this.RegisterListener(EventList.GamePlay.SPAWN_BOOSTER_ITEM, OnSpawnBoosterItems);
        }

        private void OnDisable()
        {
            this.UnregisterListener(EventList.GamePlay.TIMER_TIME_OUT, OnLevelTimeOut);
            this.UnregisterListener(EventList.GamePlay.COLLECT_ITEM, OnCollectItem);
            this.UnregisterListener(EventList.GamePlay.USER_USE_HELPER, OnUserUseHelper);
            this.UnregisterListener(EventList.GamePlay.PAUSE_GAME, OnPauseGame);
            this.UnregisterListener(EventList.GamePlay.RESUME_GAME, OnResumeGame);
            this.UnregisterListener(EventList.GamePlay.ADD_TIME_FOR_TIMER, ReviveGame);
            this.UnregisterListener(EventList.GamePlay.USE_BOOSTER, OnUseBooster);
            this.UnregisterListener(EventList.GamePlay.ON_BOOSTER_COLLECTED, OnCollectedBoosterItem);
            this.UnregisterListener(EventList.GamePlay.SPAWN_BOOSTER_ITEM, OnSpawnBoosterItems);
        }

        private void Update()
        {
            UpdateCurrentState();
        }

        public void Dispose()
        {
            RecycleAllLevelObjects();
            ChangeState(State.None);
        }

        public void LoadLevel(int level)
        {
            m_Level = level;
            int index = Mathf.Max(0, level - 1);
            var m_LevelConfig = PlayableController.Instance.GetLevelSpawnData(index);

            m_AllItems.Clear();
            var levelSpawnData = m_LevelConfig.levelSpawnData;
            m_AllItems = LevelHelper.SpawnAllItem(levelSpawnData.listItemSpawns, allItemHolder, Game.itemVersion);
            if (!Game.isUsingGroundExtend)
            {
                LevelHelper.SpawnMap(m_LevelConfig, levelObjectHolder);
            }

            m_AllBombs = LevelHelper.SpawnAllBomb(m_LevelConfig, m_AllItems, allItemHolder);
            m_CurrentLevelData = LevelHelper.CreateLevelData(m_LevelConfig, level);

            SetUpStartHolePosition(m_LevelConfig.mapCenter);
            SearchAllObjectiveItems(m_AllItems);
            Reset();
            playerHole.HideLevelText();
            var themeId = m_LevelConfig.theme;
            this.RaiseEvent(EventList.GamePlay.ON_LEVEL_LOADED, new EventLevelLoad(m_AllItems, themeId));
        }

        public void StartPlay()
        {
            allItemHolder.gameObject.SetActive(true);
            playerHole.OnNewGame();
            ChangeState(State.Play);


            if (addBoosterAction != null)
            {
                addBoosterAction.Invoke();
                addBoosterAction = null;
            }

            //Check spawn booster items
            OnSpawnBoosterItems(null);
        }

        private void Reset()
        {
            _isShowEnding = false;
            m_ScaleUpHelperActive = false;
            m_ScaleUpHelperCoolDown = 0;
            m_MagnetHelperActive = false;
            m_MagnetHelperCoolDown = 0;
            playerHole.StopScaleByHelper();
            playerHole.StopMagnetHelper();
            playerHole.Reset();
            SpecialProgressRewardObjCollected = 0;
        }

        private void SearchAllObjectiveItems(List<BaseItem> allItems)
        {
            m_ObjectiveItems.Clear();
            m_ObjectiveItems = allItems.Where(x =>
                    m_CurrentLevelData
                        .levelObjectives
                        .Exists(objective => objective.itemId == x.GetItemId()))
                .ToList();
        }

        public LevelData GetLevelData()
        {
            return m_CurrentLevelData;
        }

        public Hole GetHolePlayer() => playerHole;

        public List<BaseItem> GetAllItems() => m_AllItems
            .Concat(m_AllSpecialItems)
            .Concat(m_AllBoosters).ToList();

        public Transform GetLevelObjectHolder() => levelObjectHolder;
        public Transform GetAllItemHolder() => allItemHolder;

        private void OnCollectItem(object param)
        {
            var data = (CollectItemData)param;
            if (data == null) return;
            var itemId = data.itemID;


            //var timeDelay = Config.soundConfig.delayInterval;


            if (IsObjectiveItem(itemId))
            {
                CollectObjectiveItem(data);
                //SoundController.PlaySoundWithInterval(new AudioInfo(Config.soundConfig.objectiveConsume, false), timeDelay);
            }
            else
            {
                    CollectNormalItem(data);
                    //SoundController.PlaySoundWithInterval(new AudioInfo(Config.soundConfig.normalConsume, false), timeDelay);
            }
            //VibrationController.VibrateWithInterval(HapticTypes.Selection);
        }

        private bool IsSpecialObject(string itemId)
        {
            //return itemId.Contains("SpecialItem");
            return m_AllSpecialItems.Find(item => item.ItemDefinition.GetId().Equals(itemId));
        }

        private void CollectSpecialObject(CollectItemData data)
        {
            CollectNormalItem(data);
            if (data.itemID.Contains("Ocean"))
            {
                SpecialProgressRewardObjCollected++;
            }

            //GamePlay.gamePlayData.AddAchievementItem(data.itemID,data.xp);
            this.RaiseEvent(EventList.GamePlay.COLLECT_SPECIAL_ITEM_FOR_UI, data);
        }

        private void OnLevelTimeOut(object param)
        {
            // if (GetLevelData().levelType == LevelType.COIN)
            // {
            //     ChangeState(State.Win);
            //     EventController.RaiseEvent(EventList.GamePlay.LEVEL_WIN,new EventParam(){p0 = LevelType.COIN});
            //     return;
            // }
            //
            // if (IsInResultState() == false)
            // {
            //     ChangeState(State.Lose);
            //     EventController.RaiseEvent(EventList.GamePlay.LEVEL_LOSE,new EventParam(){p0 = LoseReason.TimeUp});
            // }
            PlayableController.Instance.ShowEndCard();

        }

        private void ReviveGame(object param)
        {
            playerHole.gameObject.SetActive(true);
            playerHole.ResetSize();
            //UI.ScreenInGame.UpdateUI();
            if (IsLevelComplete())
            {
                EventController.RaiseEvent(EventList.GamePlay.LEVEL_WIN, new EventParam() { p0 = LevelType.NORMAL });
                ChangeState(State.Win);
                return;
            }

            this.RaiseEvent(EventList.GamePlay.RESUME_GAME);
            ChangeState(State.Play);
        }

        private void OnUseBooster(object param)
        {
            var data = (UseBoosterEventData)param;
            if (data.boosterId != BoosterId.BonusExp) return;
            //var bonusExpConfig = Config.boosterConfig.GetBoosterConfigModelById(BoosterId.BonusExp);
            //playerHole.AddXp(bonusExpConfig.advantageNumber);
        }

        private void OnCollectedBoosterItem(object param)
        {
            // var data = (MagicPouchBooster)param;
            // if (data.type != MagicPouchBoosterType.Lightning) return;
            var data = (BoosterData)param;
            addBoosterAction +=
                () =>
                {
                    if (data.boosterType == ResourceType.EvolveStone)
                    {
                        playerHole.AddXp(Mathf.FloorToInt(data.effectValue));
                    }
                    else if (data.boosterType == ResourceType.WindSurf)
                    {
                        playerHole.OnCollectSpeedBooster(data.effectValue, data.effectTime);
                    }
                };

            if (CurrentState == State.Play)
            {
                addBoosterAction.Invoke();
                addBoosterAction = null;
            }
            //SoundController.PlaySound(new AudioInfo(Config.soundConfig.boosterLightning));
        }

        private void OnSpawnBoosterItems(object param)
        {
            //var data = (SpawnBoosterEventData)param;

            m_AllBoosters.Clear();
            // m_AllBoosters.AddRange(SpawnBoosterItem(data, MagicPouchBoosterType.Lightning));
            // m_AllBoosters.AddRange(SpawnBoosterItem(data, MagicPouchBoosterType.Chrono));
            //


            foreach (var boosterItem in m_AllBoosters)
            {
                var randomPos = GetRandomPositionInCircle(playerHole.transform.position,
                    playerHole.GetWorldRadius() + 5f, playerHole.GetWorldRadius() + 0.25f);
                var booster = (IBoosterItem)boosterItem;
                booster.Init();
                booster.MoveToPos(randomPos);
            }
        }

        //List<BaseItem> SpawnBoosterItem(SpawnBoosterEventData data, MagicPouchBoosterType type)
        //{
        //    var itemDef = Config.magicPouchConfig.boosters.Find(x => x.type == type).itemDefinition;
        //    if (itemDef == null)
        //    {
        //        Debug.LogError($"Missing item definition for {type}");
        //        return null;
        //    }
        //    
        //    return LevelHelper.SpawnBoosterItems(data.countWinStreak, allItemHolder, itemDef, new ItemData
        //    {
        //        itemId = itemDef.GetId(),
        //        eulerAngle = Vector3.zero,
        //        isStartKinematic = true,
        //        position = data.posStartSpawn
        //    });
        //}

        private Vector3 GetRandomPositionInCircle(Vector3 center, float radius, float minDistanceFromCenter)
        {
            minDistanceFromCenter = Mathf.Min(minDistanceFromCenter, radius);
            var angle = Random.Range(0f, Mathf.PI * 2);
            var randomRadius = Random.Range(minDistanceFromCenter, radius);
            Vector3 randomPosition = center + new Vector3(
                Mathf.Cos(angle) * randomRadius,
                0.1f,
                Mathf.Sin(angle) * randomRadius
            );

            return randomPosition;
        }

        private void OnPauseGame(object param)
        {
            m_IsGameplayPause = true;
            Physics.autoSimulation = false;
        }

        private void OnResumeGame(object param)
        {
            m_IsGameplayPause = false;
            Physics.autoSimulation = true;
        }

        private void OnUserUseHelper(object param)
        {
            var data = (UserUseHelper) param;
            if (data.helperId == HelperId.Magnet)
            {
                playerHole.StartMagnetHelper();
                m_MagnetHelperCoolDown = PlayableController.Instance.gamePlayConfig.GetHelperActiveTime(HelperId.Magnet);
                m_MagnetHelperActive = true;
            }
        }

        private void CollectObjectiveItem(CollectItemData data)
        {
            CollectNormalItem(data);
            var levelObjective = GetLevelData().levelObjectives.Find(x => x.itemId == data.itemID);
            levelObjective.number--;
            if (levelObjective.number < 0)
            {
                levelObjective.number = 0;
            }

            this.RaiseEvent(EventList.GamePlay.COLLECT_OBJECTIVE_ITEM_NEEDED_FOR_UI, data);
            if (IsLevelComplete() && IsInResultState() == false)
            {
                EventController.RaiseEvent(EventList.GamePlay.LEVEL_WIN, new EventParam() { p0 = LevelType.NORMAL });
                ChangeState(State.Win);
            }
        }

        private void CollectNormalItem(CollectItemData data)
        {
            playerHole.AddXp(data.xp);
        }

        private void CollectBomb()
        {
            if (Game.isImmortal) return;

            if (IsInResultState() == false)
            {
                playerHole.CollectABomb();
                ChangeState(State.Lose);
                DOVirtual.DelayedCall(1.5f,
                    () =>
                    {
                        EventController.RaiseEvent(EventList.GamePlay.LEVEL_LOSE,
                            new EventParam() { p0 = LoseReason.Boom });
                    });
            }
        }

        public bool IsInResultState()
        {
            return m_CurrentState == State.Win || m_CurrentState == State.Lose;
        }

        private void CollectCoin(CollectItemData data)
        {
            CollectNormalItem(data);
            //GamePlay.gamePlayData.AddAchievementItem(LevelAchievementData.ID_COIN, data.GetCoinValue());
            //var totalAmountCollected = GamePlay.gamePlayData.GetAchievementItemCount(LevelAchievementData.ID_COIN);
            this.RaiseEvent(EventList.GamePlay.COLLECT_SPECIAL_ITEM_FOR_UI, data);
        }

        private bool IsLevelComplete()
        {
            //return false;
            var levelObjectives = GetLevelData().levelObjectives;
            if (levelObjectives.Count == 0) return true;
            return levelObjectives.All(x => x.number <= 0);
        }

        private bool IsObjectiveItem(string itemId)
        {
            if (GetLevelData().levelObjectives == null || GetLevelData().levelObjectives.Count == 0) return false;
            return GetLevelData().levelObjectives.Exists(x => x.itemId == itemId);
        }

        private bool IsBomb(string itemId)
        {
            return itemId == "Bomb";
        }

    

        private void SetUpStartHolePosition(Vector3 startPosition)
        {
            playerHole.SetPosition(startPosition);
        }

        private void RecycleAllLevelObjects()
        {
            var childCount = levelObjectHolder.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                var child = levelObjectHolder.transform.GetChild(i);
                child.gameObject.Recycle();
            }

            childCount = allItemHolder.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                var child = allItemHolder.transform.GetChild(i);
                child.gameObject.Recycle();
            }
        }

        private void ChangeState(State newState)
        {
            if (newState == m_CurrentState) return;
            ExitOldState();
            m_CurrentState = newState;
            EnterNewState();
        }

        private void EnterNewState()
        {
            switch (m_CurrentState)
            {
                case State.None:
                    break;
                case State.Begin:
                    break;
                case State.Play:
                    m_IsGameplayPause = false;
                    break;
                case State.Win:
                    break;
                case State.Lose:
                    break;
            }
        }

        private void UpdateCurrentState()
        {
            switch (m_CurrentState)
            {
                case State.None:
                    break;
                case State.Begin:
                    break;
                case State.Play:
                    if (m_IsGameplayPause) return;
                    if (m_MagnetHelperActive)
                    {
                        m_MagnetHelperCoolDown -= Time.deltaTime;
                        if (m_MagnetHelperCoolDown < 0)
                        {
                            m_MagnetHelperCoolDown = 0;
                            playerHole.StopMagnetHelper();
                            m_MagnetHelperActive = false;
                            this.RaiseEvent(EventList.GamePlay.COMPLETE_HELPER,
                                new UserCompleteHelper() { helperId = HelperId.Magnet });
                        }
                    }

                    if (m_ScaleUpHelperActive)
                    {
                        m_ScaleUpHelperCoolDown -= Time.deltaTime;
                        if (m_ScaleUpHelperCoolDown < 0)
                        {
                            m_ScaleUpHelperCoolDown = 0;
                            playerHole.StopScaleByHelper();
                            m_ScaleUpHelperActive = false;
                            this.RaiseEvent(EventList.GamePlay.COMPLETE_HELPER, new UserCompleteHelper()
                            {
                                helperId = HelperId.ScaleUp
                            });
                        }
                    }

                    break;
                case State.Win:
                    break;
                case State.Lose:
                    break;
            }
        }

        private void ExitOldState()
        {
            switch (m_CurrentState)
            {
                case State.None:
                    break;
                case State.Begin:
                    break;
                case State.Play:
                    break;
                case State.Win:
                    break;
                case State.Lose:
                    break;
            }
        }

        public enum State
        {
            None,
            Begin,
            Play,
            Win,
            Lose
        }

        public void LocateGroup()
        {
            Pool.Set(this);
        }
    }
}