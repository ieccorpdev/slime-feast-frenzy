using Sirenix.OdinInspector;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    [CreateAssetMenu(fileName = "Item Definition", menuName = "Game/Features/Gameplay/New Falling Item Definition", order = 0)]
    public class ItemDefinition : ScriptableObject
    {
        [SerializeField] [PreviewField(125)] private GameObject prefab;
        [SerializeField] private string id;
        [SerializeField] private SpecialItemID specialItemId;
        [SerializeField] [PreviewField(125)] private Sprite icon;
        //[SerializeField] private SDFSpriteReference sdfSpriteReference;
        [SerializeField] private int level;
        [SerializeField] private int overrideLevel;
        [SerializeField] private int xp;
        [SerializeField] private bool startAsKinematic;
        [SerializeField] private bool canBeGoal;
        [SerializeField] private bool ignoreInCalculations;
        [SerializeField] private FallingItemSetType fallingItemSetType;
        [SerializeField] private PhysicFallingItemType physicFallingItemType;


        
        public void SetSpecialId(SpecialItemID newSpecialItemID)
        {
            specialItemId = newSpecialItemID;
        }

        public SpecialItemID GetSpecialId()
        {
            return specialItemId;
        }
        public PhysicFallingItemType GetPhysicFallingItemType()
        {
            return physicFallingItemType;
        }

        public void SetPhysicFallingItemType(PhysicFallingItemType newPhysicFallingItemType)
        {
            physicFallingItemType = newPhysicFallingItemType;
        }
        
        public bool GetIgnoreInCalculations()
        {
            return ignoreInCalculations;
        }

        public void SetIgnoreInCalculations(bool newIgnoreInCalculations)
        {
            ignoreInCalculations = newIgnoreInCalculations;
        }

        public FallingItemSetType GetFallingItemSetType()
        {
            return fallingItemSetType;
        }

        public void SetFallingItemSetType(FallingItemSetType newFallingItemSetType)
        {
            fallingItemSetType = newFallingItemSetType;
        }
        
        public int GetXp()
        {
            return xp;
        }

        public void SetXp(int newXp)
        {
            xp = newXp;
        }

        public bool GetStartAsKinematic()
        {
            return startAsKinematic;
        }

        public void SetStartAsKinematic(bool newStartAsKinematic)
        {
            startAsKinematic = newStartAsKinematic;
        }

        public bool GetCanBeGoal()
        {
            return canBeGoal;
        }

        public void SetCanBeGoal(bool newCanBeGoal)
        {
            canBeGoal = newCanBeGoal;
        }

        public int GetLevel()
        {
            return level;
        }

        public void SetLevel(int newLevel)
        {
            level = newLevel;
        }

        public int GetOverrideLevel()
        {
            return overrideLevel;
        }

        public void SetOverrideLevel(int newOverrideLevel)
        {
            overrideLevel = newOverrideLevel;
        }
        public GameObject GetPrefab()
        {
            if (prefab == null)
            {
                Debug.LogWarning($"{id} missing prefab");
            }
            return prefab;
        }

        public void SetPrefab(GameObject newPrefab)
        {
            prefab = newPrefab;
        }

        public string GetId()
        {
            return id;
        }

        public void SetId(string newId)
        {
            id = newId;
        }

        public Sprite GetIcon()
        {
            return icon;
        }

        // public SDFSpriteReference GetSpriteReference()
        // {
        //     if (sdfSpriteReference.SDFSprite == null)
        //     {
        //         Debug.LogError("Can not find out sdf for: " + id);
        //     }
        //     return sdfSpriteReference;
        // }

        public void SetIcon(Sprite sprite)
        {
            icon = sprite;
        }
        
#if UNITY_EDITOR
        public void SetDirty()
        {
            UnityEditor.EditorUtility.SetDirty(this);
        }

        public void Save()
        {
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }  
}