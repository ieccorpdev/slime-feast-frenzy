using UnityEngine;

public class Mechanic_Balloon : MonoBehaviour
{
	[SerializeField]
	private string _id;

	[SerializeField]
	private Sprite _icon;

	[SerializeField]
	private Mechanic_BalloonType _type;

	[SerializeField]
	private LineRenderer _line;

	[SerializeField]
	private VisibilityObservable _visibilityObservable;

	[SerializeField]
	private Transform _balloonTransform;

	[SerializeField]
	private AnimationCurve _dragCurve;

	private RangeMinMax _lineHeightRange;

	private RangeMinMaxInt _windInitialSpeedRange;

	private RangeMinMax _windConstantForceRange;

	private RangeMinMax _windSpeedRange;

	private RangeMinMax _windPowerRange;

	private float _lineHeight;

	private int _lineSegmentCount;

	private float _heightPerSegment;

	private float _windConstantForce;

	private float _windSpeed;

	private float _windInitialSpeed;

	private float _windPower;

	private int _colorIndex;

	private Transform _cachedTransform;

	private Vector3 _oldPosition;

	private Vector3 _deltaMovement;

	private Vector3[] _linePositions;

	private bool _isInitialized;

	public string Id => null;

	public Sprite Icon => null;
	

	public Mechanic_BalloonType Type => default(Mechanic_BalloonType);

	public float Height => 0f;

	public bool IsTall { get; set; }

	public void Start()
	{
	}

	public void OnUpdate()
	{
	}
}
