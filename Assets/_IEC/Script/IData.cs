using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public interface IData
    {
    }

    public static class IDataExt
    {


        public static Game Game(this IData self)
        {
            return Pool.Get<Game>();
        }

        public static GamePlayGroup GamePlay(this IData self)
        {
            return Pool.Get<GamePlayGroup>();
        }

        public static PlayerSecret PlayerData(this IData self)
        {
            return Pool.Get<PlayerSecret>();
        }

        public static UIGroup UI(this IData self)
        {
            return Pool.Get<UIGroup>();
        }

       
    }
}