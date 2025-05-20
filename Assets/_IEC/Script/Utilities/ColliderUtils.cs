using UnityEngine;

public static class ColliderUtils
{
    /// <summary>
    /// Clone một Collider từ sourceCollider sang destinationObject.
    /// Hỗ trợ Box, Sphere, Capsule, Mesh Colliders.
    /// </summary>
    public static Collider CopyCollider(Collider sourceCollider, GameObject destinationObject)
    {
        if (sourceCollider == null || destinationObject == null)
        {
            Debug.LogError("Source collider or destination object is null.");
            return null;
        }

        // Xác định kiểu collider gốc
        System.Type type = sourceCollider.GetType();
        // Tạo component mới cùng loại trên destinationObject
        Collider newCollider = (Collider)destinationObject.AddComponent(type);

        // Copy các thuộc tính chung
        newCollider.isTrigger    = sourceCollider.isTrigger;
        newCollider.material     = sourceCollider.material;
        newCollider.enabled      = sourceCollider.enabled;
        newCollider.sharedMaterial = sourceCollider.sharedMaterial;

        // Copy riêng theo từng loại
        if (newCollider is BoxCollider boxNew && sourceCollider is BoxCollider boxSrc)
        {
            boxNew.center = boxSrc.center;
            boxNew.size   = boxSrc.size;
        }
        else if (newCollider is SphereCollider sphereNew && sourceCollider is SphereCollider sphereSrc)
        {
            sphereNew.center = sphereSrc.center;
            sphereNew.radius = sphereSrc.radius;
        }
        else if (newCollider is CapsuleCollider capNew && sourceCollider is CapsuleCollider capSrc)
        {
            capNew.center   = capSrc.center;
            capNew.radius   = capSrc.radius;
            capNew.height   = capSrc.height;
            capNew.direction= capSrc.direction;
        }
        else if (newCollider is MeshCollider meshNew && sourceCollider is MeshCollider meshSrc)
        {
            meshNew.sharedMesh = meshSrc.sharedMesh;
            meshNew.convex     = meshSrc.convex;
            meshNew.inflateMesh = meshSrc.inflateMesh;
            meshNew.skinWidth   = meshSrc.skinWidth;
        }
        else
        {
            Debug.LogWarning($"CopyCollider: Collider type {type.Name} chưa được hỗ trợ đầy đủ, chỉ copy các thuộc tính chung.");
        }

        return newCollider;
    }
}
