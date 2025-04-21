using UnityEngine;

[ExecuteAlways] // Ensures it runs in both Play Mode and Edit Mode
[RequireComponent(typeof(Renderer), typeof(MeshFilter))]
public class FillShaderEditor : MonoBehaviour
{
    private Renderer rend;
    private Material material;
    private Mesh mesh;
    
    private float minHeight;
    private float maxHeight;
    
    private MaterialPropertyBlock mpb;

    private bool _isAnim = false;

    void Awake()
    {
        UpdateShaderBounds();

        var islandItem = transform.parent.GetComponent<IslandItem>();
        if (islandItem == null)
            islandItem = GetComponent<IslandItem>();
        
        if (islandItem != null && islandItem.animItem != null)
        {
            _isAnim = true;
        }
        
        rend = GetComponent<Renderer>();


        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
            mesh = meshFilter.sharedMesh;
    }

    void Update()
    {
        // Automatically update in editor
        //if (!Application.isPlaying)
        if (!_isAnim)
            return;

        if (!rend)
            return;

        if (!rend.isVisible)
            return;
        
        UpdateShaderBounds();
    }
    //
    void OnValidate()
    {
        // Ensure values update when properties are changed
        UpdateShaderBounds();
    }
    
    bool IsFullyFilled()
    {
        // Fetch current fill amount from the material
        if (mpb == null) return false;
        float fillAmount;
        rend.GetPropertyBlock(mpb);
        fillAmount = mpb.GetFloat("_FillAmount");

        // Check if fill height reaches max height
        float fillHeight = Mathf.Lerp(minHeight, maxHeight, fillAmount);
        return fillHeight >= maxHeight;
    }
    
    void UpdateShaderBounds()
    {
        if (BaseShaderBound(out var localUp)) return;

        // Use MaterialPropertyBlock to avoid creating new materials
        mpb = new MaterialPropertyBlock();
        rend.GetPropertyBlock(mpb);
        mpb.SetVector("_LocalUp", localUp);
        mpb.SetFloat("_MinHeight", minHeight);
        mpb.SetFloat("_MaxHeight", maxHeight);
        rend.SetPropertyBlock(mpb);
    }

    private bool BaseShaderBound(out Vector3 localUp)
    {
        localUp = Vector3.up;
        if (!rend) return true;
        if (!rend.sharedMaterial) return true;

        if (!mesh) return true;

        if (IsFullyFilled())
            return true;

        material = rend.sharedMaterial;

        // Compute true local up vector based on world up
        localUp = transform.InverseTransformDirection(Vector3.up).normalized;
        material.SetVector("_LocalUp", localUp);

        // Find actual min/max along the local up axis by iterating all vertices
        Vector3[] vertices = mesh.vertices;
        minHeight = float.MaxValue;
        maxHeight = float.MinValue;

        int vertexCount = vertices.Length;

        for (int i = 0; i < vertexCount; i++)
        {
            float height = Vector3.Dot(vertices[i], localUp);
            if (height < minHeight) minHeight = height;
            if (height > maxHeight) maxHeight = height;
        }

        // Ensure minHeight is smaller than maxHeight to prevent fill inversion
        if (minHeight > maxHeight)
        {
            (minHeight, maxHeight) = (maxHeight, minHeight);
        }

        // Add a slight offset to ensure full fill
        float offset = (maxHeight - minHeight) * 0.01f; // 1% of the object's height
        maxHeight += offset;
        return false;
    }

    void SetShareShaderBound()
    {
        if (BaseShaderBound(out var localUp)) return;
        
        //Set Direction to shader base
        rend.sharedMaterial.SetVector("_LocalUp", localUp);
        rend.sharedMaterial.SetFloat("_MinHeight", minHeight);
        rend.sharedMaterial.SetFloat("_MaxHeight", maxHeight);
    }
}