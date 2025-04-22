using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace IEC
{
#if UNITY_IOS && !UNITY_EDITOR
    public class IECiosNative
    {

        [DllImport("__Internal")]
        private static extern void _openURL(string urlString);

        [DllImport("__Internal")]
        private static extern void _openAppSetting();

        public static void OpenURL(string url)
        {

            _openURL(url);
        }
        
        public static void OpenAppSetting()
        {
            _openAppSetting();

        }
    }
#endif
}