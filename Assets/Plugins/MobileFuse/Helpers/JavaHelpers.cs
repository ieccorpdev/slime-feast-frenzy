using UnityEngine;

namespace MobileFuseAds.Helpers
{
    public static class JavaHelpers
    {
        private static AndroidJavaObject _handler;
        
        public static void ExecuteOnMainThread(MobileFuse.NoArgsDelegate action)
        {
            Handler.Call<bool>("post", new Runnable(action));
        }

        private static AndroidJavaObject Handler
        {
            get
            {
                if (_handler != null) return _handler;
                _handler = new AndroidJavaObject("android.os.Handler",
                    new AndroidJavaClass("android.os.Looper").CallStatic<AndroidJavaObject>("getMainLooper")
                );
                return _handler;
            }
        }
        
        public class Runnable : AndroidJavaProxy
        {
            private readonly MobileFuse.NoArgsDelegate _callback;

            public Runnable(MobileFuse.NoArgsDelegate callback) : base("java.lang.Runnable")
            {
                _callback = callback;
            }
            
            // ReSharper disable once InconsistentNaming (Proxy implementation)
            public void run() => _callback();
        }
    }
}