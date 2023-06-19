using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC.DailyTask
{
    public class DailyTaskDataSave
    {
        public DateTime deadlineDate;
        public List<string> currentTasks;
        public Dictionary<int, int> taskProgress;
        public List<int> taskClaimed;

        public int GetTaskProgress(int saveSlot)
        {
            if (taskProgress == null) taskProgress = new Dictionary<int, int>();
            if (!taskProgress.ContainsKey(saveSlot)) taskProgress.Add(saveSlot, 0);
            return taskProgress[saveSlot];
        }
        public void AddTaskProgress(int saveSlot, int addValue = 1)
        {
            if (taskProgress == null) taskProgress = new Dictionary<int, int>();
            if (!taskProgress.ContainsKey(saveSlot)) taskProgress.Add(saveSlot, 0);
            taskProgress[saveSlot] += 1;
        }
        public void ResetTaskProgress(int saveSlot, int addValue = 1)
        {
            if (taskProgress == null) taskProgress = new Dictionary<int, int>();
            if (!taskProgress.ContainsKey(saveSlot)) taskProgress.Add(saveSlot, 0);
            taskProgress[saveSlot] = 0;
        }
        public void ClaimTaskClear()
        {
            if (taskClaimed == null) taskClaimed = new List<int>();
            else taskClaimed.Clear();
        }
        public void ClaimTaskClear(int saveSlot)
        {
            if (taskClaimed == null) taskClaimed = new List<int>();
            else if (taskClaimed.Contains(saveSlot)) taskClaimed.Remove(saveSlot);
        }
        public void ClaimTask(int saveSlot)
        {
            if (taskClaimed == null) taskClaimed = new List<int>();
            if (!taskClaimed.Contains(saveSlot))
                taskClaimed.Add(saveSlot);
        }

        public bool IsTaskClaim(int saveSlot)
        {
            if (taskClaimed == null) taskClaimed = new List<int>();
            return taskClaimed.Contains(saveSlot);
        }
    }
}