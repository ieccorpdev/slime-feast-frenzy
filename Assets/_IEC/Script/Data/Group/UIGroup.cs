using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using NaughtyAttributes;

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
        public ScreenShopTest screenShopTest;

        public void LocateGroup()
        {
            Pool.Set(this);
        }

        public void HideAllScreens()
        {
            var screens = transform.GetComponentsInChildren<ScreenUI>().ToList();
            screens.ForEach(s => HideScreen(s, false));
        }

        [Button]
        public void ShowScreenByGameState()
        {
            ShowScreen(Game.gameState);
        }

        public void ShowScreen(GameState state)
        {
            switch(state)
            {
                case GameState.Playing:
                    ShowScreen(screenInGame);
                    break;
                case GameState.Win:
                    ShowScreen(screenLevelWin);
                    break;
                case GameState.Lose:
                    ShowScreen(screenLevelLose);
                    break;
            }
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
        }

        public void HideScreen(ScreenUI screen, bool isUseEffect)
        {
            if (isUseEffect)
            {
                screen.Show();
            }
            else
            {
                screen.SetActive(false);
            }
        }

    }
}