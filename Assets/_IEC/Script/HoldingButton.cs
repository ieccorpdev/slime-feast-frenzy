using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using IEC;
using MoreMountains.NiceVibrations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoldingButton : _BaseBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Range(0.0f, 1.0f)]
    public float seconds = 0.5f;
    public bool stackWhenHold = false;
    public bool doPunchAnimOnHold = true;
    public UnityEvent onPressedOverSeconds;
    public bool buttonPressed;
    private float time = 0;
    private float multipleUpdate;
    private Tween punchTween;

    private void OnEnable()
    {
        StopHolding();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        time = 0;
        multipleUpdate = 0;
        buttonPressed = true;
        PlayerData.Vibrate(HapticTypes.Failure);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopHolding();
    }

    private void OnDisable()
    {
        StopHolding();
    }

    public void StopHolding()
    {
        buttonPressed = false;
        if (punchTween != null && punchTween.IsPlaying() && punchTween.active)
        {
            punchTween.Kill(); // Stop the tween
        }
    }
    private void Update()
    {
        if (buttonPressed)
        {
            time += Time.deltaTime;
            if (time > 0.1f)
            {
                time = 0.1f - seconds;
                if (stackWhenHold)
                {
                    multipleUpdate += 0.5f;
                    for (int i = 0; i < (int)multipleUpdate; i++)
                    {
                        DoEventPress();
                    }
                }
                else
                {
                    DoEventPress();
                }

            }
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DoEventPress();
        }
#endif
    }

    public void DoEventPress()
    {
        onPressedOverSeconds?.Invoke();
        StartPunchScaleLoop();
    }
    
    void StartPunchScaleLoop()
    {
        if (!doPunchAnimOnHold)
            return;
        
        if (punchTween == null || !punchTween.IsPlaying())
        {
            punchTween = gameObject.transform.DOPunchScale((Vector3.down + Vector3.left)*0.05f, 0.1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetId($"PunchScaleLoop{gameObject.GetInstanceID()}"); // Loop infinitely with a Yoyo type
        }
    }

    private void OnDestroy()
    {
        DOTween.Kill($"PunchScaleLoop{gameObject.GetInstanceID()}");
    }
}
