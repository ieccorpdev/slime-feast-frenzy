using System;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay;
using _IEC.Features.Gameplay.ObserverEventData;
using DG.Tweening;
using IEC;
using MoreMountains.NiceVibrations;
using TMPro;
using UnityEngine;

public class Hole : _BaseBehaviour
{
	[SerializeField] private Transform holeGraphics;
	[SerializeField] private Transform holeCanvases;
	
	[Header("Settings")]
	[SerializeField]
	private HoleConfig m_settings;
	[SerializeField] private HoleController holeController;
	[SerializeField] private TextMeshProUGUI txtLevel;
	[SerializeField] private HoleXpBar xpBar;
	[Header("Helper")]
	[SerializeField] private HoleMagnet holeMagnet;
	[Header("VFX")] [SerializeField] private ParticleSystem levelUpVfx;
	[SerializeField] private ParticleSystem sizeUpVfx;
	[SerializeField] private ParticleSystem magnetVfx;
	[SerializeField] private ParticleSystem exploreVfx;
	[SerializeField] private ParticleSystem deathVfx;
	[SerializeField] private ParticleSystem lightningVfx;
	[Header("Character")]
	[SerializeField]private Transform characterGraphics;
	[SerializeField]private Transform holePipe;
	[SerializeField]private Animator characterAnimator;

	private int m_Level;
	private int m_Xp;
	private float m_AdditionalSize = 0;

	private HashSet<BaseItem> m_CollectableItems = new HashSet<BaseItem>();
	private string TweenIdScale => gameObject.GetInstanceID() + "scale";
	
	public float Speed => CalculateIndices(_baseSpeed, Level,Settings.SpeedIncreasePerLevel);

	public float Size => CalculateIndices(Settings.BaseSize,Level,Settings.SizeIncreasePerLevel) + m_AdditionalSize;

	public int Level => m_Level;

	private Vector3 _defaultScaleGraphic;
	private Vector3 _defaultScaleCanvas;
	private float _baseSpeed;

	public float baseSpeed
	{
		get => _baseSpeed;
		set => _baseSpeed = value;
	}

	public HoleConfig Settings => m_settings;

	private void Awake()
	{
		m_settings = EncryptedAssetModule.LoadAsset<HoleConfig>("HoleConfig.asset");
		_defaultScaleCanvas = holeCanvases.localScale;
		_defaultScaleGraphic = holeGraphics.localScale;
		_baseSpeed = Settings.BaseSpeed;
		
		this.RegisterListener(EventList.GamePlay.ON_BOOSTER_COLLECTED, OnCollectLightningBooster);
	}

	private void OnDestroy()
	{
		this.UnregisterListener(EventList.GamePlay.ON_BOOSTER_COLLECTED, OnCollectLightningBooster);
	}

	void OnCollectLightningBooster(object param)
	{
		var data = (MagicPouchBooster)param;
		if (data.type == MagicPouchBoosterType.Lightning)
		{
			lightningVfx.Play();
		}
	}
	
	public void PushItemToTrackingList(BaseItem baseItem)
	{
		if (m_CollectableItems.Contains(baseItem) == false)
		{
			m_CollectableItems.Add(baseItem);
		}
	}

	public void RemoveItemFromTrackingList(BaseItem baseItem)
	{
		if (m_CollectableItems.Contains(baseItem))
		{
			m_CollectableItems.Remove(baseItem);
		}
	}

	public void StartScaleByHelper()
	{
		m_AdditionalSize = Config.gamePlayConfig.scaleUpHelperAdditionalSize;
		SetSize(Size,true,null);
	}

	public void StopScaleByHelper()
	{
		if (m_AdditionalSize <= 0) return;
		m_AdditionalSize = 0;
		SetSize(Size,true,null);
	}
	
	public void SetLevel(int level, bool lerpSize = false)
	{
		gameObject.SetActive(true);
		var myTransform = transform;
		txtLevel.text = $"Lv.{level}";
		m_Level = level;
		var targetSize = Size;
		SetSize(targetSize,lerpSize, () =>
		{
			this.RaiseEvent(EventList.GamePlay.UPDATE_CAMERA_TARGET,new UpdateCameraTarget()
			{
				target =  myTransform,
				//cameraDistance = Settings.BaseCameraDistance  + (level-1) * Settings.CameraDistanceIncreasePerLevel
				cameraDistance = CalculateIndices(Settings.BaseCameraDistance,level,Settings.CameraDistanceIncreasePerLevel)
			});
		});
		SetXp(0,false);

	}

