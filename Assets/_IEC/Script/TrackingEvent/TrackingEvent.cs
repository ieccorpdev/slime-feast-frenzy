using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IEC.TrackingEvent
{
    public abstract class Game : AnalyticEvent
    {
        private IEC.Game m_DataGame;
        public IEC.Game DataGame => m_DataGame ?? Pool.Get<IEC.Game>();

        private IEC.PlayerSecret m_DataPlayer;
        public IEC.PlayerSecret DataPlayer => m_DataPlayer ?? Pool.Get<IEC.PlayerSecret>();

        private IEC.GamePlayGroup m_DataGamePlay;
        public IEC.GamePlayGroup DataGamePlay => m_DataGamePlay ?? Pool.Get<IEC.GamePlayGroup>();

        private IEC.GameConfig m_DataConfig;
        public IEC.GameConfig DataConfig => m_DataConfig ?? Pool.Get<IEC.GameConfig>();

        public override string Name => "GAME";
    }

    public class OpenCheat : AnalyticEvent
    {
        public override string Name => "open_cheat";
    }

    public class UnlockCheat : AnalyticEvent
    {
        public override string Name => "unlock_cheat";
    }
}