using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IEC
{
    public enum TimeFormat
    {
        FullTime,            // "00h 00' 00''"
        PlainSeconds,        // "seconds as a plain integer"
        MinutesSeconds,      // "00:00"
        HoursMinutesSeconds,  // "00:00:00"
        SpacedMinutesSeconds, // "00 : 00"
        DetailSpacedHoursMinutesSeconds // "00h 00h 00s"
    }
    
    public static class Utilities
    {
        public static Vector2 ConvertWorldToRectTransform(Canvas canvas, Camera cam, Vector3 worldPosition)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
                cam.WorldToScreenPoint(worldPosition), canvas.worldCamera, out var rectTransformPosition);
            return rectTransformPosition;
        }
        
        private static readonly Dictionary<(float, TimeFormat), string> CachedTime = new Dictionary<(float, TimeFormat), string>();

        public static string GetCachedCountdownTime(float countdownTime, TimeFormat format)
        {
            if (CachedTime.TryGetValue((countdownTime, format), out var cachedTime))
            {
                return cachedTime;
            }

            var time = TimeSpan.FromSeconds(countdownTime);

            string timeString;
            switch (format)
            {
                case TimeFormat.FullTime:
                    timeString = $"{time.Hours:D2}h {time.Minutes:D2}' {time.Seconds:D2}''";
                    break;
                case TimeFormat.PlainSeconds:
                    timeString = countdownTime.ToString(CultureInfo.InvariantCulture);
                    break;
                case TimeFormat.MinutesSeconds:
                    timeString = $"{(int)time.TotalMinutes:D2}:{time.Seconds:D2}";
                    break;
                case TimeFormat.HoursMinutesSeconds:
                    timeString = $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
                    break;
                case TimeFormat.SpacedMinutesSeconds:
                    timeString = $"{(int)time.TotalMinutes:D2} : {time.Seconds:D2}";
                    break;
                case TimeFormat.DetailSpacedHoursMinutesSeconds:
                    timeString = $"{time.Hours:D2}h {time.Minutes:D2}m {time.Seconds:D2}s";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), "Unsupported time format");
            }

            CachedTime.Add((countdownTime, format), timeString);
            return timeString;
        }
        
        // public static void Smoothing(MaskableGraphic graphic, float smooth)
        // {
        //     if (!graphic) return;
        //
        //     var canvasRenderer = graphic.canvasRenderer;
        //     var currentColor = canvasRenderer.GetColor();
        //     var targetAlpha = 1f;
        //     if (graphic.maskable && 0 < smooth)
        //     {
        //         var currentAlpha = graphic.color.a * canvasRenderer.GetInheritedAlpha();
        //         if (0 < currentAlpha)
        //         {
        //             targetAlpha = Mathf.Lerp(0.01f, 0.002f, smooth) / currentAlpha;
        //         }
        //     }
        //
        //     if (Mathf.Approximately(currentColor.a, targetAlpha)) return;
        //     currentColor.a = Mathf.Clamp01(targetAlpha);
        //     canvasRenderer.SetColor(currentColor);
        // }

        public static List<T> FindAllAssetsInFolder<T>(string folderPath) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (!folderPath.StartsWith("Assets/"))
            {
                folderPath = "Assets/" + folderPath;
            }
            
            folderPath = folderPath.TrimEnd('/');
            
            string[] guids = AssetDatabase.FindAssets("t:"+typeof(T).Name, new[] { folderPath });
            
            List<T> assets = new List<T>();
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                assets.Add(asset);
            }
            
            return assets;
#endif
            return null;
        }
        

        public static T FindAssetInFolder<T>(string folderPath, string itemName) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (!folderPath.StartsWith("Assets/"))
            {
                folderPath = "Assets/" + folderPath;
            }
            
            folderPath = folderPath.TrimEnd('/');
            
            string[] guids = AssetDatabase.FindAssets("t:"+typeof(T).Name, new[] { folderPath });
            
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(assetPath);
                if (fileName == itemName)
                {
                    return AssetDatabase.LoadAssetAtPath<T>(assetPath);
                }
            }
            
            Debug.LogWarning($"Missing item of type {typeof(T)} with name {itemName}");
            return null;
#else
            return null;
#endif
        }
        
        public static T FindAssetInFolder<T>(string folderPath) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (!folderPath.StartsWith("Assets/"))
            {
                folderPath = "Assets/" + folderPath;
            }
            
            folderPath = folderPath.TrimEnd('/');
            
            string[] guids = AssetDatabase.FindAssets("t:"+typeof(T).Name, new[] { folderPath });
            
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(assetPath);
                
                // Check if the filename matches our search
                return AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
            
            return null;
#else
            Debug.LogWarning("FindImageInFolder can only be used in the Unity Editor");
            return null;
#endif
        }
        
        public static List<T> FindAllAssetsOfType<T>() where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            var folderPath = "Assets";
            
            string[] guids = AssetDatabase.FindAssets("t:"+typeof(T).Name, new[] { folderPath });

            var result = new List<T>();
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                
                // Check if the filename matches our search
                var item = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                result.Add(item);
            }

            return result;
#else
            Debug.LogWarning("FindImageInFolder can only be used in the Unity Editor");
            return null;
#endif
        }
        
        public static GameObject FindPrefabInFolder(string folderPath, string itemName)
        {
#if UNITY_EDITOR
            if (!folderPath.StartsWith("Assets/"))
            {
                folderPath = "Assets/" + folderPath;
            }
            
            folderPath = folderPath.TrimEnd('/');
            
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
            
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(assetPath);
                
                // Check if the filename matches our search
                if (fileName == itemName)
                {
                    return AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                }
            }
            
            Debug.LogWarning($"Missing item of type {typeof(GameObject)} with name {itemName}");
            return null;
#else
            Debug.LogWarning("FindImageInFolder can only be used in the Unity Editor");
            return null;
#endif
        }
        
        public static void MoveDirectChildren(GameObject sourceParent, GameObject newParent, bool preserveWorldPosition = true)
        {
            if (sourceParent == null || newParent == null)
            {
                Debug.LogError("Source or target parent is null!");
                return;
            }

            // Store the children first because changing parents will affect the hierarchy
            List<Transform> directChildren = new List<Transform>();
            foreach (Transform child in sourceParent.transform)
            {
                directChildren.Add(child);
            }

            // Move each direct child to the new parent
            // Their own children will automatically move with them
            foreach (Transform child in directChildren)
            {
                child.SetParent(newParent.transform, preserveWorldPosition);
            }
        }
    }
}

