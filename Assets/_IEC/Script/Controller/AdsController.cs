using System.Collections;
using UnityEngine;

namespace IEC
{
    public class AdsController : _BaseBehaviour, IController, IEventListener, IEventAwake, IEventStart
    {
        public void LocateController()
        {
            Controllers.Add(this);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }

        public void OnEventRaise(string gameEvent, EventParam param)
        {
            switch (gameEvent)
            {
                case EventList.GamePlay.END_LEVEL_ADS:
                    
                    break;
                case EventList.Service.INIT_AD_DONE:
                    UpdateWhiteBannerSize();
                    break;

            }
        }

        public void BehaviourAwake()
        {
        }
        public void BehaviourStart()
        {
           
        }

        public void UpdateWhiteBannerSize()
        {
        }

        Vector2 GetNativeDPI()
        {
            // AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            // AndroidJavaObject activity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
            //
            // AndroidJavaObject metrics = new AndroidJavaObject("android.util.DisplayMetrics");
            // activity.Call<AndroidJavaObject>("getWindowManager").Call<AndroidJavaObject>("getDefaultDisplay").Call("getMetrics", metrics);
            //
            // return new Vector2(metrics.Get<float>("xdpi"), metrics.Get<float>("ydpi"));
            return Vector2.zero;
        }
    }
}