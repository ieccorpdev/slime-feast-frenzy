using System;
using UnityEngine;

namespace IEC
{
    public class AndroidNativeMock : IPlatformNative
    {
        public void SendMail(string title, string message, string address)
        {
            Debug.Log("SendMail");
            Debug.Log(title + " : " + message);
        }

        public void Share(string title, string message)
        {
            Debug.Log("Share");
            Debug.Log(title + " : " + message);
        }

        public void Share(string title, string message, Texture2D image)
        {
            Debug.Log("Share image");
            Debug.Log(title + " : " + message);
        }

        public void ShowAlert(string title, string message, Action<bool> callback = null, string btnText = "Ok")
        {
            Debug.Log("ShowAlert");
            Debug.Log(title + " : " + message);
            if (callback != null)
            {
                callback(true);
            }
        }

        public void ShowConfirm(string title, string message, Action<bool> callback = null, string positiveBtnText = "Ok", string negativeBtnText = "Cancel")
        {
            Debug.Log("ShowConfirm");
            Debug.Log(title + " : " + message);
            if (callback != null)
            {
                callback(true);
            }
            if (callback != null)
            {
                callback(true);
            }
        }

        public void ShowPreloader()
        {
            Debug.Log("ShowPreloader");
        }

        public void HidePreloader()
        {
            Debug.Log("HidePreloader");
        }
    }
}