using _IEC.Features.Gameplay;
using UnityEngine;

public class HoleValidator : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("FallingItem_Physics") == false) return;
		BaseItem baseItem = other.GetComponentInParent<BaseItem>();
		if (baseItem == null) return;
		var position = baseItem.transform.position;
		position.y = transform.position.y;
		baseItem.OnCollected(position);
	}
}
