using Sirenix.OdinInspector;
using UnityEngine;

namespace IEC
{
    [ExecuteAlways]
    public class SpawnSettings : MonoBehaviour
    {
#if UNITY_EDITOR
        private Vector3 originalOffset;
        private Transform meshTransform;
        private Vector3 originalLocalPos;
        
        void OnEnable()
        {
            if (gameObject.scene.name == null) return;
            
            string currentScenePath = gameObject.scene.path;
            string currentSceneName = gameObject.scene.name;

            bool isLevelEditorScene = false;

            // foreach (string editorScene in LevelEditor.LEVEL_EDITOR_SCENES)
            // {
            //     if (currentScenePath.Contains(editorScene) || currentSceneName.Contains(editorScene))
            //     {
            //         isLevelEditorScene = true;
            //         break;
            //     }
            // }

            if (isLevelEditorScene)
            {
                InitializeMeshReference();
            }
        }

        private void InitializeMeshReference()
        {
            // Find the first active MeshRenderer in children
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in renderers)
            {
                if (renderer.enabled)
                {
                    meshTransform = renderer.transform;
                    originalOffset = meshTransform.position - transform.position;
                    originalLocalPos = meshTransform.localPosition;
                    break;
                }
            }

            if (meshTransform == null)
            {
                Debug.LogWarning("No active MeshRenderer found in children!");
            }
        }

        [Button]
        public void PreserveOffset()
        {
            if (meshTransform != null)
            {
                // Calculate the difference between current and original local position
                Vector3 localPosDelta = meshTransform.localPosition - originalLocalPos;
        
                // If there's a difference in local position
                if (localPosDelta != Vector3.zero)
                {
                    // Adjust parent position to maintain child's world position
                    Vector3 worldPosDelta = transform.rotation * localPosDelta;
                    transform.position += worldPosDelta;
            
                    // Reset child's local position to original
                    meshTransform.localPosition = originalLocalPos;
                }
            }
        }

        // Get the mesh transform reference
        public Transform GetMeshTransform()
        {
            return meshTransform;
        }

        // Get the original offset
        public Vector3 GetOriginalOffset()
        {
            return originalOffset;
        }

        // This will run in the editor even when not in play mode
        void OnValidate()
        {
            // Reinitialize mesh reference if not set
            if (meshTransform == null)
            {
                InitializeMeshReference();
            }
            else
            {
                // Update offset if mesh still exists
                originalOffset = meshTransform.position - transform.position;
                //PreserveOffset();
            }
        }

        // Optional: Add this if you want to track when the mesh is enabled/disabled
        void OnTransformChildrenChanged()
        {
            InitializeMeshReference();
        }
#endif
    }
}
