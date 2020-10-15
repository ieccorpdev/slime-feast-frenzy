using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public RectTransform bottomAds;
        public TMP_Text textHello;

        public ScreenInGame screenInGame;
        public ScreenLevelWin screenLevelWin;
        public ScreenLevelLose screenLevelLose;

        List<ScreenUI> screens;

        public void LocateGroup()
        {
            Pool.Set(this);
        }

        public void HideAllScreens()
        {
            screens = transform.GetComponentsInChildren<ScreenUI>().ToList();
            screens.ForEach(s => s.SetActive(false));
        }

        public void ShowInGame()
        {
            ShowScreen(screenInGame);
        }

        public void ShowLevelWin()
        {
            ShowScreen(screenLevelWin);
        }

        public void ShowLevelLose()
        {
            ShowScreen(screenLevelLose);
        }

        public void ShowScreen(ScreenUI screen)
        {
            HideAllScreens();
            screen.Show();
        }

    }
}