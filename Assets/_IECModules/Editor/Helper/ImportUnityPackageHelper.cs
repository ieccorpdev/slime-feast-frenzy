using System;
using System.IO;
using System.Linq;
using BestHTTP;
using ICSharpCode.SharpZipLib.Zip;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace _IECModules
{
    [InitializeOnLoad]
    public static class ImportUnityPackageHelper
    {

        static ImportUnityPackageHelper()
        {
            Debug.Log("Init asset database callback");
            AssetDatabase.importPackageStarted += ImportPackageStart;
            AssetDatabase.importPackageCompleted += ImportPackageComplete;
        }

        public static void DownloadPackage(string url, Action onComplete = null)
        {
            LogEditorHelper.AddLog($"Start Download SDK: {url}");

            HTTPManager.Setup();
            var httpRequest = new HTTPRequest(new Uri(url), (originalRequest, response) =>
            {
                EditorUtility.ClearProgressBar();
                onComplete?.Invoke();

                LogEditorHelper.AddLog($"Download SDK Complete: {BytesToString(response.Data.Length)}");
                // using (var fs = File.Create("Assets/facebook.zip")) 
                // {
                //     fs.Write(response.Data, 0, response.Data.Length);
                // }

                // var byteArray = File.ReadAllBytes("Assets/facebook.zip");
                // LogEditorHelper.AddLog($"Assets/facebook.zip: {byteArray.Length} bytes");

                if (url.EndsWith(".unitypackage"))
                {
                    ImportPackage(response.Data, url);
                }
                else
                {
                    ZipUtils.GetZipFileFromByteArray(response.Data, zipFile =>
                    {
                        foreach (ZipEntry entry in zipFile)
                        {
                            if (entry.IsDirectory) continue;
                            if (!entry.Name.EndsWith(".unitypackage")) continue;
                            LogEditorHelper.AddLog(entry.Name);
                            var zippedStream = zipFile.GetInputStream(entry);
                            var fileByteArray = ZipUtils.ReadFully(zippedStream);

                            var filePath = $"{GetTempFolderPath()}{entry.Name.Split('/').Last()}";

                            LogEditorHelper.AddLog($"Start Unzip To: {filePath}");

                            ImportPackage(fileByteArray, entry.Name);

                            break;
                        }

                    });
                }
            }).Send();
            httpRequest.OnProgress += (request, downloaded, length) =>
            {
                EditorUtility.DisplayProgressBar("Downloading", $"{BytesToString(downloaded)}/{BytesToString(length)}", (float)downloaded / length);
            };
        }

        public static string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0" + suf[0];
            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num) + suf[place];
        }

        public static void ImportPackage(byte[] packageByteArray, string packageName)
        {
            var fileName = GetFileNameFromURL(packageName);
            var filePath = $"{GetTempFolderPath()}{fileName}";
            using (var fs = File.Create(filePath))
            {
                fs.Write(packageByteArray, 0, packageByteArray.Length);
            }
            AssetDatabase.ImportPackage(filePath, false);

        }

        private static string GetFileNameFromURL(string url)
        {
            var fileName = url.Split('/').Last();
            fileName = fileName.Split(new[] { "%2F" }, StringSplitOptions.None).Last();
            fileName = fileName.Split(new[] { "=" }, StringSplitOptions.None).Last();
            return fileName;
        }

        private static string GetTempFolderPath()
        {
            var consoleLogFile = new FileInfo(Application.consoleLogPath);
            var tempFolderPath = $"{consoleLogFile.Directory}/Temp/";
            Directory.CreateDirectory(tempFolderPath);
            return tempFolderPath;
        }

        public static void ImportPackageComplete(string packageName)
        {
            LogEditorHelper.AddLog($"Import Package Done: {packageName}");
            //Debug.Log("import package done");
            if (FileUtil.DeleteFileOrDirectory("Assets/ExternalDependencyManager"))
            {
                LogEditorHelper.AddLog("Delete EDM");
            }
            if (FileUtil.DeleteFileOrDirectory("Assets/PlayServicesResolver"))
            {
                LogEditorHelper.AddLog("Delete Play Service Resolver");
            }
            AssetDatabase.Refresh();

        }
        public static void ImportPackageStart(string packageName)
        {
            LogEditorHelper.AddLog($"Import Package Start: {packageName}");
        }


    }

    //[InitializeOnLoad]
    //public class PackageRegistrar : AssetPostprocessor
    //{
    //    static PackageRegistrar()
    //    {
    //        Debug.Log("PackageRegistrar: s_ctor");
    //        AssetDatabase.importPackageStarted += OnImportPackageStarted;
    //        AssetDatabase.importPackageCompleted += OnImportPackageCompleted;
    //    }

    //    public static void OnPostprocessAllAssets(
    //        string[] importedAssets
    //        , string[] deleted
    //        , string[] movedAssets
    //        , string[] movedFrom)
    //    {
    //        //LogIfAny("importedAssets", importedAssets);
    //        //LogIfAny("deleted", deleted);
    //        //LogIfAny("movedAssets", movedAssets);
    //        //LogIfAny("movedFrom", movedFrom);
    //    }

    //    private static void OnImportPackageStarted(string packagename)
    //    {
    //        Debug.Log($"PackageRegistrar: OnImportPackageStarted({packagename})");
    //    }

    //    private static void OnImportPackageCompleted(string packagename)
    //    {
    //        Debug.Log($"PackageRegistrar: OnImportPackageCompleted({packagename})");
    //    }
    //}
}