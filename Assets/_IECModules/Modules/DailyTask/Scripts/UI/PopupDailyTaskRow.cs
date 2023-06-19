using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC.DailyTask
{
    public class PopupDailyTaskRow : BasePopup
    {
        DailyTaskController Controller => Controllers.Get<DailyTaskController>();
        [SerializeField]
        bool m_specialTask;
        [SerializeField]
        GameObject m_panelClaimed;
        [SerializeField]
        GameObject m_iconTaskNormal, m_iconTaskAds;

        [SerializeField]
        Sprite[] m_spr_taskDiffs;
        [SerializeField]
        Image m_img_taskDiff;
        [SerializeField]
        GameObject[] m_objStars;
        [SerializeField]
        Button m_btn_claim, m_btn_refresh, m_btn_AdsTask;
        [SerializeField]
        Image m_imgProgress;
        [SerializeField]
        TextMeshProUGUI m_txtTaskDesc, m_txtRewardGem, m_txtRewardMoney, m_txtTaskProgress;
        [SerializeField]
        GameObject m_obj_claimNotif;

        DailyTaskConfig m_task;
        PopupDailyTask m_parent;
        int m_taskSaveSlot;

        private void Start()
        {
            SetEvent(m_btn_claim, () =>
            {
                m_parent.ClaimReward(m_task, m_taskSaveSlot);

            });

            if (m_btn_refresh != null)
                SetEvent(m_btn_refresh, () =>
            {
                Services.ShowRewardAds(() =>
                {
                    Controller.DataSave.ResetTaskProgress(m_taskSaveSlot);
                    m_task = Controller.RefreshTasks(m_task, m_taskSaveSlot);
                    LoadUI(m_parent, m_task, m_taskSaveSlot);
                    SetProgress(0);
                });
            });

            SetEvent(m_btn_AdsTask, () =>
            {
                Services.ShowRewardAds(() =>
                {
                    EventController.RaiseEvent(DailyTasksEventList.UPDATE_TASK, new EventParam(this, DailyTasksConfig.DailyTaskType.WatchAds));
                });
            });
        }

        public void LoadUI(PopupDailyTask parent, DailyTaskConfig task, int taskSaveSlot)
        {
            m_task = task;
            m_parent = parent;
            m_taskSaveSlot = taskSaveSlot;

            if (m_objStars.Length > 0)
                foreach (var item in m_objStars)
                {
                    item.gameObject.SetActive(false);
                }
            if (task.taskDiff == DailyTasksConfig.DailyTaskDifficult.Easy)
            {
                m_img_taskDiff.sprite = m_spr_taskDiffs[0];
                if (m_objStars.Length > 0)
                    m_objStars[0].SetActive(true);
            }
            if (task.taskDiff == DailyTasksConfig.DailyTaskDifficult.Normal)
            {
                m_img_taskDiff.sprite = m_spr_taskDiffs[1];
                if (m_objStars.Length > 0)
                {
                    m_objStars[0].SetActive(true);
                    m_objStars[1].SetActive(true);
                }
            }
            if (task.taskDiff == DailyTasksConfig.DailyTaskDifficult.Hard)
            {
                m_img_taskDiff.sprite = m_spr_taskDiffs[2];
                if (m_objStars.Length > 0)
                {
                    m_objStars[0].SetActive(true);
                    m_objStars[1].SetActive(true);
                    m_objStars[2].SetActive(true);
                }
            }
            m_txtRewardGem.text = task.rewardGem.ToString();
            m_txtRewardMoney.text = task.rewardMoney.ToString();
            var taskDesc = Controller.DataConfig.GetTaskDescByTaskType(task.taskType);
            string taskCount = task.count + "";
            if (taskDesc.taskCountText != null && taskDesc.taskCountText.Length > 0)
            {
                taskCount += " " + taskDesc.taskCountText;
                if (task.count > 1)
                    taskCount += "s";
            }
            m_txtTaskDesc.text = string.Format(taskDesc.taskText, taskCount);
            int taskProgress = Controller.DataSave.GetTaskProgress(m_taskSaveSlot);


            if (m_btn_AdsTask != null)
                m_btn_AdsTask.gameObject.SetActive(m_task.taskType == DailyTasksConfig.DailyTaskType.WatchAds);
            if (m_iconTaskNormal != null)
                m_iconTaskNormal.SetActive(!m_btn_AdsTask.gameObject.activeSelf);
            if (m_iconTaskAds != null)
                m_iconTaskAds.SetActive(m_btn_AdsTask.gameObject.activeSelf);

            SetProgress(taskProgress);

        }

        void SetProgress(int progress)
        {
            if (progress >= m_task.count) progress = m_task.count;
            m_txtTaskProgress.text = progress + "/" + m_task.count;
            m_imgProgress.transform.localScale = new Vector3((float)progress / (float)m_task.count, 1, 1);
            m_btn_claim.interactable = progress >= m_task.count && !Controller.IsTaskClaimed(m_taskSaveSlot);
            m_btn_claim.gameObject.SetActive(!Controller.IsTaskClaimed(m_taskSaveSlot));
            if (m_specialTask)
            {
                m_btn_claim.gameObject.SetActive(true);
            }

            if (m_task.taskType == DailyTasksConfig.DailyTaskType.WatchAds)
            {
                if (m_btn_AdsTask != null)
                    m_btn_AdsTask.gameObject.SetActive(!Controller.IsTaskFinish(m_task, null, m_taskSaveSlot));
            }
            else
            {
                if (m_btn_refresh != null)
                    m_btn_refresh.gameObject.SetActive(!Controller.IsTaskFinish(m_task, null, m_taskSaveSlot));
            }


            if (m_panelClaimed != null)
                m_panelClaimed.gameObject.SetActive(Controller.IsTaskClaimed(m_taskSaveSlot));
            if (m_obj_claimNotif != null)
                if (Controller.IsTaskFinish(m_task, null, m_taskSaveSlot) && !Controller.IsTaskClaimed(m_taskSaveSlot))
                    m_obj_claimNotif.SetActive(true);
                else
                    m_obj_claimNotif.SetActive(false);
        }

    }
}