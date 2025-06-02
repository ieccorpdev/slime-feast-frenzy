#if UNITY_ANDROID
using UnityEngine;
using GooglePlayGames;
using System;
using System.Collections.Generic;
using System.Text;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using Newtonsoft.Json;

namespace IEC.CloudSave
{
    public class GooglePlayCloudSaveModule : ICloudSaveModule
    {
        private const string SAVE_FILENAME = "game_save";
        private string _cachedData = ""; // Cached JSON string
        private ISavedGameMetadata _savedGameMetadata;

        private Action _onSyncCompleted;
        public Action<bool> OnAuthenticated { get; set; }

        public string UserId => PlayGamesPlatform.Instance.localUser.authenticated ? PlayGamesPlatform.Instance.GetUserId() : "0";

        public void Init()
        {
            PlayGamesPlatform.Activate(); // Activate Google Play Games
            
        }

        public void Authenticate()
        {
            PlayGamesPlatform.Instance.Authenticate(status =>
            {
                Debug.LogWarning("[Google Play Games] Auto Sign In: " + status);
                OnAuthenticated?.Invoke(status == SignInStatus.Success);
                _onSyncCompleted?.Invoke(); // Notify when authentication is complete
                if (status == SignInStatus.Success)
                {
                    Debug.LogWarning("[Google Play Games] Signed In Successfully!");
                }
                else
                {
                    Debug.LogWarning("[Google Play Games] Signed In Failed.");
                }
            });
        }

        /// <summary>
        /// Authenticate User with Google Play Games.
        /// </summary>
        public void ManualAuthenticate()
        {
            PlayGamesPlatform.Instance.ManuallyAuthenticate(status =>
            {
                Debug.LogWarning("[Google Play Games] Manual Sign In: " + status);
                OnAuthenticated?.Invoke(status == SignInStatus.Success);
                _onSyncCompleted?.Invoke(); // Notify when authentication is complete
                if (status == SignInStatus.Success)
                {
                    Debug.LogWarning("[Google Play Games] Signed In Successfully!");
                }
                else
                {
                    Debug.LogWarning("[Google Play Games] Signed In Failed.");
                }
            });
        }

        public void Authenticate(Action callback, bool isManual)
        {
            _onSyncCompleted = callback;
            if (isManual)
                ManualAuthenticate();
            else
                Authenticate();
        }

        public bool IsAvailable()
        {
            return Social.localUser.authenticated;
        }

        /// <summary>
        /// Saves game data as a JSON string using Newtonsoft.Json.
        /// </summary>
        public void SaveData(CloudSaveData save)
        {
            if (!PlayGamesPlatform.Instance.localUser.authenticated)
            {
                Debug.LogWarning("User not authenticated, cannot save game.");
                return;
            }

            string jsonData = CloudSaveData.ConvertDataToJson(save); // Convert game state to JSON
            _cachedData = jsonData; // Cache locally

            byte[] data = Encoding.UTF8.GetBytes(jsonData);
            PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(
                SAVE_FILENAME,
                GooglePlayGames.BasicApi.DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseMostRecentlySaved,
                (status, metadata) =>
                {
                    if (status == SavedGameRequestStatus.Success)
                    {
                        _savedGameMetadata = metadata;
                        SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();
                        ((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(metadata, update, data,
                            (commitStatus, saveMetadata) =>
                            {
                                Debug.Log(commitStatus == SavedGameRequestStatus.Success
                                    ? "Game Saved Successfully!"
                                    : "Save Failed!");
                                Debug.Log($"Save Metadata: {saveMetadata.LastModifiedTimestamp}");
                            });
                    }
                    else
                    {
                        Debug.LogWarning("Failed to open save file.");
                    }
                });
        }

        /// <summary>
        /// Loads game data from Google Play Games Cloud Save.
        /// </summary>
        public void LoadData(Action<string> onLoadCompleted)
        {
            if (!PlayGamesPlatform.Instance.localUser.authenticated)
            {
                Debug.LogWarning("User not authenticated, cannot load game.");
                return;
            }

            PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(
                SAVE_FILENAME,
                GooglePlayGames.BasicApi.DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseMostRecentlySaved,
                (status, metadata) =>
                {
                    if (status == SavedGameRequestStatus.Success)
                    {
                        ((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(metadata, (readStatus, data) =>
                        {
                            if (readStatus == SavedGameRequestStatus.Success)
                            {
                                string jsonData = Encoding.UTF8.GetString(data);
                                onLoadCompleted?.Invoke(jsonData); // Return loaded data
                                // _cachedData = jsonData; // Cache loaded data
                                Debug.Log("Game Loaded  Successfully!");
                            }
                            else
                            {
                                Debug.LogWarning("Failed to read save data.");
                            }
                        });
                    }
                    else
                    {
                        Debug.LogWarning("Failed to open save file.");
                    }
                });
        }

        public void ClearData(List<string> keys)
        {
            if (!Social.localUser.authenticated)
            {
                Debug.LogWarning("User not authenticated, cannot clear game data.");
                return;
            }

            if (_savedGameMetadata == null)
            {
                Debug.LogWarning("No saved game metadata found.");
                return;
            }

            ((PlayGamesPlatform)Social.Active).SavedGame.Delete(_savedGameMetadata);
        }

        public void OnSyncComplete(Action callback)
        {
            _onSyncCompleted = callback;
        }
    }
}
#endif