using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Root
{
    [CreateAssetMenu(fileName = "Config", menuName = "Remote/Config")]
    public class RemoteConfig : ScriptableObject
    {
        [HideInInspector]
        public string key;
        [HideInInspector]
        public RemoteType type;

        [HideInInspector]
        public bool boolDefault;
        [HideInInspector]
        public bool boolTest;

        [HideInInspector]
        public int intDefault;
        [HideInInspector]
        public int intTest;

        [HideInInspector]
        public string stringDefault;
        [HideInInspector]
        public string stringTest;


    }

    public enum RemoteType { Bool, Int, String }
}
