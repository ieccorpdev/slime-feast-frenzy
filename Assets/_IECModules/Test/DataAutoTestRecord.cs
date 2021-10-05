using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [System.Serializable]
    public class DataAutoTestRecord
    {
        public int screenWidth;
        public int screenHeight;

        public string abName;
        public int abValue;

        public PlayerSecret playerData;
        public ServiceData serviceData;

        public List<DataTapRecord> listTap;

        public string ScreenSize => screenWidth + "x" + screenHeight;
        public string AbVersion => abName + "_" + abValue;
    }

    [System.Serializable]
    public class DataTapRecord
    {
        public float time;
        public float x;
        public float y;
    }
}