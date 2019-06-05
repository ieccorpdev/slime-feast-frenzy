using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Reflection;

[CustomEditor(typeof(MeshRenderer))]
public class MeshRenderOverrideEditor : Editor
{
    MeshRenderer renderer
    {
        get
        {
            return target as MeshRenderer;
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Custom Inspector", MessageType.Info);

        EditorGUI.BeginChangeCheck();

        var so = new SerializedObject(renderer);

        EditorGUILayout.PropertyField(so.FindProperty("m_CastShadows"));
        EditorGUILayout.PropertyField(so.FindProperty("m_ReceiveShadows"));
        EditorGUILayout.PropertyField(so.FindProperty("m_MotionVectors"));
        EditorGUILayout.PropertyField(so.FindProperty("m_Materials"), true);

        var options = GetSortingLayerNames();
        var picks = new int[options.Length];
        var name = renderer.sortingLayerName;
        var choice = -1;
        for (int i = 0; i < options.Length; i++)
        {
            picks[i] = i;
            if (name == options[i]) choice = i;
        }

        choice = EditorGUILayout.IntPopup("Sorting Layer", choice, options, picks);
        renderer.sortingLayerName = options[choice];
        renderer.sortingOrder = EditorGUILayout.IntField("Sorting Order", renderer.sortingOrder);

        renderer.lightProbeUsage = (LightProbeUsage)EditorGUILayout.EnumPopup("Ligth Probes", renderer.lightProbeUsage);
        renderer.reflectionProbeUsage = (ReflectionProbeUsage)EditorGUILayout.EnumPopup("Reflection Probes", renderer.reflectionProbeUsage);
        renderer.probeAnchor = EditorGUILayout.ObjectField("Anchor Override", renderer.probeAnchor, typeof(Transform), true) as Transform;

        if (EditorGUI.EndChangeCheck())
            SceneView.RepaintAll();
    }

    //void OnSceneGUI()
    //{
    //    var transform = renderer.transform;
    //    var mesh = renderer.GetComponent<MeshFilter>().sharedMesh;
    //    if (!mesh) return;

    //    var vertices = mesh.vertices;
    //    for (int i = 0; i < mesh.vertexCount; i++)
    //    {
    //        Handles.Label(transform.TransformPoint(vertices[i]), i.ToString());
    //    }
    //}

    public string[] GetSortingLayerNames()
    {
        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        return (string[])sortingLayersProperty.GetValue(null, new object[0]);
    }
}
