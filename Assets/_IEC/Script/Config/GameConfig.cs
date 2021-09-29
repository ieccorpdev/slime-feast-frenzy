using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public ColorConfig colorConfig;
        public ComponentConfig componentConfig;
        public ShopConfig shopConfig;

        public GameObject effectLeavesObj;

        public List<TutorialConfig> listTuts;

        //Effect config
        public float startEffectMoveTime;

        public InterDelayConfig interDelayConfig;

        public DataTextConfig DataDifficutyV4Config;
        public DataTextConfig LevelDifficutyV4Config;

        public DataTextConfig ListLevelRemoveMixColorConfig;
        public DataTextConfig ListLevelsMixColorsConfig;

        public DataTextConfig ListSolutionMixColorsConfig;
        public DataTextConfig ListSolutionRemoveMixColorConfig;

        public Texture waterTexNormal, waterTexNew, distTexNormal, distTexNew;
    }
}