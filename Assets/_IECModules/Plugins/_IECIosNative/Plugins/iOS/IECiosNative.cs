#if UNITY_IOS
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace IEC
{
    public class IECiosNative
    {
        [DllImport("__Internal")]
        private static extern void _openURL(string urlString);

        [DllImport("__Internal")]
        private static extern void _openAppSetting();
        
        
        public static void OpenURL(string url)
        {
#if UNITY_IOS && !UNITY_EDITOR
            _openURL(url);
#endif
        }
        
        public static void OpenAppSetting()
        {
#if UNITY_IOS && !UNITY_EDITOR
            _openAppSetting();
#endif
        }
        
    }
}
#endif