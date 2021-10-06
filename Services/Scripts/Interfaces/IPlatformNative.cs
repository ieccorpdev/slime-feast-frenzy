using System;
using UnityEngine;

namespace IEC
{
    public interface IPlatformNative
    {
        void Share(string title, string message);
        void Share(string title, string message, Texture2D image);
        void ShowPreloader();
        void HidePreloader();
        void SendMail(string title, string message, string address);
        void ShowConfirm(string title, string message, Action<bool> callback = null, string positiveBtnText = "Ok", string negativeBtnText = "Cancel");
        void ShowAlert(string title, string message, Action<bool> callback = null, string btnText = "Ok");
    }
}

