using UnityEngine;

namespace IEC
{
    [ExecuteAlways]
    public class ConditionalSelectionBase : MonoBehaviour
    {
#if UNITY_EDITOR
        private void OnEnable()
        {
            if (Application.IsPlaying(gameObject)) return;
            UpdateSelectionBaseAttribute();
        }

        private void UpdateSelectionBaseAttribute()
        {
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
                if (GetComponent<SelectionBaseHolder>() == null)
                {
                    gameObject.AddComponent<SelectionBaseHolder>();
                }

                // foreach (Transform child in transform)
                // {
                //     child.gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.NotEditable;
                // }
            }
        }
#endif
    }
}