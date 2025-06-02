using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IEC.CloudSave;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace IEC
{
    public class UIGroup : _BaseBehaviour, IGroup
    {
        public Canvas canvas;
        public CanvasScaler canvasScaler;
        public EventSystem eventSystem;

        public TMP_Text textHello;
        
        public ScreenInGame ScreenInGame => Pool.Get<ScreenInGame>();
        public ScreenLevelWin ScreenLevelWin => Pool.Get<ScreenLevelWin>();
        public ScreenLevelLose ScreenLevelLose => Pool.Get<ScreenLevelLose>();
        public ScreenShopTest ScreenShopTest => Pool.Get<ScreenShopTest>();

        public List<ScreenUI> ListScreen => 
            SingleCreateIfNotExist("ListScreen", () => transform.GetComponentsInChildren<ScreenUI>(true).ToList());

        public ScreenUI ScreenHome;
        public PopupCloudSave PopupCloudSave => Pool.Get<PopupCloudSave>();
        public PopUpHomeSettings PopUpHomeSettings => Pool.Get<PopUpHomeSettings>();
        // public ScreenUI ScreenHome => Pool.Get<ScreenHome>();


        public void LocateGroup()
        {
            Pool.Set(this);
            
            foreach (var screen in ListScreen)
            {
                screen.LocateScreen();
            }
        }

        void Start()
        {
            foreach (var screen in ListScreen)
            {
                screen.onShow.AddListener(() =>
                {
                    screen.SetShowOrder(CountScreenShowing());
                    GamePlay.SetGameState(screen);
                    ListScreen.Where(s => s.IsShow && s != screen).ToList().ForEach(s => s.LostFocus());
                });

                screen.onHideDone.AddListener(() =>
                {
                    var topScreen = ListScreen.Where(s => s.IsShow).OrderByDescending(s => s.ShowOrder).FirstOrDefault();
                    if (topScreen != null)
                    {
                        GamePlay.SetGameState(topScreen);
                        topScreen.HaveFocus();
                    }
                });
            }
        }

        public void HideAllScreens()
        {
            ListScreen.ForEach(s => HideScreen(s, false));
        }

        public void ShowScreen(ScreenUI screen)
        {
            if (screen.isHideAllOtherScreen)
            {
                HideAllScreens();
            }
            else
            {
                screen.listHideNoEffect.ForEach(s => HideScreen(s, false));
                screen.listHideHaveEffect.ForEach(s => HideScreen(s, true));
            }
            screen.Show();
            
            this.RaiseEvent(EventList.GamePlay.SCREEN_CHANGE, new ScreenChangeData
            {
                screenUI = screen
            });
        }

        public void HideScreen(ScreenUI screen, bool isUseEffect)
        {
            if (screen.IsHide) return;

            if (isUseEffect)
            {
                screen.Hide();
            }
            else
            {
                screen.SetActive(false);
            }
            screen.SetShowOrder(0);
        }

        public bool IsPauseGame()
        {
            var listScreen = ListScreen;
            foreach (var screen in listScreen)
            {
                if (screen.IsShow && screen.IS_PAUSE_GAME)
                {
                    return true;
                }
            }
            return false;
        }

        public int CountScreenShowing()
        {
            return ListScreen.Count(s => s.IsShow);
        }
    }
}