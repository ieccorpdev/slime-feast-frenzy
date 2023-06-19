using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC.DailyTask
{
    public class DailyTaskController : _BaseBehaviour, IController, IEventAwake, IEventUpdate, IEventListener
    {
        public DailyTaskDataSave DataSave { get; set; }
        public DailyTasksConfig DataConfig { get; set; }

        public PopupDailyTask PopupDailyTask; //=> UI.popupDailyTask;
        public GameObject NotifDailyTask; //=> UI.screenInGame.NotifDailyTask

        #region Implement
        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void BehaviourAwake()
        {
            //if (!Game.isHaveDailyTask)
            //    gameObject.SetActive(false);
            InitData();
        }

        public void BehaviourUpdate(float deltaTime)
        {
            //if (!Game.isHaveDailyTask)
            //    gameObject.SetActive(false);
            TimeToReset();
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.T))
            {
                ResetTasks();
            }
#endif
        }


        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case DailyTasksEventList.UPDATE_TASK:
                    DailyTasksConfig.DailyTaskType taskType = (DailyTasksConfig.DailyTaskType)param.p0;
                    UpdateTask(taskType);
                    PopupDailyTask.UpdateUI();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Private
        private void InitData()
        {
            //Load DataSave:
            DataController Data = Controllers.Get<DataController>();
            DataSave = Data.LoadData<DailyTaskDataSave>(DailyTaskConstants.PLAYER_DATA_DAILYTASK_KEY);
            Data.SetAutoSaveObject(DailyTaskConstants.PLAYER_DATA_DAILYTASK_KEY, DataSave);
            //Load Config:
            DataConfig = Resources.Load<DailyTasksConfig>(DailyTaskConstants.CONFIG_DAILYTASK_PATH);
            //Check and Init TasksL
            if (DataSave.currentTasks == null || DataSave.currentTasks.Count == 0)
                ResetTasks();
        }

        private void ResetTasks()
        {
            var taskComplete = DataConfig.GetTaskByType(DailyTasksConfig.DailyTaskType.Complete);
            DataSave.ResetTaskProgress(1);
            DataSave.ClaimTaskClear(1);

            DataSave.currentTasks = new List<string>();
            List<DailyTaskDiffConfig> taskDiffConfig = DataConfig.taskDiffConfig;
            int i = 0;
            int taskIndex = 2;
            foreach (var item in taskDiffConfig)
            {
                while (i < item.missionPerDay)
                {

                    var tasks = DataConfig.GetTaskByDiff(item.taskDiff);
                    var task = tasks[UnityEngine.Random.Range(0, tasks.Count)];
                    if (!DataSave.currentTasks.Contains(task.id))
                    {
                        DataSave.currentTasks.Add(task.id);
                        DataSave.ResetTaskProgress(taskIndex);
                        DataSave.ClaimTaskClear(taskIndex);
                        taskIndex++;
                        i++;
                    }
                }
                i = 0;
            }
            DataSave.ClaimTaskClear();
            PopupDailyTask.UpdateUI(true);
            NotifDailyTask.SetActive(IsHaveTaskNeedClaim());
            Debug.Log("Reset Task");
        }
        #endregion

        #region Public
        public List<DailyTaskConfig> GetAllCurrentTask()
        {
            List<DailyTaskConfig> allTasks = new List<DailyTaskConfig>();
            allTasks.Add(DataConfig.GetTaskByType(DailyTasksConfig.DailyTaskType.WatchAdsInfinity).Clone(0));
            allTasks.Add(DataConfig.GetTaskByType(DailyTasksConfig.DailyTaskType.Complete).Clone(1));
            List<string> allNormalTasksIDs = DataSave.currentTasks;
            int saveSlot = 2;
            foreach (string id in allNormalTasksIDs)
            {
                allTasks.Add(DataConfig.GetTaskByID(id).Clone(saveSlot));
                saveSlot++;
            }
            return allTasks;
        }

        public DailyTaskConfig RefreshTasks(DailyTaskConfig currentTask, int taskSaveSlot)
        {
            var tasks = DataConfig.GetTaskByDiff(currentTask.taskDiff);
            var task = tasks[UnityEngine.Random.Range(0, tasks.Count)];
            int index = taskSaveSlot - 2;
            DataSave.currentTasks[index] = task.id;
            DataSave.ResetTaskProgress(taskSaveSlot);
            DataSave.ClaimTaskClear(taskSaveSlot);
            Debug.Log("RefreshTasks " + currentTask.id + "," + taskSaveSlot + " to " + task.id);
            return task;
        }

        public bool IsHaveTaskNeedClaim()
        {
            int taskSaveSlot = 0;
            List<DailyTaskConfig> allTasks = GetAllCurrentTask();
            foreach (DailyTaskConfig task in allTasks)
            {
                if (IsTaskFinish(task, null, taskSaveSlot) && !IsTaskClaimed(taskSaveSlot))
                    return true;
                taskSaveSlot++;
            }
            return false;
        }

        public bool IsTaskClaimed(int taskSaveSlot)
        {
            return DataSave.IsTaskClaim(taskSaveSlot);
        }
        public bool IsTaskFinish(DailyTaskConfig taskInput, string taskID, int taskSaveSlot)
        {
            DailyTaskConfig task = taskInput != null ? taskInput : DataConfig.GetTaskByID(taskID);
            taskID = task.id;
            int taskProgress = DataSave.GetTaskProgress(taskSaveSlot);
            if (taskProgress >= task.count)
                return true;
            return false;
        }
        public List<DailyTaskConfig> GetCurrentTasksByType(DailyTasksConfig.DailyTaskType type)
        {
            List<DailyTaskConfig> output = new List<DailyTaskConfig>();
            List<DailyTaskConfig> allCurrentTasks = GetAllCurrentTask();
            foreach (DailyTaskConfig task in allCurrentTasks)
            {
                if (task.taskType == type)
                    output.Add(task);
            }
            return output;
        }
        public void UpdateTask(DailyTasksConfig.DailyTaskType type)
        {
            List<DailyTaskConfig> allTasksOfType = GetCurrentTasksByType(type);

            if (allTasksOfType == null || allTasksOfType.Count == 0) return;
            DailyTasksConfig.DailyTaskDifficult[] tasksOrder = {
                DailyTasksConfig.DailyTaskDifficult.Easy,
                DailyTasksConfig.DailyTaskDifficult.Normal,
                DailyTasksConfig.DailyTaskDifficult.Hard
            };
            allTasksOfType.OrderBy(x => Array.IndexOf(tasksOrder, x.taskDiff));

            DailyTasksConfig.DailyTaskDifficult currentDiff = DailyTasksConfig.DailyTaskDifficult.None;
            for (int i = 0; i < allTasksOfType.Count; i++)
            {
                DailyTaskConfig task = allTasksOfType[i];
                if (IsTaskFinish(task, null, task.SaveSlot)) continue;
                if (currentDiff != DailyTasksConfig.DailyTaskDifficult.None)
                {
                    if (task.taskDiff != currentDiff)
                        break;
                }
                DataSave.AddTaskProgress(task.SaveSlot);
                currentDiff = task.taskDiff;
            }
        }

        public void ClaimTaskSaveData(DailyTaskConfig task, int taskSaveSlot)
        {
            //Update SaveData:
            DataSave.ClaimTask(taskSaveSlot);
            //update task complete
            if (task.taskType != DailyTasksConfig.DailyTaskType.Complete && task.taskType != DailyTasksConfig.DailyTaskType.WatchAdsInfinity)
                EventController.RaiseEvent(DailyTask.DailyTasksEventList.UPDATE_TASK, new EventParam(this, DailyTask.DailyTasksConfig.DailyTaskType.Complete));
            //Save:
            EventController.RaiseEvent(EventList.SAVE_DATA);
            //Update Task Complete:
        }
        public void ClaimTaskReward(DailyTaskConfig task)
        {
            //add Gem
            Debug.LogError("Handle Add Gem "+task.rewardGem);
            //add gold
            Debug.LogError("Handle Add Gold " + task.rewardMoney);
            //Save:
            EventController.RaiseEvent(EventList.SAVE_DATA);
        }

        public DateTime TimeToReset()
        {
            var timeNow = DateTime.Now;
            var timeReset = timeNow.Date.AddHours(12);//12h today
            if (timeReset < timeNow)
            {
                timeReset = timeReset.AddDays(1); //12h tomorrow
                                                  //  UI.popupDailyTask.UpdateUI();
            }

            if (timeReset != DataSave.deadlineDate)
            {
                DataSave.deadlineDate = timeReset;
                ResetTasks();

            }
            return timeReset;
        }

        #endregion
    }
}