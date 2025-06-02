#if UNITY_IOS
using System;
using System.Collections.Generic;
using System.Text;
using Apple.Core.Runtime;
using Apple.GameKit;
using Cysharp.Threading.Tasks;
using FancyHole.Features.CloudSave;
using FancyHole.Linq;
using UnityEngine;

namespace FancyHole.SkipRename
{
    public class AppleGameCenterCloudSaveModule : ICloudSaveModule
    {
        public string UserId => GKLocalPlayer.Local.IsAuthenticated ? GKLocalPlayer.Local.GamePlayerId : "0";
        
        private Action _onSyncComplete;
        public Action<bool> OnAuthenticated { get; set; }

        public void Init()
        {
            GKLocalPlayer.AuthenticateUpdate += OnAuthenticateUpdate;
            GKLocalPlayer.AuthenticateError += OnAuthenticateError;
        }

        private void OnAuthenticateUpdate(GKLocalPlayer localplayer)
        {
            Debug.LogWarning($"GameKit Authentication: player {localplayer}");
            if (localplayer.IsAuthenticated)
            {
                Debug.LogWarning($"GameKit Authentication: player {localplayer.DisplayName} + {localplayer.GamePlayerId}");
            }
            else
            {
                Debug.LogError("GameKit Authentication failed");
            }
        }

        private void OnAuthenticateError(NSError error)
        {
            Debug.LogError ($"GameKit Authentication Error: {error.LocalizedDescription} + {error.UserInfo.JsonSerialize()}");
        }

        public async void Authenticate()
        {
            if (GKLocalPlayer.Local.IsAuthenticated)
            {
                _onSyncComplete?.Invoke();
                return;
            }

            try
            {
                var player = await GKLocalPlayer.Authenticate();
                Debug.LogWarning($"GameKit Authentication: player {player}");
            
                await UniTask.Delay(1000);
                _onSyncComplete?.Invoke();
                OnAuthenticated?.Invoke(player != null && player.IsAuthenticated);
                
                if (player == null || !player.IsAuthenticated)
                {
                    Debug.LogError("GameKit Authentication failed");
                    return;
                }

                // Grab the display name.
                Debug.LogWarning($"GameKit Authentication: player {GKLocalPlayer.Local.DisplayName} + {player.GamePlayerId}");
            }
            catch (Exception e)
            {
                Debug.LogError($"GameKit Authentication Error: {e.Message}");
                
                await UniTask.Delay(1000);
                _onSyncComplete?.Invoke();
                OnAuthenticated?.Invoke(GKLocalPlayer.Local != null && GKLocalPlayer.Local.IsAuthenticated); // not false because can still get userId when offline
            }
        }

        public void Authenticate(Action callback, bool isManual)
        {
            _onSyncComplete = callback;
            Authenticate();
        }

        public bool IsAvailable()
        {
            return GKLocalPlayer.Local != null && GKLocalPlayer.Local.IsAuthenticated;
        }

        public async void SaveData(CloudSaveData save)
        {
            if (!GKLocalPlayer.Local.IsAuthenticated)
            {
                Debug.LogWarning("User not authenticated, cannot save game.");
                return;
            }

            var json = CloudSaveData.ConvertDataToJson(save);
            var data = Encoding.UTF8.GetBytes(json);
            var nsData = new NSData(data);

            var savedGame = await GKLocalPlayer.Local.SaveGameData(nsData, CloudSaveController.CLOUD_SAVE_KEY);
            if (savedGame == null)
            {
                Debug.LogError("Failed to save game data.");
                return;
            }
            
            Debug.LogWarning($"Game data saved successfully: {savedGame}");
        }

        public async void LoadData(Action<string> onLoadCompleted)
        {
            if (!GKLocalPlayer.Local.IsAuthenticated)
            {
                Debug.LogWarning("User not authenticated, cannot load game.");
                return;
            }
            
            var savedGames = await GKLocalPlayer.Local.FetchSavedGames();
            
            if (savedGames == null || savedGames.Count == 0)
            {
                Debug.LogError("No saved games found.");
                onLoadCompleted?.Invoke(null);
                return;
            }
            
            var savedGame = savedGames.Find(x => x.Name == CloudSaveController.CLOUD_SAVE_KEY);
            if (savedGame == null)
            {
                Debug.LogError("No saved game found with the specified key. Fallback to first save game");
                savedGame = savedGames[0]; // Fallback to the first saved game                
            }
            
            var data = await savedGame.LoadData();
            if (data == null)
            {
                Debug.LogError("Failed to load game data.");
                onLoadCompleted?.Invoke(null);
                return;
            }
            
            var json = Encoding.UTF8.GetString(data.Bytes);
            Debug.LogWarning($"Game data loaded successfully: {json}");
            onLoadCompleted?.Invoke(json);
        }


        public void ClearData(List<string> keys)
        {
            if (!GKLocalPlayer.Local.IsAuthenticated)
            {
                Debug.LogWarning("User not authenticated, cannot clear game data.");
                return;
            }

            foreach (var key in keys)
            {
                GKLocalPlayer.Local.DeleteSavedGames(CloudSaveController.CLOUD_SAVE_KEY);
            }
        }

        public void OnSyncComplete(Action callback)
        {
            _onSyncComplete = callback;
        }
    }
}
#endif