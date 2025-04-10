using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HoleXpBar : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private Image mainImage;

	[Space]
	[SerializeField]
	private RectTransform barContainer;

	[SerializeField]
	private RectTransform innerGlowContainer;

	[SerializeField]
	[Header("Parameters")]
	private float minScale;
	[SerializeField]
	private float maxScale;

	
	public void SetAmount(float amount,bool isLerp)
	{
		if (isLerp)
		{
			mainImage.DOFillAmount(amount, 0.2f);
		}
		else
		{
			mainImage.fillAmount = amount;
		}
		
		transform.localScale = Mathf.Lerp(minScale, maxScale, amount) * (Vector3.right + Vector3.up) + Vector3.forward;
	}
}