	public void SetSize(float targetSize,bool isLerpSize,Action callback)
	{
		var targetScale = targetSize * Vector3.one;
		var myTransform = transform;
		DOTween.Kill(TweenIdScale);
		if (isLerpSize)
		{
			sizeUpVfx.Play();
			myTransform.DOScale(targetScale, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
			{
				callback?.Invoke();
			}).SetId(TweenIdScale);
		}
		else
		{
			myTransform.localScale = targetScale;
			callback?.Invoke();
		}
		holeController.SetScale(targetSize);
	}

	public void LevelUp(int remaining, int count = 1)
	{
		SetLevel(Level + count,true);
		SetXp(remaining,true);
		
		//SoundController.PlaySound(new AudioInfo(Config.soundConfig.levelUp, false));
		//VibrationController.Vibrate(HapticTypes.Selection);
	}

	public void SetXp(int xp,bool isLerp)
	{
		m_Xp = xp;
		var neededXp = Settings.GetRequiredXp(Level, false);
		xpBar.SetAmount((float)m_Xp/neededXp,isLerp);
		if (m_Xp >= neededXp)
		{
			int remaining = m_Xp - neededXp;
			LevelUp(remaining);
		}
	}

	public void AddXp(int xp)
	{
		SetXp(m_Xp + xp,true);
	}
	
	public void SetActive(bool active)
	{
		holeController.enabled = active;
	}

	public void SetPosition(Vector3 position)
	{
		holeController.SetPosition(position);
	}

	public void StartMagnetHelper()
	{
		holeMagnet.gameObject.SetActive(true);
		magnetVfx.Play();
	}

	public void StopMagnetHelper()
	{
		holeMagnet.gameObject.SetActive(false);
		magnetVfx.Stop();
	}

	public void CollectABomb()
	{
		SpawnVfx(exploreVfx);
		SpawnVfx(deathVfx);
		DOTween.Sequence()
			.AppendInterval(0.2f)
			.Append(holeGraphics.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack))
			.Join(holeCanvases.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack))
			.InsertCallback(0.3f, () =>
			{
				//SoundController.PlaySound(new AudioInfo(Config.soundConfig.bombLose, false));
			});
	}

	public void ResetSize()
	{
		// Debug.LogError("Size: " + Size);
		// Debug.LogError("Level: " + Level);
		m_AdditionalSize = 0;
		SetSize(Size,false,null);
		holeGraphics.localScale = _defaultScaleGraphic;
		holeCanvases.localScale = _defaultScaleCanvas;
	}
	
	private void SpawnVfx(ParticleSystem vfxPrefab)
	{
		var vfx = vfxPrefab.Spawn();
		var vfxTransform = vfx.transform;
		vfxTransform.position = transform.position;
		vfxTransform.parent = null;
		vfxTransform.localScale = Size * Vector3.one;
		vfx.Play();
	}
	

	private float CalculateIndices(float baseIndices, float level, float bias)
	{
		return baseIndices + (level - 1) * bias;
	}
	
	public void Reset()
	{
		TurnOffAllVfx();
		holeMagnet.gameObject.SetActive(false);
		m_Level = 1;
		ResetSize();
		m_Xp = 0;
		xpBar.SetAmount(m_Xp,false);
		SetLevel(1);
		ResetCharacter();
	}

	public void HideLevelText()
	{
		txtLevel.gameObject.SetActive(false);
	}

	public void ShowLevelText()
	{
		txtLevel.gameObject.SetActive(true);
	}

	public void OnNewGame()
	{
		AnimationTurnHole();
		txtLevel.gameObject.SetActive(true);
	}
	private void TurnOffAllVfx()
	{
		//levelUpVfx.Stop();
		sizeUpVfx.Stop();
	}

	public void ResetCharacter()
	{
		holePipe.transform.localScale = Vector3.zero;
		holePipe.gameObject.SetActive(false);
		characterGraphics.transform.localScale = Vector3.one;
		characterAnimator.Play("Slime_Idle");
	}
	public void AnimationTurnHole()
	{
		holePipe.gameObject.SetActive(true);
		characterAnimator.SetTrigger("Trans");
		holePipe.DOScale(Vector3.one, 1.0f);
	}

	public float GetWorldRadius()
	{
		return transform.localScale.x / 2f;
	}
}
