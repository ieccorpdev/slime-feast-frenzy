using System.Collections;
using System.Linq;
using _IEC.Features.Gameplay;
using IEC;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class CubeScreenshot : MonoBehaviour
{
    public Camera renderCamera;  // Camera that will capture the cube
    public RenderTexture renderTexture;  // Render texture to capture the image
    public GameObject cubePrefab;  // The cube prefab
    public Vector3 cubePosition = Vector3.zero;  // The position where the cube will be spawned
    private GameObject cube;  // Reference to the current cube

    [Header("MeoMEw")]
    public GameplayItemPrefabConfig itemConfig;
    public string itemID;
    public int startFrom = 0;

    private Coroutine _meoMeoPictureCoroutine;
    
    [Button]
    void MeoMeoScreenShot()
    {
        if(_meoMeoPictureCoroutine != null)
            StopCoroutine(_meoMeoPictureCoroutine);
        _meoMeoPictureCoroutine = StartCoroutine(TakePictureAllItem());
    }

    [Button]
    void SetIconAllItem()
    {
        foreach (var item in itemConfig.listItemDefinitionShp)
        {
            var icon = ImportPngAsSprite("Assets/New Item Icon/" + item.GetId() + ".png");
            if (icon != null)
            {
                item.SetIcon(icon);
                EditorUtility.SetDirty(item);
                AssetDatabase.SaveAssets();
            }
        }
    }

    void Start()
    {
        StartCoroutine(TakePictureAllItem());
    }

    public IEnumerator TakePictureAllItem()
    {
        if (!string.IsNullOrEmpty(itemID))
        {
            yield return DoScreenShot(itemConfig.listItemDefinitionShp.Find(item => item.GetId().Equals(itemID)));
        }
        else
        {
            for (int i = startFrom; i < itemConfig.listItemDefinitionShp.Count; i++)
            {
                // Wait until the end of the frame to allow cube destruction and instantiation
                yield return new WaitForEndOfFrame();

                // Get item data for this iteration
                var itemData = itemConfig.listItemDefinitionShp[i];

                if (itemData == null || itemData.GetPrefab() == null)
                    continue;

                yield return DoScreenShot(itemData);

                // // Spawn the new cube
                // SpawnCube(itemData.GetPrefab());
                //
                // // Adjust the camera size based on the cube's collider bounds
                // AdjustCameraSize();
                //
                // // Set camera rotation to (45, 0, 0)
                // SetCameraRotation();
                //
                // // Capture the screenshot with the unique name for each item
                // var icon = LoadSpriteFromFile(CaptureScreenshot(itemData.GetId()));
                //
                // itemData.SetIcon(icon);
                //
                // // Wait for the next frame before capturing the next picture
                // yield return new WaitForEndOfFrame();
                // ClearOutRenderTexture(renderTexture);
                // if (cube) DestroyImmediate(cube);
                // yield return new WaitForEndOfFrame();
            }
        }
    }

    IEnumerator DoScreenShot(ItemDefinition itemData)
    {

        // Spawn the new cube
        SpawnCube(itemData.GetPrefab());

        // Adjust the camera size based on the cube's collider bounds
        AdjustCameraSize();

        // Set camera rotation to (45, 0, 0)
        SetCameraRotation();

        // Capture the screenshot with the unique name for each item
        var icon = ImportPngAsSprite(CaptureScreenshot(itemData.GetId()));
            
        itemData.SetIcon(icon);
        
        EditorUtility.SetDirty(itemData);
        AssetDatabase.SaveAssets();

        // Wait for the next frame before capturing the next picture
        yield return new WaitForEndOfFrame();
        ClearOutRenderTexture(renderTexture);
        if (cube) DestroyImmediate(cube);
        yield return new WaitForEndOfFrame();
    }

    void SpawnCube(GameObject prefab)
    {
        // Instantiate the cube at the specified position
        //cube = Instantiate(prefab, cubePosition, Quaternion.identity);
        cube = Instantiate(prefab, cubePosition, Quaternion.Euler(0f, 30f, 0f));
        cube.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private Bounds bound;
    void AdjustCameraSize()
    {
        // // Try to get the MeshCollider or BoxCollider attached to the cube
        // collider = cube.GetComponentInChildren<Collider>();
        //
        // if (collider != null)
        // {
        //     // Get the bounds from the collider
        //     Bounds colliderBounds = collider.bounds;
        //
        //     // Set the camera's orthographic size based on the object's height
        //     renderCamera.orthographicSize = colliderBounds.size.y;
        //
        //     // Adjust the camera's position to ensure the cube is in view
        //     renderCamera.transform.position = new Vector3(colliderBounds.center.x, colliderBounds.center.y, renderCamera.transform.position.z);
        //
        //     // Ensure the camera is looking at the cube's position
        //     renderCamera.transform.LookAt(colliderBounds.center);
        //
        //     // Optional: Make Rigidbody kinematic if present
        //     Rigidbody rgb = cube.GetComponent<Rigidbody>();
        //     if (rgb != null) rgb.isKinematic = true;
        // }
        // else
        // {
        //     Debug.LogError("Cube does not have a MeshCollider or BoxCollider attached.");
        // }
        
        var collider = cube.GetComponentsInChildren<Collider>().ToList();

        if (collider.Count > 0)
        {
            bound = GroundExtend.CalculateBounds(collider);

            // Move object to origin to simplify positioning
            cube.transform.position = Vector3.zero;
            //bounds = collider.bounds; // Recalculate after moving
            bound = GroundExtend.CalculateBounds(collider);

            // Set camera rotation (45° top-down)
            renderCamera.transform.rotation = Quaternion.Euler(50f, 0f, 0f);

            // Calculate diagonal size to cover angled view
            float size = bound.extents.magnitude; // Radius of the bounding box
            renderCamera.orthographicSize = size; // Add some padding

            // Set camera position offset at a 45° angle
            Vector3 cameraOffset = new Vector3(0, 1, -1).normalized * size * 2.5f; // tweak multiplier as needed
            renderCamera.transform.position = bound.center + cameraOffset;

            renderCamera.transform.LookAt(bound.center);
        }
        else
        {
            Debug.LogError("Cube does not have a MeshCollider or BoxCollider attached.");
        }

    }

    void SetCameraRotation()
    {
        // Set the camera's rotation to (45, 0, 0)
        renderCamera.transform.rotation = Quaternion.Euler(45f, 0f, 0f);

        // Adjust the camera's position to ensure it's looking at the cube
        // Calculate an appropriate distance along the Z-axis to keep the cube in view
        // We want the camera positioned at a place where it can still see the cube after rotation
        float distance = 10f; // Adjust this distance based on the cube size and camera's field of view
        renderCamera.transform.position = new Vector3(0, 5 , -distance);
        // Ensure that the camera is looking at the cube
        renderCamera.transform.LookAt(cubePosition);
        renderCamera.transform.localPosition += new Vector3(0,+ bound.size.y/2,0);


    }

    string CaptureScreenshot(string nameSave)
    {
        renderCamera.clearFlags = CameraClearFlags.SolidColor;
        renderCamera.backgroundColor = new Color(0, 0, 0, 0); // Fully transparent
        
        // Clear the RenderTexture to ensure it's not overwritten
        ClearOutRenderTexture(renderTexture);

        // Set the camera's target texture to the RenderTexture
        renderCamera.targetTexture = renderTexture;

        // Render the camera's view to the texture
        renderCamera.Render();

        // Create a texture to store the screenshot
        //Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

        // Read the pixels from the RenderTexture into the new texture
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // Optionally, save the texture to a file
        byte[] bytes = texture.EncodeToPNG();
        var path = "Assets/New Item Icon/" + nameSave + ".png";
        System.IO.File.WriteAllBytes("Assets/New Item Icon/" + nameSave + ".png", bytes);

        // Reset the RenderTexture and clean up
        RenderTexture.active = null;
        renderCamera.targetTexture = null;

        // Log a message to indicate the screenshot is saved
        Debug.Log("Screenshot saved as png: " + nameSave);

        return path;
    }
    
    public Sprite LoadSpriteFromFile(string filePath)
    {
        if (!System.IO.File.Exists(filePath))
        {
            Debug.LogError("File not found at: " + filePath);
            return null;
        }

        byte[] fileData = System.IO.File.ReadAllBytes(filePath);

        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(fileData); // Auto-resizes the texture based on the data
        tex.Apply();

        // Create sprite
        return Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f), // pivot in the center
            100f                     // pixels per unit, tweak if needed
        );
    }
    
    public static Sprite ImportPngAsSprite(string assetRelativePath)
    {
        AssetDatabase.ImportAsset(assetRelativePath, ImportAssetOptions.ForceUpdate);

        TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(assetRelativePath);
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Single;
        textureImporter.alphaIsTransparency = true;
        textureImporter.SaveAndReimport();

        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetRelativePath);
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetRelativePath); // This works if it's a single-sprite asset

        return sprite;
    }

    public void ClearOutRenderTexture(RenderTexture renderTexture)
    {
        renderTexture.DiscardContents();
        renderTexture.Release();
        RenderTexture.active = null;
    }
}
