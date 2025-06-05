using _IEC.Features.Gameplay;
using IEC;
using Sirenix.OdinInspector;
using UnityEngine;

public class TowerGenerator : _BaseBehaviour
{
    public Mesh mesh;
    public Vector3 scale = new Vector3(2f, 2f, 2f);
    public int maxTowers = 1000;
    public string itemID1 = "rubberduck_0";
    public string itemID2 = "food_294";
    public string itemID3 = "toy_123";

    private void Start()
    {
        SpawnTowers();
    }

    [Button]
    public void ClearTowers()
    {
        foreach (Transform item in this.gameObject.transform)
        {
            DestroyImmediate(item.gameObject);
        }
    }

    [Button]
    public void SpawnTowers()
    {
        Vector3[] verts = mesh.vertices;
        int step = Mathf.Max(1, verts.Length / maxTowers);

        // Find min and max Y
        float minY = float.MaxValue;
        float maxY = float.MinValue;
        for (int i = 0; i < verts.Length; i++)
        {
            float y = Vector3.Scale(verts[i], scale).y;
            if (y < minY) minY = y;
            if (y > maxY) maxY = y;
        }

        float sectionHeight = (maxY - minY) / 3f;

        for (int i = 0; i < verts.Length; i += step)
        {
            Vector3 pos = Vector3.Scale(verts[i], scale);
            string currentItemID;

            if (pos.y < minY + sectionHeight)
                currentItemID = itemID1;
            else if (pos.y < minY + 2 * sectionHeight)
                currentItemID = itemID2;
            else
                currentItemID = itemID3;

            var itemDefinition = PlayableController.Instance.levelConfig.GetItemDefinition(currentItemID, Game.itemVersion);
            var prefab = itemDefinition.GetPrefab();
            GameObject newItem = prefab.Spawn(this.transform);
            var itemComponent = newItem.AddComponent<BaseItem>();
            _IEC.Features.Gameplay.ItemData itemData = new _IEC.Features.Gameplay.ItemData();
            itemData.isStartKinematic = true;
            itemData.itemId = itemDefinition.GetId();
            itemData.position = pos + transform.position;
            itemComponent.LoadData(itemData, itemDefinition);
        }
    }

}
