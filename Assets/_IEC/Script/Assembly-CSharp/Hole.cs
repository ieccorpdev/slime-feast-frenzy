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
	public HoleBombShield holeBombShield;
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
	[SerializeField] private ParticleSystem radialCompassVfx;
	[SerializeField] private ParticleSystem magnetVfx;
	[SerializeField] private ParticleSystem exploreVfx;
	[SerializeField] private ParticleSystem deathVfx;
	[SerializeField] private ParticleSystem lightningVfx;
	public ParticleSystem waterSplashFx;
	[SerializeField] private ParticleSystem speedLinesVFX;
	[Header("Character")]
	[SerializeField]private Transform characterGraphics;
	[SerializeField]private Transform holePipe;
	[SerializeField]public Animator characterAnimator;
	[SerializeField]public Animator sharkBiteAnimator;
	[SerializeField]private Transform characterParent;
	[SerializeField]private Transform sharkBiteParent;
	[Header("ThemeConfig")]
	private int m_Level;
	private int m_Xp;
	private float m_AdditionalSize = 0;
	private float m_AdditionalSizeFromCharacterTrait = 1;
	private float m_AdditionalSpeedFromCharacterTrait = 1;
	private float m_AdditionalSizeFromCharacterPower = 1;
	//Increase size while idle
	private float m_characterIdleSizeIncrease = 0f;
	private float m_characterIdleSizeIncreaseInterval = 1f;
	private float m_currentIntervalIdleSizeIncrease = 0f;
	private float m_characterMaxSize;
	
	private float defaultPosYCharacter = -0f;
	private HashSet<BaseItem> m_CollectableItems = new HashSet<BaseItem>();
	private string TweenIdScale => gameObject.GetInstanceID() + "scale";
	private string WaterMagnetEffect => "_WhirlpoolRadius";
	public float Speed => CalculateIndices(_baseSpeed, Level,Settings.SpeedIncreasePerLevel) * m_AdditionalSpeedFromCharacterTrait 
	                      * (1 + (timeEffectSpeedBooster > 0 ? (_speedBoosterIncrease / 100) : 0)) ;

	public float Size => CalculateIndices(Settings.BaseSize,Level,Settings.SizeIncreasePerLevel) 
	                     * m_AdditionalSizeFromCharacterTrait 
	                     * m_AdditionalSizeFromCharacterPower
	                     + m_AdditionalSize;

	public int Level => m_Level;

	private Vector3 _defaultScaleGraphic;
	private Vector3 _defaultScaleCanvas;
	private float _baseSpeed;
	private bool onSharkForm;
	private bool onInitAnimNewGame;

	private float _speedBoosterIncrease;
	private float timeEffectSpeedBooster;

	private float timeAdditionalCamSize;
	private float _additionalCamSize;
	public float baseSpeed
	{
		get => _baseSpeed;
		set => _baseSpeed = value;
	}

	public HoleConfig Settings => m_settings;

	public CharacterConfig CurrentCharacterConfig { get; private set; }

	private void Awake()
	{
		m_settings = EncryptedAssetModule.LoadAsset<HoleConfig>("HoleConfig.asset");
		_defaultScaleCanvas = holeCanvases.localScale;
		_defaultScaleGraphic = holeGraphics.localScale;
		_baseSpeed = Settings.BaseSpeed;
		
		m_characterMaxSize = CalculateIndices(Settings.BaseSize, Settings.GetListLevel().Length - 1,
			                     Settings.SizeIncreasePerLevel)
		                     * m_AdditionalSizeFromCharacterTrait
		                     + m_AdditionalSize;
		
		this.RegisterListener(EventList.GamePlay.ON_BOOSTER_COLLECTED, OnCollectLightningBooster);
	}

	private void OnDestroy()
	{
		this.UnregisterListener(EventList.GamePlay.ON_BOOSTER_COLLECTED, OnCollectLightningBooster);
	}
	private float cTime = 0;
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
			else
			{
				if (speedLinesVFX.isPlaying)
				{
					speedLinesVFX.Stop();
				} 
				SoundController.StopSound(EventList.Audio.BOOSTER_WINDSURF);
			}

			if (timeAdditionalCamSize > 0)
			{
				timeAdditionalCamSize =
					Mathf.Clamp(timeAdditionalCamSize - Time.deltaTime, 0, timeAdditionalCamSize);

				if (timeAdditionalCamSize <= 0)
				{
					this.RaiseEvent(EventList.GamePlay.UPDATE_CAMERA_TARGET,new UpdateCameraTarget()
					{
						target =  transform,
						//cameraDistance = Settings.BaseCameraDistance  + (level-1) * Settings.CameraDistanceIncreasePerLevel
						// cameraDistance = CalculateIndices(Settings.BaseCameraDistance,m_Level,Settings.CameraDistanceIncreasePerLevel) 
						//                  * (1 + (timeAdditionalCamSize > 0 ? _additionalCamSize : 0))
						cameraDistance = GetCamSize()
					});
				}
			}

			CheckIncreaseCharacterSizeOnIdle();
		}

		CheckSharkForm();
		UpdateGravityBySpeed();
	}

	private void CheckIncreaseCharacterSizeOnIdle()
	{
		if (!Locator.Game.ScreenInGame.gameplayTimer.GameStarted)
			return;
		
		if (m_characterIdleSizeIncrease < Mathf.Epsilon)
			return;
		
		m_currentIntervalIdleSizeIncrease += Time.deltaTime;
		if (m_currentIntervalIdleSizeIncrease >= m_characterIdleSizeIncreaseInterval)
		{
			m_currentIntervalIdleSizeIncrease = 0f;
			
			if (!holeController.IsMoving)
			{
				if (Size <= m_characterMaxSize)
				{
					m_AdditionalSizeFromCharacterPower *= m_characterIdleSizeIncrease;
					if (m_AdditionalSizeFromCharacterPower > 1f)
					{

						SetSize(Size, true, null);
					}
				}
			}
			else
			{
				// if (m_AdditionalSizeFromCharacterPower > 1f)
				// {
				// 	m_AdditionalSizeFromCharacterPower = 1f;
				// 	ResetSize();
				// }

				if (Mathf.Abs(m_AdditionalSizeFromCharacterPower - 1f) > Mathf.Epsilon)
				{
					m_AdditionalSizeFromCharacterPower /= m_characterIdleSizeIncrease;
					m_AdditionalSizeFromCharacterPower = Mathf.Max(m_AdditionalSizeFromCharacterPower, 1f);
					
					if (m_AdditionalSizeFromCharacterPower < 1f)
						m_AdditionalSizeFromCharacterPower = 1f;
					
					SetSize(Size, true, null, false);
				}
			}
			
			this.RaiseEvent(EventList.GamePlay.UPDATE_CAMERA_TARGET, new UpdateCameraTarget()
			{
				target = transform,
				//cameraDistance = Settings.BaseCameraDistance  + (level-1) * Settings.CameraDistanceIncreasePerLevel
				cameraDistance = GetCamSize()
			});
		}
	}

	private float GetCamSize()
	{
		return CalculateIndices(Settings.BaseCameraDistance, m_Level,
			       Settings.CameraDistanceIncreasePerLevel)
		       * (1 + (timeAdditionalCamSize > 0 ? _additionalCamSize : 0)) *
		       Mathf.Max((m_AdditionalSizeFromCharacterPower / 2f), 1f);
	}

	private void UpdateGravityBySpeed()
	{
		//This function must stay in another class
		float baseGravity = -30 -(Speed - 4) * 5;
		Physics.gravity = new Vector3(0, baseGravity , 0);
	}
	public void TriggerdBite()
	{
		if (readyForSharkBite && !onSharkForm)
		{
			float randomRotationY = 0f;
			if (UnityEngine.Random.value < 0.5f)
			{
				randomRotationY = UnityEngine.Random.Range(0f, 360f);
				sharkBiteAnimator.Play( "Bite" );
			}
			else
			{
				sharkBiteAnimator.Play( "BiteRotate");
			}
			sharkBiteParent.transform.Rotate(0f, randomRotationY, 0f);
			readyForSharkBite = false;
            EventController.RaiseEvent(EventList.Audio.SHARK_BITE_VARIANT);
        }
	}
	private void CheckSharkForm()
	{
		cTime += Time.deltaTime;
		if (cTime > 0.1f)
		{
			cTime = 0f;
			if (!onInitAnimNewGame) return;
			var obj =  GetObjectInDistanceAnimation();
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

	public float GetDistanceToObject()
	{
		return (Speed * 1.1f  + Level * 0.4f) * m_AdditionalSizeFromCharacterTrait * m_AdditionalSizeFromCharacterPower;
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
		speedLinesVFX.Play();
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
	public void StartCompassHelper()
	{
		radialCompassVfx.Play();
	}

	public void StopScaleByHelper()
	{
		if (m_AdditionalSize <= 0) return;
		m_AdditionalSize = 0;
		SetSize(Size,true,null);
	}
	public void StopCompassHelper()
	{
		radialCompassVfx.Stop();
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
				// cameraDistance = CalculateIndices(Settings.BaseCameraDistance,level,Settings.CameraDistanceIncreasePerLevel) 
				//                  * (1 + (timeAdditionalCamSize > 0 ? _additionalCamSize : 0))
				cameraDistance = GetCamSize()
			});
		});
		SetXp(0,false);

	}

	public void SetSize(float targetSize,bool isLerpSize,Action callback, bool playVfx = true)
	{
		var targetScale = targetSize * Vector3.one;
		var myTransform = transform;
		
		DOTween.Kill(TweenIdScale);
		if (isLerpSize)
		{
			if(playVfx)
				sizeUpVfx.Play();
			
			//levelUpVfx.Play();
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
        EventController.RaiseEvent(EventList.Audio.HOLE_LEVELUP);
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

	public int GetCurrentXp()
	{
		int xp = m_Xp;
		for (int i = 0; i < m_Level; i++)
		{
			xp += Settings.GetRequiredXp(i, false);
		}
		return xp;
	}

	public void DecreaseXp(int xp)
	{
		var totalDecreaseXp = xp;

		while (totalDecreaseXp > 0)
		{
			if (totalDecreaseXp > m_Xp)
			{
				totalDecreaseXp -= m_Xp;
				if(m_Level > 1)
					m_Level--;
				SetLevel(Level);
				
				if(m_Level == 1)
				{
					//totalDecreaseXp = 0;
					if (m_Xp > totalDecreaseXp)
					{
						SetXp(m_Xp - totalDecreaseXp,true);
					}
					else
					{
						SetXp(0,true);
					}

					totalDecreaseXp = 0;
				}
				else
				{
					//SetXp(Settings.GetRequiredXp(Level, false) - 1, true);
					//SetXp(0,true);
					m_Xp = Settings.GetRequiredXp(Level, false);
				}
			}
			else
			{
				SetXp(m_Xp - totalDecreaseXp,true);
				totalDecreaseXp = 0;
			}
		}
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

	public void CollectABomb(bool isDoAnim = true)
	{
		SpawnVfx(exploreVfx);
		//SpawnVfx(deathVfx);
		if (isDoAnim)
		{
			DOTween.Sequence()
				.AppendInterval(0.2f)
				.Append(holeGraphics.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack))
				.Join(holeCanvases.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack))
				.InsertCallback(0.3f, () =>
				{
					//SoundController.PlaySound(new AudioInfo(Config.soundConfig.bombLose, false));
				});
		}
	}

	public void ResetSize()
	{
		// Debug.LogError("Size: " + Size);
		// Debug.LogError("Level: " + Level);
		m_AdditionalSize = 0;
		SetSize(Size,false,null);
		this.RaiseEvent(EventList.GamePlay.UPDATE_CAMERA_TARGET,new UpdateCameraTarget()
		{
			target =  transform,
			//cameraDistance = Settings.BaseCameraDistance  + (level-1) * Settings.CameraDistanceIncreasePerLevel
			// cameraDistance = CalculateIndices(Settings.BaseCameraDistance,m_Level,Settings.CameraDistanceIncreasePerLevel) 
			//                  * (1 + (timeAdditionalCamSize > 0 ? _additionalCamSize : 0))
			cameraDistance = GetCamSize()
		});
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
		CheckCurrentCharacter();
		CheckApplyCharacterTrait();
		ResetSize();
		m_Xp = 0;
		xpBar.SetAmount(m_Xp,false);
		SetLevel(1);
		ResetCharacter();
	}

	#region Character Power
	private void CheckCurrentCharacter()
	{
		CurrentCharacterConfig = Config.allCharactersConfig.GetCharacterConfig(PlayerData.playerCharacterName);
		
		var charObj = CurrentCharacterConfig.characterControlPrefab
			.Spawn(
				characterAnimator.transform.parent, 
				CurrentCharacterConfig.characterControlPrefab.transform.position,
				CurrentCharacterConfig.characterControlPrefab.transform.rotation
				);
		Destroy(characterAnimator.gameObject);
		characterAnimator = charObj;
		charObj.GetComponent<SharkControl>().SetHole(this);
		charObj.transform.localScale = CurrentCharacterConfig.characterControlPrefab.transform.localScale;

		charObj.speed = CurrentCharacterConfig.characterAnimSpeed;
		
		var biteObj = CurrentCharacterConfig.characterBitePrefab
			.Spawn(
				sharkBiteAnimator.transform.parent,
				CurrentCharacterConfig.characterBitePrefab.transform.position,
				CurrentCharacterConfig.characterBitePrefab.transform.rotation
				);
		Destroy(sharkBiteAnimator.gameObject);
		sharkBiteAnimator = biteObj;
		biteObj.GetComponent<SharkControl>().SetHole(this);
		biteObj.transform.localScale = CurrentCharacterConfig.characterBitePrefab.transform.localScale;
		
		biteObj.speed = CurrentCharacterConfig.characterAnimSpeed;

		defaultPosYCharacter = CurrentCharacterConfig.defaultCharacterPosY;
	}

	private void CheckApplyCharacterTrait()
	{
		m_AdditionalSpeedFromCharacterTrait = 1;
		m_AdditionalSizeFromCharacterTrait = 1;

		foreach (var trait in CurrentCharacterConfig.listCharacterTraits)
		{
			switch (trait.traitType)
			{
				case CharacterTraitType.IncreaseSpeed:
					m_AdditionalSpeedFromCharacterTrait += trait.buffAmount;
					break;
				case CharacterTraitType.IncreaseSize:
					m_AdditionalSizeFromCharacterTrait += trait.buffAmount;
					break;
			}
		}
	}

	public CharacterPower CheckApplyCharacterPower(CharacterPowerType powerType, ResourceType resourceAffected = default)
	{
		var power = CurrentCharacterConfig.GetPower(powerType, resourceAffected);
		
		if (power == null)
			return null;

		var value = power.GetCharacterPowerValue(PlayerData.GetCharacterLevel(PlayerData.playerName));
		
		switch (power.powerType)
		{
			case CharacterPowerType.LoseXpInsteadOfDeadOnBomb:
				DecreaseXp(Mathf.FloorToInt(value.buffAmount * GetCurrentXp()));
				break;
			case CharacterPowerType.SpeedBoostOnUseHelper:
				_speedBoosterIncrease = value.buffAmount;
				timeEffectSpeedBooster += GamePlay.levelController.GetDurationHelper(resourceAffected);
				speedLinesVFX.Play();
				break;
			case CharacterPowerType.BonusXpOnSmallItems:
				break;
			case CharacterPowerType.CompassOceanGiftItem:
				int itemCount = value.buffAmount < 0
					? GamePlay.levelController.AllSpecialItems.Count
					: (int)value.buffAmount;
				this.RaiseEvent(EventList.GamePlay.TURN_ON_COMPASS_DIRECTOR_OCEAN_GIFT,new TurnOnCompassDirector()
				{
					remainingObjectiveItems = GamePlay.levelController.AllSpecialItems.GetRange(0, itemCount),
					holeTransform = transform
				});
				break;
			case CharacterPowerType.BombShieldOnHelper:
				holeBombShield.SetupShield((int)value.buffAmount, GamePlay.levelController.GetDurationHelper(resourceAffected));
				break;
			case CharacterPowerType.IncreaseCameraSizeOnHelper:
				timeAdditionalCamSize = GamePlay.levelController.GetDurationHelper(resourceAffected);
				_additionalCamSize = value.buffAmount;
				this.RaiseEvent(EventList.GamePlay.UPDATE_CAMERA_TARGET,new UpdateCameraTarget()
				{
					target =  transform,
					//cameraDistance = Settings.BaseCameraDistance  + (level-1) * Settings.CameraDistanceIncreasePerLevel
					// cameraDistance = CalculateIndices(Settings.BaseCameraDistance,m_Level,Settings.CameraDistanceIncreasePerLevel) 
					//                  * (1 + (timeAdditionalCamSize > 0 ? _additionalCamSize : 0))
					cameraDistance = GetCamSize()
				});
				break;
			case CharacterPowerType.IncreaseSizeOnIdle:
				m_characterIdleSizeIncrease = value.buffAmount;
				break;
		}
		
		return power;
	}
	
	#endregion

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
		txtLevel.gameObject.SetActive(true);
	}
	private void TurnOffAllVfx()
	{
		//levelUpVfx.Stop();
		sizeUpVfx.Stop();
		levelUpVfx.Stop();
		speedLinesVFX.Stop();
	}

	#region Character Animation
	public string SHARK_TWEEN_HOLE_SCALE => characterParent.gameObject.GetInstanceID() + "_HOLE_SCALE";
	public void ResetCharacter()
	{
		onInitAnimNewGame = false;
		holePipe.transform.localScale = Vector3.zero;
		holePipe.gameObject.SetActive(false);
		characterGraphics.transform.localScale = Vector3.one;
		characterParent.gameObject.SetActive(true);
		characterAnimator.transform.localPosition = new Vector3(0, defaultPosYCharacter, 0);
		
		m_characterIdleSizeIncrease = 0f;
		m_currentIntervalIdleSizeIncrease = 0f;
		m_characterIdleSizeIncreaseInterval = 1f;
		
		StartAnimationCharacter();
	}

	public void StartAnimationCharacter()
	{
		var obj = GetObjectInDistanceAnimation();
		characterGraphics.transform.localPosition = new Vector3(0, 0, 0);
		characterParent.transform.localPosition = new Vector3(0, 0, 0);
		characterAnimator.transform.localPosition = new Vector3(0, defaultPosYCharacter, 0);
		readyForSharkBite = true;
		if (obj != null)//Have object near
		{
			characterAnimator.Play("Spawn_Game");
			//Hole call from EndAnimationCharacter
			onSharkForm = false;
		}
		else
		{
			onInitAnimNewGame = true;
			holePipe.transform.localScale = Vector3.zero;
			onSharkForm = true;
			characterAnimator.Play("HoleToShark");
		}
	}
	private BaseItem GetObjectInDistanceAnimation()
	{
		return GamePlay.levelController.GetAllItems().FirstOrDefault(x =>
			x != null && Vector3.Distance(x.transform.position, transform.position) < GetDistanceToObject());
	}

	public void PlayEffectSharkDriveIn()
	{
		
		//Apply position because animation diff with pos
		waterSplashFx.Play();
		DOTween.Kill(SHARK_TWEEN_HOLE_SCALE);
		holePipe.gameObject.SetActive(true);
		holePipe.DOScale(Vector3.one, 1.0f).SetId(SHARK_TWEEN_HOLE_SCALE);
	}
	public void EndAnimationCharacter()
	{
		characterParent.gameObject.SetActive(false);
		onInitAnimNewGame = true;
		readyForSharkBite = true;
    }
	public void TurnHoleToShark()
	{
		readyForSharkBite = false;
		DOTween.Kill(SHARK_TWEEN_HOLE_SCALE);
		holePipe.DOScale(Vector3.zero, 0.5f).SetId(SHARK_TWEEN_HOLE_SCALE);
		characterParent.gameObject.SetActive(true);
		characterGraphics.transform.localPosition = new Vector3(0, 0, 0);
		characterAnimator.transform.localPosition = new Vector3(0, defaultPosYCharacter, 0);
		characterAnimator.Play("HoleToShark");
        EventController.RaiseEvent(EventList.Audio.SHARK_UP);
    }
	public void TurnSharkToHole()
	{
		readyForSharkBite = false;
		DOTween.Kill(SHARK_TWEEN_HOLE_SCALE);
		holePipe.DOScale(Vector3.one, 1.0f).SetId(SHARK_TWEEN_HOLE_SCALE);
		characterGraphics.transform.localPosition = new Vector3(0, 0, 0);
		characterParent.transform.localPosition = new Vector3(0, 0, 0);
		characterAnimator.transform.localPosition = new Vector3(0, defaultPosYCharacter, 0);
		characterAnimator.Play("Dive Into Water 0");
    }
	#endregion


}
