using _IEC.Features.Gameplay;
using IEC;
using UnityEngine;

public class HoleController : _BaseBehaviour
{
	[SerializeField] private HoleTransformer transformer;
	[SerializeField] private RectTransform direction;
	[SerializeField] private Rigidbody rb;
	[SerializeField] private Transform holeCharacter;
	private Hole m_Hole;
	
	private void Awake()
	{
		m_Hole = GetComponent<Hole>();
	}

	public void SetPosition(Vector3 position)
	{
		transformer.transform.position = position;
		transform.position = position;
	}

	public void SetScale(float scale)
	{
		transformer.transform.localScale = Vector3.one * scale;
	}
	
	private void Update()
	{
		if (GamePlay.levelController.CurrentState == LevelController.State.Lose ||GamePlay.levelController.CurrentState == LevelController.State.Win )
		{
			Move(Vector3.zero);
			return;
		}
    
		transform.position = transformer.transform.position;
		var dir = Joystick.Direction;
		m_Hole.characterAnimator.SetFloat("moveAxis",dir.magnitude);
		if (dir.magnitude < Mathf.Epsilon)
		{
			Move(Vector3.zero);
			direction.gameObject.SetActive(false);
			return;
		}
		direction.gameObject.SetActive(true);
		
		//Xoay object theo hướng joystick
		var angleCharacter = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;  // Tính góc quay theo hướng joystick
		holeCharacter.localRotation = Quaternion.Euler(0, angleCharacter, 0); // Xoay holeCharacter theo trục Y

		var angle = Vector3.SignedAngle(Vector3.right * dir.x + Vector3.forward * dir.y,Vector3.right,Vector3.up);
		direction.localEulerAngles = Vector3.forward * (angle - 90);
		Move(dir.normalized);
	}


	private void FixedUpdate()
	{
		rb.MovePosition(transformer.transform.position);
	}

	private void Move(Vector2 dir)
	{
		var velocity = (dir.x * Vector3.right + dir.y * Vector3.forward) * m_Hole.Speed;
		transformer.Move(velocity);
	}
}
