using System.Collections;
using System.Collections.Generic;
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
        public TMP_Text textHello, txtLevel, txtGO;

        public GameObject groupGameOver;

        public Button btnRestart, btnSkip, btnGONext, btnSetting, btnHint;

        public PopupSetting popupSettings;

        public void LocateGroup()
        {
            Pool.Set(this);
        }
    }
}