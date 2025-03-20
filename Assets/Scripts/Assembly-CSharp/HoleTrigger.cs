using _IEC.Features.Gameplay;
using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
	private Hole m_Hole;

	private void Awake()
	{
		m_Hole = transform.GetComponentInParent<Hole>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("FallingItem_Physics") == false) return;
		BaseItem baseItem = other.GetComponentInParent<BaseItem>();
		if (baseItem == null) return;
		baseItem.EnterHoleTrigger();
		//m_Hole.PushItemToTrackingList(baseItem);
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("FallingItem_Physics") == false) return;
		BaseItem baseItem = other.GetComponentInParent<BaseItem>();
		if (baseItem == null) return;
		var holeTransform = m_Hole.transform;
		/*var offset = holeTransform.position.y - 2;
		if (baseItem.transform.position.y < offset) return;*/
		baseItem.ExitHoleTrigger();
		//m_Hole.RemoveItemFromTrackingList(baseItem);
	}
}
