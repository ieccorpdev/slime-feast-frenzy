using System;
using UnityEngine;

namespace IEC
{
	[ExecuteAlways]
	public class GroundElement : MonoBehaviour
	{
		public string Id;

		private void OnEnable()
		{
//			if (Application.IsPlaying(gameObject)) return;
			HideSelectionInLevelEditor();
		}

		void HideSelectionInLevelEditor()
		{
#if UNITY_EDITOR
			SetHideFlagsRecursively(gameObject, HideFlags.HideInHierarchy | HideFlags.NotEditable);
			return;

			string currentScenePath = gameObject.scene.path;
			string currentSceneName = gameObject.scene.name;

			bool isLevelEditorScene = false;

			foreach (string editorScene in LevelEditor.LEVEL_EDITOR_SCENES)
			{
				if (currentScenePath.Contains(editorScene) || currentSceneName.Contains(editorScene))
				{
					isLevelEditorScene = true;
					break;
				}
			}

			if (isLevelEditorScene)
			{
				gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.NotEditable;
				foreach (Transform child in transform)
				{
					child.gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.NotEditable;
				}
			}
#endif

		}
		
		private void SetHideFlagsRecursively(GameObject obj, HideFlags flags)
		{
			if (obj == null) return;

			obj.hideFlags = flags;

			for (int i = 0; i < obj.transform.childCount; i++)
			{
				Transform childTransform = obj.transform.GetChild(i);
				SetHideFlagsRecursively(childTransform.gameObject, flags);
			}
		}
	}
}
