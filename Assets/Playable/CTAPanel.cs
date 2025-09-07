using UnityEngine;
using UnityEngine.UI;

public class CTAPanel : MonoBehaviour
{
    [SerializeField] private Button _playNowButton;
    void Awake()
    {
        _playNowButton.onClick.AddListener(() =>
        {
            Luna.Unity.Playable.InstallFullGame();
        });
    }
}