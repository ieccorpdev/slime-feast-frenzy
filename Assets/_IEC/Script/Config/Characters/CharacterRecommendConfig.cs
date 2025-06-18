using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace IEC
{
    [CreateAssetMenu(menuName = "Game/CharacterRecommendConfig")]
    public class CharacterRecommendConfig : ScriptableObject
    {
        public List<CharacterRecommendData> listCharacterRecommendData = new();
        public List<CharacterRecommendData> listForcedTrialData = new();
        
        [Header("Load data from csv")]
        public string csv;

        [Button]
        public void LoadData()
        {
            var dataDefault = DownloadCsvFromAuthUrl(csv);

            if (dataDefault.Count == 0)
                return;

            listCharacterRecommendData.Clear();
            
            Debug.Log(dataDefault);

            foreach (var row in dataDefault)
            {
                listCharacterRecommendData.Add(
                    new CharacterRecommendData()
                    {
                        level = int.Parse(row[0]),
                        characterName = row[1],
                        isTrial = row[2].ToLower().Equals("true")
                    });
            }
        }
        
        public CharacterRecommendData GetCharacterData(string name)
        {
            var data = listCharacterRecommendData.Find(data => data.characterName.Equals(name));

            if (data == null)
                return null;

            return data;
        }

        public CharacterRecommendData GetRecommendedCharacter(int level)
        {
            var data = listCharacterRecommendData.Find(data => data.level == level);

            if (data == null)
                return null;

            return data;
        }
        
        public CharacterRecommendData GetForcedTrialCharacter(int level)
        {
            var data = listForcedTrialData.Find(data => data.level == level);

            if (data == null)
                return null;

            return data;
        }
        
        private List<List<string>> DownloadCsvFromAuthUrl(string url)
        {
            var jsoner = new IEC.JsonDotNetModule();
            var uwr = UnityEditorHelper.GetWebRequestCsvFromAuthUrl(url);
            uwr.SendWebRequest();
            while (!uwr.isDone)
            {
                //Debug.Log(uwr.downloadProgress);
            }

            if (uwr.isNetworkError)
            {
                Debug.LogError("Download Error");
                Debug.LogError($"Downloading: {url}");
            }

            Debug.Log("Data downloaded:\n" + uwr.downloadHandler.text);

            return jsoner.FromJson<List<List<string>>>(uwr.downloadHandler.text);
        }
    }

    [System.Serializable]
    public class CharacterRecommendData
    {
        public int level;
        public string characterName;
        public bool isTrial;
    }
}
