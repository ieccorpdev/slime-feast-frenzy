namespace IEC
{
	public class GoldTextCountAnimation: UITextCountAnimation
	{
		private void OnEnable()
		{
			SetTargetValue(PlayerData.GetPlayerCurrencyCount(ResourceType.Gold), true);
		}

		private void Start()
		{
			Observer.AddObserver(EventList.GamePlay.GOLD_CHANGE, () =>
			{
				if (!gameObject.activeInHierarchy) return;
				SetTargetValue(PlayerData.GetPlayerCurrencyCount(ResourceType.Gold));
			});
		}
	}
}

