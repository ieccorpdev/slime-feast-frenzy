using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using Random = System.Random;

namespace IEC.ClickEffectModule
{
    public class ScreenClickEffect : _BaseBehaviour
    {
        public Transform vfxHolder;
        public Camera cameraVFXClick;
        [Header("Config")]
        public ModeClickEffectScreen modeClick;
        public bool turnOn = true;
        [ShowIf("modeClick",ModeClickEffectScreen.Single)]
        [Header("Prefab Effect Click Screen")] 
        public EffectClickData effectClickScreen;
        [ShowIf("modeClick",ModeClickEffectScreen.List)]
        [Header("List Prefab Effect Click Screen")] 
        public ClickEffectRandomListType typeRandomEffect;
        [ShowIf("modeClick",ModeClickEffectScreen.List)]
        public List<EffectClickData> listEffectClickScreen;
        private Sequence _sequence;
        private int count = 0;

        void Start()
        {
            foreach (var effect in listEffectClickScreen)
            {
                PrefabHelper.SetUpPrefab(effect.effectClick);
            }

            PrefabHelper.SetUpPrefab(effectClickScreen.effectClick);
            _sequence = DOTween.Sequence();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && turnOn)
            {
                var pos = cameraVFXClick.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 5;
                var data = GetObjectClickEffect();
                var vfx = PrefabHelper.Clone(data.effectClick, vfxHolder);
                vfx.transform.position = pos;
                vfx.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
                _sequence.AppendInterval(data.timeEffect).OnComplete(() => { PrefabHelper.Destroy(vfx); });
            }
        }

        private EffectClickData GetObjectClickEffect()
        {
            switch (modeClick)
            {
                case ModeClickEffectScreen.Single:
                    return effectClickScreen;
                case ModeClickEffectScreen.List:
                    int id = GetIndexList();
                    return listEffectClickScreen[id];
                default:
                    return effectClickScreen;
            }
        }

        private int GetIndexList()
        {
            switch (typeRandomEffect)
            {
                case ClickEffectRandomListType.Random:
                    return UnityEngine.Random.Range(0, listEffectClickScreen.Count);
                case ClickEffectRandomListType.Increase:
                    int idi = count;
                    count = count + 1 >= listEffectClickScreen.Count ? 0 : count + 1;
                    return idi;
                case ClickEffectRandomListType.Decrease:
                    int idd = count;
                    count = count - 1 < 0 ? listEffectClickScreen.Count - 1 : count - 1;
                    return idd;
                default:
                    return UnityEngine.Random.Range(0, listEffectClickScreen.Count);
            }
        }
    }

    [Serializable]
    public class EffectClickData
    {
        public GameObject effectClick;
        public float timeEffect = 2f;
    }

    public enum ModeClickEffectScreen
    {
        Single,
        List
    }

    public enum ClickEffectRandomListType
    {
        Random,
        Increase,
        Decrease
    }
}