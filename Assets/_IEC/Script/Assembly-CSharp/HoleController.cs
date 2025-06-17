using _IEC.Features.Gameplay;
using UnityEngine;
using IEC;
using System.Collections;

public class HoleController : _BaseBehaviour
{
    [SerializeField] private HoleTransformer transformer;
    [SerializeField] private RectTransform direction;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform holeCharacter;

    private Hole m_Hole;
    private bool wasMoving = false;
    private float nextCallDelay = 0f;

    private Coroutine movingCoroutine;

    public bool IsMoving => wasMoving;

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
        if (GamePlay.levelController.CurrentState == LevelController.State.Lose ||
            GamePlay.levelController.CurrentState == LevelController.State.Win)
        {
            Move(Vector3.zero);
            direction.gameObject.SetActive(false);
            nextCallDelay = 0;

            if (wasMoving)
            {
                StopTryCallCoroutine();
                wasMoving = false;
            }
            return;
        }

        transform.position = transformer.transform.position;
        var dir = Joystick.Direction;
        
        if(m_Hole.characterAnimator != null)
            m_Hole.characterAnimator.SetFloat("moveAxis", dir.magnitude);

        bool isCurrentlyMoving = dir.magnitude >= Mathf.Epsilon;

        if (!isCurrentlyMoving)
        {
            Move(Vector3.zero);
            direction.gameObject.SetActive(false);
            nextCallDelay = 0;

            if (wasMoving)
            {
                StopTryCallCoroutine();
                wasMoving = false;
            }
            return;
        }

        if (!wasMoving && isCurrentlyMoving)
        {
            TryCallOnStartMoving();
        }

        wasMoving = isCurrentlyMoving;

        direction.gameObject.SetActive(true);

        var angleCharacter = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        holeCharacter.localRotation = Quaternion.Euler(0, angleCharacter, 0);

        var angle = Vector3.SignedAngle(Vector3.right * dir.x + Vector3.forward * dir.y, Vector3.right, Vector3.up);
        direction.localEulerAngles = Vector3.forward * (angle - 90);

        Move(dir.normalized);
    }

    private void TryCallOnStartMoving()
    {
        if (!wasMoving && nextCallDelay != 0)
        {
            return;
        }

        //EventController.RaiseEvent(EventList.Audio.SHARK_SWIM);
        nextCallDelay = Random.Range(5f, 7f);
        StopTryCallCoroutine(); // Ensure no duplicate coroutines
        movingCoroutine = StartCoroutine(DelayedCallTryCallOnStartMoving(nextCallDelay));
    }

    private IEnumerator DelayedCallTryCallOnStartMoving(float delay)
    {
        yield return new WaitForSeconds(delay);
        TryCallOnStartMoving();
    }

    private void StopTryCallCoroutine()
    {
        if (movingCoroutine != null)
        {
            StopCoroutine(movingCoroutine);
            movingCoroutine = null;
        }
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
