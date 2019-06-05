using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Root
{
    [CustomEditor(typeof(RemoteConfig))]
    public class CustomRemoteConfig : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var config = (RemoteConfig)target;

            config.key = EditorGUILayout.TextField("Key", config.key);
            config.type = (RemoteType)EditorGUILayout.EnumPopup("Type", config.type);

            switch (config.type)
            {
                case RemoteType.Bool:
                    config.boolDefault = EditorGUILayout.Toggle("Default", config.boolDefault);
                    config.boolTest = EditorGUILayout.Toggle("Test", config.boolTest);
                    EditorUtility.SetDirty(config);
                    break;
                case RemoteType.Int:
                    config.intDefault = EditorGUILayout.IntField("Default", config.intDefault);
                    config.intTest = EditorGUILayout.IntField("Test", config.intTest);
                    EditorUtility.SetDirty(config);
                    break;
                case RemoteType.String:
                    config.stringDefault = EditorGUILayout.TextField("Default", config.stringDefault);
                    config.stringTest = EditorGUILayout.TextField("Test", config.stringTest);
                    EditorUtility.SetDirty(config);
                    break;

            }
        }
    }
}
