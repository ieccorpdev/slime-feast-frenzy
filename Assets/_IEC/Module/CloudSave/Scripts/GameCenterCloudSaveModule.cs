// using System.Runtime.InteropServices;
// using UnityEngine;
// using VoxelBusters.EssentialKit;
//
// namespace FancyHole.Features.CloudSave
// {
//     public class GameCenterCloudSaveModule : ICloudSaveModule
//     {
//         // Native iOS calls
//         [DllImport("__Internal")]
//         private static extern void SaveToiCloud(string key, string value);
//
//         [DllImport("__Internal")]
//         private static extern string LoadFromiCloud(string key);
//
//         [DllImport("__Internal")]
//         private static extern bool iCloudAvailable();
//
//         public void Authenticate()
//         {
//         }
//
//         public void SaveData(CloudSaveData save)
//         {
//             
//         }
//
//         private void OnUserChange(CloudServicesUserChangeResult data)
//         {
//             
//         }
//
//         public string LoadData()
//         {
//             return null;
//         }
//     }
// }
