using System;

namespace Root
{
    public class LoginMock : ILoginService
    {

        public void Login(string inputName, Action<bool> callback, bool isOldNick = false)
        {
            callback(true);
        }

        public void IsHaveServerName(Action<bool> callback)
        {
            callback(false);
        }

        public string GetUserId()
        {
            return "Tester";
        }

        public bool IsLoggedIn()
        {
            return true;
        }

        public string GetUserName()
        {
            return "Tester";
        }

        public void SubmitName(string name, Action<bool> callback)
        {
            callback(true);
        }

    }
}