using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using Sirenix.OdinInspector;
using _IEC.Features.Gameplay;

[CreateAssetMenu(fileName = "LevelTimeBonusData", menuName = "Game Data/Level Time Bonus Data")]
public class LevelTimeBonusData : ScriptableObject
{
    public List<CustomLevelData> bonusLevels;

    public int GetBonusForLevel(int level)
    {
        foreach (var bonus in bonusLevels)
        {
            if (bonus.level == level)
                return bonus.timeExpand;
        }
        return 0;
    }

#if UNITY_EDITOR
    public string googleSheetUrl;
    [Button]
    public void UpdateTimeData()
    {
        EditorApplication.update += UpdateTimeDataLoop;
    }

    private bool isUpdating = false;
    private List<List<string>> sheetData = null;

    private void UpdateTimeDataLoop()
    {
        if (isUpdating) return;

        isUpdating = true;

        DownloadCsvFromAuthUrl(googleSheetUrl, result =>
        {
            sheetData = result;

            if (sheetData == null || sheetData.Count <= 1)
            {
                Debug.LogError("Failed to load Google Sheet data.");
                StopUpdateTimeDataLoop();
                return;
            }

            bonusLevels.Clear();

            for (int i = 1; i < sheetData.Count; i++)
            {
                var row = sheetData[i];
                if (!int.TryParse(row[0], out int levelIndex)) continue;
                int timerValue = row[1] == "∞" ? 0 : int.TryParse(row[1], out int tempValue) ? tempValue : -1;
                if (timerValue == -1) continue;
                bonusLevels.Add(new CustomLevelData
                {
                    level = levelIndex,
                    timeExpand = timerValue
                });
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            Debug.Log("<color=green>Time data updated successfully!</color>");

            StopUpdateTimeDataLoop();
        });
    }

    private void StopUpdateTimeDataLoop()
    {
        EditorApplication.update -= UpdateTimeDataLoop;
        isUpdating = false;
    }

    private void DownloadCsvFromAuthUrl(string url, System.Action<List<List<string>>> callback)
    {
        UnityWebRequest uwr = UnityEditorHelper.GetWebRequestCsvFromAuthUrl(url);
        uwr.SendWebRequest().completed += operation =>
        {
            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Download Error: " + uwr.error);
                callback?.Invoke(null);
                return;
            }

            var jsoner = new IEC.JsonDotNetModule();
            List<List<string>> sheetData = jsoner.FromJson<List<List<string>>>(uwr.downloadHandler.text);
            callback?.Invoke(sheetData);
        };
    }
#endif

}
