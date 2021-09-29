using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLog : MonoBehaviour
{
    static List<Logs> listLogs = new List<Logs>();
    
    public static void AddLog(Logs log)
    {
        listLogs.Add(log);
    }

    public static List<Logs> GetLog()
    {
        return listLogs;
    }
}

public struct Logs
{
    public string condition;
    public string stackTrace;
    public LogType type;
    public string dateTime;
}
