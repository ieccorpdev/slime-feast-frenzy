using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _IECModules;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class GoogleSheetUtility : MonoBehaviour
{
    /// <summary>Scriptable Object MUST contain a List of Param</summary>
    ///
    public static void ParseGoogleDocToScriptableObject(string docUrl, ScriptableObject scriptableObject)
    {
        var fieldList = scriptableObject.GetType().GetFields().FirstOrDefault(f =>
            f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == typeof(List<>));
        if (fieldList == null) return;

        var listType = fieldList.FieldType.GetGenericArguments()[0];
        var fieldParamList = listType.GetFields().ToList();

        var listCurrentValue = (IList) fieldList.GetValue(scriptableObject);
        var listNewValue = (IList) Activator.CreateInstance(fieldList.FieldType);
        
        DownLoadCsvEditor(docUrl, false, data =>
        {
            var firstRow = data[0];
            data.RemoveAt(0);

            var fieldParamListSorted = firstRow
                .Select(column => column.RemoveBetweenParenthesis().Replace(" ", "").Trim().ToLower())
                .Select(lower => fieldParamList.FirstOrDefault(f => f.Name.ToLower() == lower))
                .ToList();

            foreach (var row in data)
            {
                var rowObject = Activator.CreateInstance(listType);
                listNewValue.Add(rowObject);

                for (var i = 0; i < fieldParamListSorted.Count; i++)
                {
                    var fieldInfo = fieldParamListSorted[i];
                    if (fieldInfo == null) continue;

                    var cell = row[i];
                    object value = cell;

                    if (fieldInfo.FieldType == typeof(string))
                        value = string.IsNullOrEmpty(cell) ? "" : cell;
                    else if (fieldInfo.FieldType == typeof(float))
                        value = string.IsNullOrEmpty(cell) ? 0 : float.Parse(cell);
                    else if (fieldInfo.FieldType == typeof(int))
                        value = string.IsNullOrEmpty(cell) ? 0 : int.Parse(cell);
                    else if (fieldInfo.FieldType == typeof(bool))
                        value = !string.IsNullOrEmpty(cell);
                    else if (fieldInfo.FieldType.IsEnum)
                    {
                        value = string.IsNullOrEmpty(cell?.Trim()) ? null : Enum.Parse(fieldInfo.FieldType, cell);
                        if(value == null) continue;
                    }
                    else
                        value = null;

                    fieldInfo.SetValue(rowObject, value);
                }
            }


            Debug.Log($"{scriptableObject.name} Change Count: {CalculateChange(listCurrentValue, listNewValue)}");
            fieldList.SetValue(scriptableObject, listNewValue);
            SetDirty(scriptableObject);
        });
    }

    public static int CalculateChange(IList originalList, IList newList)
    {
        if (originalList == null || newList == null) return Mathf.Max(newList?.Count ?? 0, originalList?.Count ?? 0);
        var changeCount = Mathf.Abs(originalList.Count - newList.Count);
        for (var i = Math.Min(originalList.Count, newList.Count) - 1; i >= 0; i--)
        {
            if (JsonConvert.SerializeObject(originalList[i]) != JsonConvert.SerializeObject(newList[i])) changeCount++;
        }

        return changeCount;
    }


    public static void SetDirty(ScriptableObject scriptableObject)
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(scriptableObject);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }

    public static void DownLoadCsvEditor(string url, bool skipFirstLine = false,
        Action<List<List<string>>> onComplete = null,
        Action<string> onError = null)
    {
#if UNITY_EDITOR
        var instance = FindObjectOfType<GoogleSheetToCSV>();
        if (instance == null)
        {
            onError?.Invoke("No GoogleSheetToCSV Instance");
            return;
        }

        instance.DownLoadCsv(url, skipFirstLine, onComplete, onError);
#else
            onError?.Invoke("Must use in Unity Editor");
#endif
    }
}