using UnityEngine;

public static class LayerChanger
{
    /// <summary>
    /// Thay đổi layer của GameObject và tất cả các child dựa trên tên layer.
    /// </summary>
    /// <param name="obj">GameObject cần thay đổi layer.</param>
    /// <param name="layerName">Tên layer mới.</param>
    public static void ChangeLayerRecursively(GameObject obj, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (layer == -1)
        {
            Debug.LogError($"Layer '{layerName}' không tồn tại!");
            return;
        }

        ChangeLayerRecursive(obj, layer);
    }

    /// <summary>
    /// Thay đổi layer cho GameObject và các child (đệ quy).
    /// </summary>
    /// <param name="obj">GameObject cần thay đổi layer.</param>
    /// <param name="layer">Layer index.</param>
    private static void ChangeLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            ChangeLayerRecursive(child.gameObject, layer);
        }
    }
}