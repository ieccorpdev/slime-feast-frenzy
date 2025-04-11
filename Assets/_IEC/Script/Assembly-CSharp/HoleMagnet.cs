using System;
using System.Collections.Generic;
using UnityEngine;

public class HoleMagnet : MonoBehaviour
{
	private Hole m_Hole;
	private readonly HashSet<Collider> m_IgnoreExit = new HashSet<Collider>();
	
	private void Awake()
	{
		m_Hole = GetComponentInParent<Hole>();
	}

	private void OnTriggerEnter(Collider other)
	{
		//m_IgnoreExit.Add(other);
	}

	private void OnTriggerStay(Collider other)
	{
		PullObject(other);
	}

	private void OnTriggerExit(Collider other)
	{
		//m_IgnoreExit.Remove(other);
	}

	private void PullObject(Collider other)
	{
		if (other.CompareTag("FallingItem_Physics") == false) return;
		var dir = other.transform.position - transform.position;
		dir.y = 0;
		//dir = dir.normalized;
		
		var rb = other.attachedRigidbody;
		
		if (rb != null)
		{
			if (rb.gameObject.name.Contains("Bomb")) return;
			rb.isKinematic = false;
			var cacheVelocity = rb.velocity;
			rb.velocity = -dir * m_Hole.Settings.magnetForce + Vector3.up * cacheVelocity.y;
		}
	}

	private void OnDisable()
	{
	}
}
