using _IEC.Features.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;
using System.IO;
using IEC;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ItemReplaceUtility : MonoBehaviour
{
#if UNITY_EDITOR 
	[SerializeField] private bool useConvexCollider = true;
	[SerializeField] private bool preservedYAxis = false;

	private GameObject _itemToReplace;
	private ScaleUtilities _scaleUtilities;
	
	private readonly string _folderPathIcon = "_IEC/Image/UI_Icon_Objective";
	private readonly string _folderPathItemDefinition = "_IEC/Features/Gameplay/Config/Items";
	private readonly string _folderPathSkinItem = "_Art External/";
	// private readonly string _folderPathSkinItem = "_Art External/ithappy/Casual_Food/Prefabs";

	public void ReplaceSkin(string newId, bool isPrefabOnly = false)
	{
		preservedYAxis = true;
		ValidateScaleUtilities();
		HideOldSkin();
		SpawnNewSkin(newId, isPrefabOnly);
		AddCollider();
		UpdateIconInItemDefinition();
		AdjustScaleAndPosition();
	}

	void ValidateScaleUtilities()
	{
		_scaleUtilities = gameObject.GetComponent<ScaleUtilities>();
		if (_scaleUtilities != null)
		{
			_scaleUtilities.reductionFactor = 1f;
			return;
		}

		MeshRenderer originalMesh = null;
		MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer renderer in renderers)
		{
			if (renderer.enabled)
			{
				originalMesh = renderer;
				break;
			}
		}

		if (originalMesh == null)
		{
			Debug.LogWarning($"Missing {gameObject.name}");
		}
		
		_scaleUtilities = gameObject.AddComponent<ScaleUtilities>();
		_scaleUtilities.isAutoScale = false;
		_scaleUtilities.targetObject = originalMesh;
		_scaleUtilities.reductionFactor = 1f;
	}
	
	void SpawnNewSkin(string newId, bool isPrefabOnly)
	{
		GameObject prefab;
		if (isPrefabOnly)
			prefab = Utilities.FindPrefabInFolder(_folderPathSkinItem, newId);
		else
			prefab = Utilities.FindAssetInFolder<GameObject>(_folderPathSkinItem, newId);
		_itemToReplace = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
		_itemToReplace.transform.parent = transform;
	}
	
	void AdjustScaleAndPosition()
	{
		_scaleUtilities.objectToScale = _itemToReplace.GetComponent<MeshRenderer>();
		if (preservedYAxis)
		{
			_scaleUtilities.AdjustObjectsPreserveY();
			return;
		}
		_scaleUtilities.AdjustObjects();
	}
	
	void HideOldSkin()
	{
		var oldSkin = GetOldSkin();
		if (oldSkin != null)
		{
			oldSkin.SetActive(false);
		}
		
		foreach (Transform renderer in transform)
		{
			if (renderer.gameObject.activeSelf)
			{
				renderer.gameObject.SetActive(false);
			}
		}
	}

	GameObject GetOldSkin()
	{
		if (_scaleUtilities.objectToScale == null)
		{
			return null;
		}
		
		return _scaleUtilities.objectToScale.gameObject;
	}
	
	void AddCollider()
	{
		var existingCollider = _itemToReplace.GetComponent<Collider>();
		if (existingCollider != null)
		{
			DestroyImmediate(existingCollider);
		}
		
		if (useConvexCollider)
		{
			var meshCollider = _itemToReplace.AddComponent<MeshCollider>();
			meshCollider.convex = true;
			meshCollider.cookingOptions = MeshColliderCookingOptions.None;
		}
		else
		{
			var sphereCollider = _itemToReplace.AddComponent<SphereCollider>();
		}
	}

	void UpdateIconInItemDefinition()
	{
		var itemName = _itemToReplace.name;
		var itemDefinition = Utilities.FindAssetInFolder<ItemDefinition>(_folderPathItemDefinition, transform.name);
		var newIcon = Utilities.FindAssetInFolder<Sprite>(_folderPathIcon, itemName);

		if (newIcon != null)
		{
			itemDefinition.SetIcon(newIcon);
			itemDefinition.SetDirty();
			//AssetDatabase.SaveAssets();
		}
	}
#endif
}
