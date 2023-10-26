using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace IEC.OptimizeTexture
{
    internal static class OTUtils
    {
        #region Constant

        private const string PLUGIN_FOLDER_PATH = "Tools/Optimize Texture/";

        private const string HOTKEY_9_SLICES = " &q";
        private const string HOTKEY_SCALE_TO_DIV4 = " &s";
        private const string HOTKEY_RESIZE_TO_DIV4 = " &e";

        internal const string MENU_ITEM_9_SLICES = PLUGIN_FOLDER_PATH + "9-Slices" + HOTKEY_9_SLICES;
        internal const string MENU_ITEM_SCALE_TO_DIV4 = PLUGIN_FOLDER_PATH + "Scale to Div 4" + HOTKEY_SCALE_TO_DIV4;
        internal const string MENU_ITEM_RESIZE_TO_DIV4 = PLUGIN_FOLDER_PATH + "Resize to Div 4" + HOTKEY_RESIZE_TO_DIV4;
        internal const string MENU_ITEM_TRIM_TEXTURE = PLUGIN_FOLDER_PATH + "Trim Texture";

        internal const string MENU_ITEM_IS_AUTO_TRIM_CHECK = PLUGIN_FOLDER_PATH + "Is Auto Trim";
        private const string MENU_ITEM_IS_DEBUG_CHECK = PLUGIN_FOLDER_PATH + "Is Debug";
        
        private const string KEY_IS_AUTO_TRIM = "optimize_texture_isAutoTrim";
        private const string KEY_IS_DEBUG = "optimize_texture_isDebug";

        #endregion

        #region Editor Prefs
        internal static bool IsAutoTrim
        {
            get => EditorPrefs.GetBool(KEY_IS_AUTO_TRIM, false);
            set
            {
                EditorPrefs.SetBool(KEY_IS_AUTO_TRIM, value);
                Menu.SetChecked(MENU_ITEM_IS_AUTO_TRIM_CHECK, value);
            }
        }
        
        private static bool IsDebug
        {
            get => EditorPrefs.GetBool(KEY_IS_DEBUG, false);
            set
            {
                EditorPrefs.SetBool(KEY_IS_DEBUG, value);
                Menu.SetChecked(MENU_ITEM_IS_DEBUG_CHECK, value);
            }
        }

        static OTUtils()
        {
            EditorApplication.delayCall += () =>
            {
                Menu.SetChecked(MENU_ITEM_IS_AUTO_TRIM_CHECK, IsAutoTrim);
                Menu.SetChecked(MENU_ITEM_IS_DEBUG_CHECK, IsDebug);
            };
        }

        #region Menu Item

        [MenuItem(MENU_ITEM_IS_DEBUG_CHECK)]
        private static void ToggleIsDebug() => IsDebug = !IsDebug;

        #endregion
        

        #endregion
        public static void TransformSelectedTextures(Func<Texture2D, TextureImporter, Texture2D> transform)
        {
            foreach (var obj in Selection.objects)
            {
                var texture = obj as Texture2D;
                if (texture == null) continue;
                
                var path = AssetDatabase.GetAssetPath(obj);
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                var textureName = texture.name;

                var readableStatus = importer.isReadable;
                importer.isReadable = true;
                importer.SaveAndReimport();
                texture = transform(texture, importer);
                
                SaveTexture(texture, path);

                importer.isReadable = readableStatus;
                importer.SaveAndReimport();
                
                if (IsDebug)
                    Debug.Log($"<color=green><b>Complete Optimize:</b></color> {textureName}");
            }
        }
        
        private static void SaveTexture(Texture2D texture, string filePath)
        {
            File.WriteAllBytes(filePath, texture.EncodeToPNG());
        }
    }
}

