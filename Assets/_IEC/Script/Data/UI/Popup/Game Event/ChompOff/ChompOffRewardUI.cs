using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace IEC
{
    public class ChompOffRewardUI : _BaseBehaviour
    {
        public GameObject unavailableIcon;
        public GameObject availableIcon;
        public GameObject openedIcon;

        public GameObject collectedIcon;
        public GameObject lockIcon;

        public TextMeshProUGUI titleText;
        public Color activeColor;
        public Color inactiveColor;

        public void SetRewardUIState(ChompOffRewardUIState state)
        {
            unavailableIcon.SetActive(false);
            availableIcon.SetActive(false);
            openedIcon.SetActive(false);
            collectedIcon.SetActive(false);
            lockIcon.SetActive(false);
            switch (state)
            {
                case ChompOffRewardUIState.Unavailable:
                    unavailableIcon.SetActive(true);
                    lockIcon.SetActive(true);
                    break;
                case ChompOffRewardUIState.Available:
                    availableIcon.SetActive(true);
                    break;
                case ChompOffRewardUIState.Opened:
                    openedIcon.SetActive(true);
                    collectedIcon.SetActive(true);
                    break;
            }
        }

        public void SetUITextActive(bool isActive)
        {
            titleText.color = isActive ? activeColor : inactiveColor;
        }
    }

    public enum ChompOffRewardUIState
    {
        Unavailable,
        Available,
        Opened
    }
}
