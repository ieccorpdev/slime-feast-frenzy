using Sirenix.OdinInspector;
using UnityEngine;

namespace IEC
{
    public class ScaleUtilities : _BaseBehaviour
    {
        public bool isAutoScale;
        public float reductionFactor = 1f;
        public MeshRenderer targetObject;
        public MeshRenderer objectToScale;

        private void OnValidate()
        {
            // if (!isAutoScale || (objectToScale != null && targetObject != null)) return;
            // var childObjects = gameObject.GetComponentsInChildren<MeshRenderer>(true);
            // targetObject = childObjects[0];
            // objectToScale = childObjects[1];
            // AdjustObjects();
            // AddMeshCollider();
        }

        [Button]
        public void AdjustObjects()
        {
            AdjustObjects(objectToScale, targetObject);
        }

        public void AdjustObjectsPreserveY()
        {
            AdjustObjectsPreserveY(objectToScale, targetObject);
        }
        
        void AdjustObjectsPreserveY(MeshRenderer obj, MeshRenderer target)
        {
            if (obj == null || target == null)
            {
                Debug.LogWarning("One of the objects is missing.");
                return;
            }

            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            Vector3 objSize = obj.bounds.size;
            Vector3 targetSize = target.bounds.size;

            // Calculate uniform scale based on X and Z
            float xRatio = targetSize.x / objSize.x;
            float zRatio = targetSize.z / objSize.z;
            float uniformScale = Mathf.Min(xRatio, zRatio) * reductionFactor;

            // Use the same scale for all axes to maintain proportions
            Vector3 scale = new Vector3(uniformScale, uniformScale, uniformScale);
            obj.transform.localScale = scale;

            // First align centers on X and Z
            obj.transform.position = new Vector3(
                target.transform.position.x,
                obj.transform.position.y, // Keep Y for now
                target.transform.position.z
            );

            // Now adjust Y position to match bottom bounds exactly
            float targetBottom = target.bounds.min.y;
            float objBottom = obj.bounds.min.y;
            float yOffset = targetBottom - objBottom;
    
            // Apply the Y position adjustment
            obj.transform.position = new Vector3(
                obj.transform.position.x,
                obj.transform.position.y + yOffset,
                obj.transform.position.z
            );
        }
        
        [Button]
        public void AddMeshCollider()
        {
            var meshColl = objectToScale.GetComponent<MeshCollider>();
            if (meshColl == null)
            {
                var meshCollider = objectToScale.gameObject.AddComponent<MeshCollider>();
                meshCollider.convex = true;
//                meshCollider.cookingOptions = MeshColliderCookingOptions.None;
            }
            else
            {
                meshColl.convex = true;
 //               meshColl.cookingOptions = MeshColliderCookingOptions.None;
            }
        }
        
        public void AdjustObjects(MeshRenderer obj, MeshRenderer target)
        {
            if (obj == null || target == null)
            {
                Debug.LogWarning("One of the objects is missing.");
                return;
            }

            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            Vector3 objSize = obj.bounds.size;
            Vector3 targetSize = target.bounds.size;

            // Calculate the scaling factor for each axis
            Vector3 scale = obj.transform.localScale;
            scale.x *= targetSize.x / objSize.x * reductionFactor;
            scale.y *= targetSize.y / objSize.y * reductionFactor;
            scale.z *= targetSize.z / objSize.z * reductionFactor;

            // Apply the new scale
            obj.transform.localScale = scale;

            // Calculate and apply the position adjustment
            Vector3 objBoundsCenterOffset = obj.bounds.center - obj.transform.position;
            Vector3 targetBoundsCenterOffset = target.bounds.center - target.transform.position;
            obj.transform.position = target.transform.position + (targetBoundsCenterOffset - objBoundsCenterOffset);
        }
    }
}