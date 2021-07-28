using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using _IECModules;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class GoogleSheetUtility : MonoBehaviour
{
    /// <summary>Scriptable Object MUST contain a List of Param</summary>
    public static void ParseLocalCsvToScriptableObject(string localUrl, ScriptableObject scriptableObject, Action<bool> onLoadComplete = null)
    {
#if UNITY_EDITOR
        var textAsset = (TextAsset) AssetDatabase.LoadAssetAtPath(localUrl, typeof(TextAsset));
        ParseCSVToScriptableObject(GoogleSheetToCSV.ParseCSV(textAsset.text, false), scriptableObject);
#endif  
    }
    public static void ParseGoogleDocToScriptableObject(string docUrl, ScriptableObject scriptableObject, IList listToParse)
    {
        ParseGoogleDocToScriptableObject(docUrl, scriptableObject, null, listToParse);
    }
    public static void ParseGoogleDocToScriptableObject(string docUrl, ScriptableObject scriptableObject, Action<bool> onLoadComplete = null, IList listToParse = null)
    {
        DownLoadCsvEditor(docUrl, false, data => { ParseCSVToScriptableObject(data, scriptableObject, onLoadComplete, listToParse); });
    }

    public static void ParseCSVToScriptableObject(List<List<string>> csvData, ScriptableObject scriptableObject, Action<bool> onLoadComplete = null, IList listToParse = null)
    {
        FieldInfo fieldList = null;
        if (listToParse != null) fieldList = scriptableObject.GetType().GetFields()
            .FirstOrDefault(f => Equals(f.GetValue(scriptableObject), listToParse));
        else fieldList = scriptableObject.GetType().GetFields().FirstOrDefault(f =>
            f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == typeof(List<>));
        if (fieldList == null) return;

        var listType = fieldList.FieldType.GetGenericArguments()[0];
        var fieldParamList = listType.GetFields().ToList();

        var listCurrentValue = (IList) fieldList.GetValue(scriptableObject);
        var listNewValue = (IList) Activator.CreateInstance(fieldList.FieldType);
        
        var firstRow = csvData[0];
        csvData.RemoveAt(0);

        var fieldParamListSorted = firstRow
            .Select(column => column.RemoveBetweenParenthesis().Replace(" ", "").Trim().ToLower())
            // remove same name column
            // .Distinct()
            .Select(lower => fieldParamList.FirstOrDefault(f => f.Name.ToLower() == lower))
            .ToList();

        foreach (var row in csvData)
        {
            var rowObject = Activator.CreateInstance(listType);
            listNewValue.Add(rowObject);

            var rowIsEmpty = true;
            for (var i = 0; i < fieldParamListSorted.Count; i++)
            {
                var fieldInfo = fieldParamListSorted[i];
                if (fieldInfo == null) continue;

                var cell = string.Empty;
                if (i < row.Count) cell = row[i];
                if (!string.IsNullOrWhiteSpace(cell)) rowIsEmpty = false;
                
                if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var childListType = fieldInfo.FieldType.GetGenericArguments()[0];
                    var childListValue =  (IList) fieldInfo.GetValue(rowObject) ?? (IList) Activator.CreateInstance(fieldInfo.FieldType);
                    SetFieldValue(childListType, cell, out var childValue);
                    childListValue.Add(childValue);
                    fieldInfo.SetValue(rowObject, childListValue);
                    continue;
                }

                var isNeedSetValue = SetFieldValue(fieldInfo.FieldType, cell, out var value);

                if(isNeedSetValue) fieldInfo.SetValue(rowObject, value);
            }
            
            if (rowIsEmpty)
            {
                listNewValue.RemoveAt(listNewValue.Count - 1);
                // break;
            }
        }
        
        onLoadComplete?.Invoke(true);
        
        Debug.Log($"{scriptableObject.name} Change Count: {CalculateChange(listCurrentValue, listNewValue)}");
        fieldList.SetValue(scriptableObject, listNewValue);
        SetDirty(scriptableObject);
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

    private static bool SetFieldValue(Type fieldType, string cell, out object value)
    {
        if (fieldType == typeof(float))
            value = string.IsNullOrEmpty(cell) ? 0 : float.Parse(cell);
        else if (fieldType == typeof(int))
            value = string.IsNullOrEmpty(cell) ? 0 : int.Parse(cell);
        else if (fieldType == typeof(bool))
            value = !string.IsNullOrEmpty(cell);
        else if (fieldType == typeof(string))
            value = string.IsNullOrEmpty(cell) ? "" : cell;
        else if (fieldType.IsEnum)
        {
            var cellTrim = cell?.Replace(" ", "").Trim();
            value = string.IsNullOrEmpty(cellTrim) ? null : Enum.Parse(fieldType, cellTrim, true);
            if (value == null) return false;
        }
        else
            value = null;

        return true;
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
            Debug.LogError("No GoogleSheetToCSV Instance");
            onError?.Invoke("No GoogleSheetToCSV Instance");
            return;
        }

        instance.DownLoadCsv(url, skipFirstLine, onComplete, onError);
#else
        Debug.LogError("Must use in Unity Editor");
        onError?.Invoke("Must use in Unity Editor");
#endif
    }
}