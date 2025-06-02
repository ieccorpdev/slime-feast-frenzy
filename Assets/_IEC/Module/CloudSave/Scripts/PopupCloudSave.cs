using UnityEngine;
using UnityEngine.UI;

namespace IEC.CloudSave
{
    public class PopupCloudSave : BasePopup
    {
        public GroupSaveDataInfo groupInfoLocal;
        public GroupSaveDataInfo groupInfoCloud;

        public Button btnLocal;
        public Button btnCloud;
        
        private bool _isFirstTime;

        private CloudSaveController CloudSaveController => Controllers.Get<CloudSaveController>();

        protected override void Awake()
        {
            base.Awake();

            SetEvent(btnLocal, OnBtnLocalClick);
            SetEvent(btnCloud, OnBtnCloudClick);

            // TODO: Uncommnent this to check if player is playing when popup shows/hide first time
            // onShow.AddListener(() =>
            // {
            //     if (LevelController.CurrentState == LevelController.State.Play)
            //         this.RaiseEvent(EventList.GamePlay.PAUSE_GAME);
            // });
            // onHide.AddListener(() =>
            // {
            //     if (LevelController.CurrentState == LevelController.State.Play)
            //         this.RaiseEvent(EventList.GamePlay.RESUME_GAME);
            // });
        }

        private void OnBtnLocalClick()
        {
            // If this is show the first time user install, skip apply local save data
            if (!_isFirstTime)
                CloudSaveController.ApplyLocalSaveData();
            UI.HideScreen(Locator.Main.PopupCloudSave, true);
        }

        private void OnBtnCloudClick()
        {
            CloudSaveController.ApplyCloudSaveData();
            Locator.Main.PopupSettingInHome.SetCloudSaveOnExternally();
        }

        public override void Show()
        {
            UpdateData();
            base.Show();
        }

        private void UpdateData()
        {
            groupInfoLocal.Init(PlayerData);
            groupInfoCloud.Init(CloudSaveController.GetCloudPlayerData());
        }

        public void SetupFirstTime(bool isFirstTime)
        {
            _isFirstTime = isFirstTime;
        }

        private void OnEnable()
        {
            Debug.LogError("ON ENABLE");
        }
        
        private void OnDisable()
        {
            Debug.LogError("ON DISABLE");
        }
    }
}