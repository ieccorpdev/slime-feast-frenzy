using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace IEC
{
	[Serializable]
	public class SerializableScenesContainer : ScriptableObject
	{
		// [HideInInspector]
		public bool activeSceneSwitched = false;
		public List<SceneAsset> SceneAssets;
		public List<GameObject> PrefabAsset;
	}
}
#endif