using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IEC
{
    public class LoadingStartGame : MonoBehaviour
    {
        [SerializeField] private ScreenLoading m_ScreenLoading;
        private void Start()
        {
            DOVirtual.DelayedCall(0.5f, () =>
            {
                SceneManager.LoadScene(SceneName.SERVICE);
            });
        }
    }
}