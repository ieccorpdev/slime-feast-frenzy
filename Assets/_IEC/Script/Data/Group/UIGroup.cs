using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using _IEC.Features.Booster;
using _IEC.Features.Common.Extension;
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

        //public TMP_Text textHello;

        public ScreenShopTest ScreenShopTest => Pool.Get<ScreenShopTest>();
        public PopupFeature PopupFeature => Pool.Get<PopupFeature>();

        public List<ScreenUI> ListScreen =>
            SingleCreateIfNotExist("ListScreen", () => transform.GetComponentsInChildren<ScreenUI>(true).ToList());

        public List<ScreenUI> ListAdditiveScreen =>
            SingleCreateIfNotExist("ListAdditiveScreen", () => new List<ScreenUI>());

        public List<ScreenUI> AllScreen =>
            SingleCreateIfNotExist("AllScreen", () => new List<ScreenUI>());

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
                    var topScreen = ListScreen.Where(s => s.IsShow).OrderByDescending(s => s.ShowOrder)
                        .FirstOrDefault();
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

        public void LocateAdditiveGroup()
        {
            ListAdditiveScreen.Clear();
            var additives = FindObjectsOfType<AdditiveUIGroup>();
            foreach (var add in additives)
            {
                add.LocateGroup();
                ListAdditiveScreen.AddRange(add.ListScreen);
            }

            SetEvent(ListAdditiveScreen);
            AllScreen.Clear();
            AllScreen.AddRange(ListScreen);
            AllScreen.AddRange(ListAdditiveScreen);
        }

        void SetEvent(List<ScreenUI> list)
        {
            foreach (var screen in list)
            {
                screen.onShow.AddListener(() =>
                {
                    var allScreens = AllScreen;
                    screen.SetShowOrder(GetNextTopShowOrder(allScreens));
                    GamePlay.SetGameState(screen);
                    foreach (var s in allScreens)
                    {
                        if (s.IsShow && s != screen)
                            s.LostFocus();
                    }
                });

                screen.onHideDone.AddListener(() =>
                {
                    var allScreens = AllScreen;
                    var topScreen = allScreens
                        .Where(s => s.IsShow)
                        .OrderByDescending(s => s.ShowOrder)
                        .FirstOrDefault();
                    if (topScreen != null)
                    {
                        GamePlay.SetGameState(topScreen);
                        topScreen.HaveFocus();
                    }
                });
            }
        }

        int GetNextTopShowOrder(List<ScreenUI> listScreen)
        {
            return listScreen.Max(s => s.ShowOrder) + 1;
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
        public Vector3 GetWorldPositionFromUI(RectTransform uiImage)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(
                GamePlay.uiCamera, 
                uiImage.position
            );

            Ray ray = GamePlay.mainCamera.ScreenPointToRay(screenPoint);

            int groundLayer = LayerMask.GetMask("Ground");
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayer))
            {
                return hit.point;
            }

            Debug.LogError("No ground intersection found!");
            return Vector3.zero;
        }
    }
}