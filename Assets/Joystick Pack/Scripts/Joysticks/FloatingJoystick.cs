using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    public GameObject tutorial;

    protected void Awake()
    {
        Pool.Set<Joystick>(this);
        background.gameObject.SetActive(false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
        tutorial.SetActive(false);
        StopAllCoroutines();
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(false);
        base.OnPointerUp(eventData);
        StartCoroutine(DelayShow());
    }

    IEnumerator DelayShow()
    {
        yield return new WaitForSeconds(3f);
        if (!PlayableController.Instance.isEndGame)
            tutorial.SetActive(true);
    }
}