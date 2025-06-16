// Pool Floaties from Luceed Studio - https://luceed.studio

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LuceedStudio_PoolFloaties
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Buoyancy))]
    public class BuoyancyEditor : Editor
    {
        private SerializedObject so;
        private Buoyancy buoyancy;
        private List<Buoyancy> buoyancys = new List<Buoyancy>();

        private SerializedProperty propTriggerOnStart;

        private SerializedProperty propGlobalAmplitude;
        private SerializedProperty propGlobalSpeed;

        private SerializedProperty propUseX;
        private SerializedProperty propXAmplitudeRange;
        private SerializedProperty propXDurationRange;

        private SerializedProperty propUseZ;
        private SerializedProperty propZAmplitudeRange;
        private SerializedProperty propZDurationRange;

        private void OnEnable()
        {
            so = serializedObject;

            buoyancy = target as Buoyancy;
            buoyancys.Clear();
            for (int i = 0; i < targets.Length; i++)
            {
                buoyancys.Add(targets[i] as Buoyancy);
            }

            propTriggerOnStart = so.FindProperty("triggerOnStart");

            propGlobalAmplitude = so.FindProperty("globalAmplitude");
            propGlobalSpeed = so.FindProperty("globalSpeed");

            propUseX = so.FindProperty("useX");
            propXAmplitudeRange = so.FindProperty("xAmplitudeRange");
            propXDurationRange = so.FindProperty("xDurationRange");

            propUseZ = so.FindProperty("useZ");
            propZAmplitudeRange = so.FindProperty("zAmplitudeRange");
            propZDurationRange = so.FindProperty("zDurationRange");
        }

        public override void OnInspectorGUI()
        {
            so.Update();

            EditorGUILayout.LabelField("Buoyancy", EditorStyles.boldLabel);
            DrawUILine();

            EditorGUILayout.PropertyField(propTriggerOnStart);

            GUILayout.Space(10);
            DrawUILine();

            EditorGUILayout.PropertyField(propGlobalAmplitude);
            EditorGUILayout.PropertyField(propGlobalSpeed);

            EditorGUILayout.PropertyField(propUseX);
            if (propUseX.boolValue)
            {
                EditorGUILayout.PropertyField(propXAmplitudeRange);
                EditorGUILayout.PropertyField(propXDurationRange);
            }

            EditorGUILayout.PropertyField(propUseZ);
            if (propUseZ.boolValue)
            {
                EditorGUILayout.PropertyField(propZAmplitudeRange);
                EditorGUILayout.PropertyField(propZDurationRange);
            }

            GUILayout.Space(10);

            DrawUILine();
            EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);

            if (!Application.isPlaying)
            {
                GUI.enabled = false;
            }

            bool isUpdating = buoyancy.IsUpdating;
            GUIContent buttonContent = new GUIContent();
            buttonContent.text = isUpdating ? "Disable Buoyancy" : "Enable Buoyancy";
            if (GUILayout.Button(buttonContent))
            {
                if (Application.isPlaying)
                {
                    isUpdating = !isUpdating;
                    for (int i = 0; i < buoyancys.Count; i++)
                    {
                        buoyancys[i].UpdateState(isUpdating);
                    }
                }
            }

            GUI.enabled = true;

            so.ApplyModifiedProperties();
        }

        private void DrawUILine()
        {
            Rect separator = EditorGUILayout.GetControlRect(false, GUILayout.Height(10 + 1));
            separator.height = 1;
            separator.y += 10 * 0.5f;
            separator.x = 0;
            separator.width = EditorGUIUtility.currentViewWidth;
            EditorGUI.DrawRect(separator, Color.grey);
        }
    }
}

