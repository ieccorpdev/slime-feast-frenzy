using System;
using System.Collections.Generic;
using System.Linq;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay;
using _IEC.Features.Gameplay.ObserverEventData;
using DG.Tweening;
using IEC;
using MoreMountains.NiceVibrations;
using Sirenix.OdinInspector;
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
	[SerializeField] private ParticleSystem waterSplashFx;
	[Header("Character")]
	[SerializeField]private Transform characterGraphics;
	[SerializeField]private Transform holePipe;
	[SerializeField]public Animator characterAnimator;
	[SerializeField]public Animator sharkBiteAnimator;
	[SerializeField]private Transform characterParent;
	[SerializeField]private Transform sharkBiteParent;
	
	private int m_Level;
	private int m_Xp;
	private float m_AdditionalSize = 0;

	private HashSet<BaseItem> m_CollectableItems = new HashSet<BaseItem>();
	private string TweenIdScale => gameObject.GetInstanceID() + "scale";
	
	public float Speed => CalculateIndices(_baseSpeed, Level,Settings.SpeedIncreasePerLevel) 
	                      * (1 + (timeEffectSpeedBooster > 0 ? (_speedBoosterIncrease / 100) : 0)) ;

	public float Size => CalculateIndices(Settings.BaseSize,Level,Settings.SizeIncreasePerLevel) + m_AdditionalSize;

	public int Level => m_Level;

	private Vector3 _defaultScaleGraphic;
	private Vector3 _defaultScaleCanvas;
	private float _baseSpeed;
	private bool onSharkForm;
	private bool onInitAnimNewGame;

	private float _speedBoosterIncrease;
	private float timeEffectSpeedBooster;
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
	private float cTime = 0;
	private float holeTime = 0;
	private float timeToNextBite = 5;
	public bool readyForSharkBite;
	public void Update()
	{
		if (GamePlay.levelController.CurrentState == LevelController.State.Play)
		{
			if (timeEffectSpeedBooster > 0)
			{
				timeEffectSpeedBooster =
					Mathf.Clamp(timeEffectSpeedBooster - Time.deltaTime, 0, timeEffectSpeedBooster);
			}
		}

		CheckPlayerForm();
		CheckSharkBite();
	}

	private void CheckSharkBite()
	{
		if(onSharkForm) return;
		holeTime += Time.deltaTime;
		if (holeTime > timeToNextBite)
		{
			readyForSharkBite = true;
		}
	}

	public void TriggerdBite()
	{
		if (readyForSharkBite)
		{
			float randomRotationY = UnityEngine.Random.Range(0f, 360f);
			sharkBiteParent.transform.Rotate(0f, randomRotationY, 0f);
			sharkBiteAnimator.Play("Bite");
			timeToNextBite = UnityEngine.Random.Range(3, 5);
			holeTime = 0;
		}
	}
	private void CheckPlayerForm()
	{
		cTime += Time.deltaTime;
		if (cTime > 0.1f)
		{
			cTime = 0f;
			if (onInitAnimNewGame) return;
			var obj = GamePlay.levelController.GetAllItems().FirstOrDefault(x =>
				x != null && Vector3.Distance(x.transform.position, transform.position) < 10);
			if (obj != null)
			{
				if (onSharkForm)
				{
					onSharkForm = false;
					TurnSharkToHole();
				}
			}
			else
			{
				if (!onSharkForm)
				{
					onSharkForm = true;
					TurnHoleToShark();
				}
			}
		}
	}

	void OnCollectLightningBooster(object param)
	{
		// var data = (MagicPouchBooster)param;
		// if (data.type == MagicPouchBoosterType.Lightning)
		// {
		// 	lightningVfx.Play();
		// }
	}

	public void OnCollectSpeedBooster(float boosterIncrease, float timeEffect)
	{
		_speedBoosterIncrease = boosterIncrease;
		timeEffectSpeedBooster += timeEffect;
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
	
	#if UNITY_EDITOR
	
	[PropertySpace(30)]
	
	[Button("Reset Level")]
	public void DebugResetLevel()
	{
		//LevelUp(0, 1);
		SetLevel(0,false);
		
		FindObjectOfType<CameraController>().OnUpdateCameraTargetEditMode(new UpdateCameraTarget()
		{
			target =  transform,
			//cameraDistance = Settings.BaseCameraDistance  + (level-1) * Settings.CameraDistanceIncreasePerLevel
			cameraDistance = CalculateIndices(Settings.BaseCameraDistance,Level,Settings.CameraDistanceIncreasePerLevel)
		});
	}

	private bool LevelMax => (Settings.UseSlowXp
		? Level >= Settings.RequiredXpPerLevelSlow.Length - 1
		: Level >= Settings.RequiredXpPerLevel.Length - 1);
	[ShowIf("@!LevelMax")]
	[Button("Level Up")]
	public void DebugLevelUp()
	{
		//LevelUp(0, 1);
		SetLevel(Level + 1,false);
		
		FindObjectOfType<CameraController>().OnUpdateCameraTargetEditMode(new UpdateCameraTarget()
		{
			target =  transform,
			//cameraDistance = Settings.BaseCameraDistance  + (level-1) * Settings.CameraDistanceIncreasePerLevel
			cameraDistance = CalculateIndices(Settings.BaseCameraDistance,Level,Settings.CameraDistanceIncreasePerLevel)
		});
	}
	
	[ShowIf("@(Level > 0)")]
	[Button("Level Down")]
	public void DebugLevelDown()
	{
		//LevelUp(0, -1);
		SetLevel(Level - 1,false);
		FindObjectOfType<CameraController>().OnUpdateCameraTargetEditMode(new UpdateCameraTarget()
		{
			target =  transform,
			//cameraDistance = Settings.BaseCameraDistance  + (level-1) * Settings.CameraDistanceIncreasePerLevel
			cameraDistance = CalculateIndices(Settings.BaseCameraDistance,Level,Settings.CameraDistanceIncreasePerLevel)
		});
	}
	
	#endif
	
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
	public float GetWorldRadius()
	{
		return transform.localScale.x / 2f;
	}
	public void OnNewGame()
	{
		onInitAnimNewGame = true;
		txtLevel.gameObject.SetActive(true);
	}
	private void TurnOffAllVfx()
	{
		//levelUpVfx.Stop();
		sizeUpVfx.Stop();
	}

	#region Character Animation
	public string SHARK_TWEEN_MOVE_ID => characterParent.gameObject.GetInstanceID() + "_CHARACTER_ID_MOVE";
	public void ResetCharacter()
	{
		holePipe.transform.localScale = Vector3.zero;
		holePipe.gameObject.SetActive(false);
		characterGraphics.transform.localScale = Vector3.one;
		characterParent.gameObject.SetActive(true);
		characterAnimator.transform.localPosition = new Vector3(0, -0.9f, 0);
		characterAnimator.Play("Spawn_Game");
		holeTime = -3;
	}

	public void PlayEffectSharkDriveIn()
	{

		//Apply position because animation diff with pos
		waterSplashFx.Play();
		holePipe.gameObject.SetActive(true);
		holePipe.DOScale(Vector3.one, 1.0f);
	}

	public void EndAnimationCharacter()
	{
		characterParent.gameObject.SetActive(false);
		onInitAnimNewGame = false;
	}

	public void TurnHoleToShark()
	{
		DOTween.Kill(SHARK_TWEEN_MOVE_ID);
		holePipe.DOScale(Vector3.zero, 0.5f);
		characterParent.gameObject.SetActive(true);
		characterGraphics.transform.localPosition = new Vector3(0, 0, 0);
		characterAnimator.transform.localPosition = new Vector3(0, -0.9f, 0);
		characterAnimator.Play("HoleToShark");
	}
	public void TurnSharkToHole()
	{
		DOTween.Kill(SHARK_TWEEN_MOVE_ID);
		holePipe.DOScale(Vector3.zero, 1.0f);
		characterGraphics.transform.localPosition = new Vector3(0, 0, 0);
		characterParent.transform.localPosition = new Vector3(0, 0, 0);
		characterAnimator.transform.localPosition = new Vector3(0, -0.9f, 0);
		characterAnimator.Play("Dive Into Water 0");
		holeTime = -3;
	}



	#endregion


}
