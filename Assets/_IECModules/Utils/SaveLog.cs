using System.Collections;
using System.Collections.Generic;
using System.Text;
using IEC;
using UnityEngine;

public class SaveLog : MonoBehaviour
{
    static List<Logs> listLogs = new List<Logs>();
    
    public static void AddLog(Logs log)
    {
        listLogs.Add(log);
    }

    public static string GetLog(IJsonService json, int sizeLog)
    {
        var curLogSize = Encoding.UTF8.GetByteCount(json.ToJson(listLogs));
        while (curLogSize > sizeLog)
        {
            var percentRemove = 1f * (curLogSize - sizeLog) / curLogSize;
            var countLogRemove = (int)(percentRemove * listLogs.Count);
            listLogs.RemoveRange(0, countLogRemove);
            curLogSize = Encoding.UTF8.GetByteCount(json.ToJson(listLogs));
        }
        return json.ToJson(listLogs);
    }
}

public struct Logs
{
    public string condition;
    public string stackTrace;
    public LogType type;
    public string dateTime;
}
