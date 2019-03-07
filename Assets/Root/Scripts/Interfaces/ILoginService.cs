using System;

namespace Root
{
    public interface ILoginService
    {
        void Login(string inputName, Action<bool> callback, bool isOldNick = false);
        void IsHaveServerName(Action<bool> callback);
        string GetUserId();
        bool IsLoggedIn();
        string GetUserName();
        void SubmitName(string name, Action<bool> callback);
    }

    public class UserInfo
    {
        public string fb_id;
        public string name;
    }
}

