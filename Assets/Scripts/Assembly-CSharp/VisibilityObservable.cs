using UnityEngine;
using UnityEngine.Events;

public class VisibilityObservable : MonoBehaviour
{
	private bool _isVisible;

	public UnityEvent<bool> OnVisible;

	public bool IsVisible => false;

	private void OnBecameVisible()
	{
	}

	private void OnBecameInvisible()
	{
	}
}
