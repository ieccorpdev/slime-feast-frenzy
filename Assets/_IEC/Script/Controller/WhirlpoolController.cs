using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class WhirlpoolController : MonoBehaviour
{
    [SerializeField]
    private Transform targetPosMoveTo;
    private MeshRenderer meshRenderer;
    private Material material;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
    }

    private void HandleMoveWhirlpoolToTarget()
    {
        if (targetPosMoveTo != null)
        {
            Vector3 targetPos = targetPosMoveTo.position;
            material.SetVector("_WhirlpoolCenter", new Vector4(targetPos.x, targetPos.z, 0f, 0f));
        }
    }

    private void Update()
    {
        HandleMoveWhirlpoolToTarget();
    }
}
