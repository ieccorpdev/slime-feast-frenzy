using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC.DailyTask
{
    public class PopupDailyTask : BasePopup
    {
        DailyTaskController Controller => Controllers.Get<DailyTaskController>();

        [Header("MainPanel=====")]
        [SerializeField]
        GameObject m_panelMain;
        [SerializeField]
        TextMeshProUGUI m_txtTime;
        [SerializeField]
        Transform m_tasksNormalParent;
        [SerializeField]
        PopupDailyTaskRow m_taskFinal, m_taskCulmulative;
        [Header("RewardPanel=====")]
        [SerializeField]
        GameObject m_panelReward;
        [SerializeField]
        TextMeshProUGUI m_txtRewardMoney, m_txtRewardGem;
        [SerializeField]
        Button m_btnRewardContinue;
        [SerializeField]
        RectTransform m_targetRewardGem, m_targetRewardMoney;

        PopupDailyTaskRow[] m_taskRows;
        DateTime m_timeToReset;
        DailyTaskConfig m_taskClaim;
        int m_taskClaimSaveSlot;
        DateTime m_dealineTimeWhenClickClaim;

        private void Start()
        {
            SetEvent(m_btnRewardContinue, () =>
            {

                if (m_dealineTimeWhenClickClaim == Controller.DataSave.deadlineDate)
                {
                    //only reset task if is today's task
                    Controller.ClaimTaskSaveData(m_taskClaim, m_taskClaimSaveSlot);
                }

                if (m_taskClaim.taskType == DailyTasksConfig.DailyTaskType.WatchAdsInfinity)
                {
                    Controller.DataSave.ResetTaskProgress(m_taskClaimSaveSlot);
                    Controller.DataSave.ClaimTaskClear(m_taskClaimSaveSlot);
                }
                Controller.ClaimTaskReward(m_taskClaim);
                UpdateUI();
                m_taskClaim = null;
                Hide();
            });
        }

        public override void Show()
        {
            base.Show();
            m_panelMain.SetActive(true);
            m_panelReward.SetActive(false);
            if (m_taskRows == null || m_taskRows.Length == 0)
                m_taskRows = m_tasksNormalParent.GetComponentsInChildren<PopupDailyTaskRow>();
            UpdateUI();
            m_panelReward.gameObject.SetActive(false);
        }

        public void UpdateUI(bool resetTaskOrder = false)
        {
            m_timeToReset = Controller.TimeToReset();
            List<DailyTaskConfig> tasks = Controller.GetAllCurrentTask();
            if (m_taskRows == null || m_taskRows.Length == 0)
                m_taskRows = m_tasksNormalParent.GetComponentsInChildren<PopupDailyTaskRow>(true);
            foreach (var item in m_taskRows)
            {
                item.gameObject.SetActive(false);
            }

            DailyTaskConfig taskAdsInfinity = Controller.DataConfig.GetTaskByType(DailyTasksConfig.DailyTaskType.WatchAdsInfinity);
            DailyTaskConfig taskComplete = Controller.DataConfig.GetTaskByType(DailyTasksConfig.DailyTaskType.Complete);

            m_taskCulmulative.LoadUI(this, taskAdsInfinity, 0);
            m_taskFinal.LoadUI(this, taskComplete, 1);


            for (int i = 2; i < tasks.Count; i++)
            {
                PopupDailyTaskRow row = m_taskRows[i - 2];
                if (resetTaskOrder)
                    row.transform.SetSiblingIndex(m_taskRows.Length);
                DailyTaskConfig task = tasks[i];
                row.LoadUI(this, tasks[i], i);
                row.gameObject.SetActive(true);
                if (Controller.IsTaskFinish(null, task.id, i))
                {
                    if (Controller.IsTaskClaimed(i))
                    {
                        row.transform.SetAsLastSibling();
                    }
                    else
                    {
                        row.transform.SetAsFirstSibling();
                    }
                }
            }

            Controller.NotifDailyTask.SetActive(Controller.IsHaveTaskNeedClaim());

        }

        private void Update()
        {
            var totalSeconds = (int)((m_timeToReset - DateTime.Now).TotalSeconds);
            m_txtTime.text = TimeSpan.FromSeconds(totalSeconds).ToString(@"hh\:mm\:ss");
        }

        public void ClaimReward(DailyTaskConfig task, int saveSlot)
        {
            m_taskClaim = task;
            m_taskClaimSaveSlot = saveSlot;
            m_panelMain.SetActive(false);
            m_panelReward.SetActive(true);
            m_txtRewardMoney.text = task.rewardMoney.ToString();
            m_txtRewardGem.text = task.rewardGem.ToString();
            m_dealineTimeWhenClickClaim = Controller.DataSave.deadlineDate;
        }
    }
}