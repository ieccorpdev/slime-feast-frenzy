using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace IEC
{
    [Serializable]
    public class UserVersionData
    {
        public const string KEY = "UserVersionData";
        public static string ApplicationVersion
        {
            get
            {
                if (Application.isEditor)
                {
                    return Controllers.Get<ServicesController>().config.GetPlatformVersion().version;
                }
                return Application.version;
            }
        }

        public List<UserVersion> listInfo = new List<UserVersion>();

        public UserVersion LastVersion()
        {
            return listInfo.Last();
        }

        public void AddUserVersion(UserVersion newUserVersion, bool isOverrideUserVer = false)
        {
            var userVersion = listInfo.Find(x => x.version == newUserVersion.version);
            if (userVersion == null)
            {
                listInfo.Add(newUserVersion);
            }
            if (isOverrideUserVer)
            {
                userVersion = listInfo.Find(x => x.version == newUserVersion.version);
                userVersion.abFullName = newUserVersion.abFullName;
            }
        }

        public bool IsEmpty()
        {
            return listInfo.Count == 0;
        }
    }

    [Serializable]
    public class UserVersion
    {
        public string version;
        public string abFullName;
        public bool isOverride;
        public bool isAbTestOldUser;

        public UserVersion() { }
        public UserVersion(string version, string abFullName, bool isOverride)
        {
            this.version = version;
            this.abFullName = abFullName;
            this.isOverride = isOverride;
        }

        public int Date() => int.Parse(abFullName.Split('_')[1]);
    }
}