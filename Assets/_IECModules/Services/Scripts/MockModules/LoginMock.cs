using System;

namespace IEC
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
            return "Cheater";
        }

        public bool IsLoggedIn()
        {
            return true;
        }

        public string GetUserName()
        {
            return "Cheater";
        }

        public void SubmitName(string name, Action<bool> callback)
        {
            callback(true);
        }

        public void SubmitCountry(string countryCode)
        {
        }

        public bool IsSubmitCountry()
        {
            return true;
        }

        public string GetCountry()
        {
            return string.Empty;
        }

        public void LogOut()
        {

        }

    }
}