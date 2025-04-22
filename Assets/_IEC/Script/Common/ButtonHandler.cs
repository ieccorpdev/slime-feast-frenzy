using System;
using IEC;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHandler : _BaseBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Button btnPlay;
    public Action onPointerUp;
    public Action onPointerDown;
    [Header("Feature Press Down")]
    public bool changeImageOnPointerDown;
    public Sprite spriteBase;
    public Sprite spriteChange;
    public bool smallButtonWhenPress;
    // This function will be called when the pointer is pressed down on the button
    public void Awake()
    {
        btnPlay = GetComponent<Button>();
        spriteBase = btnPlay.image.sprite;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown?.Invoke();
        UpdateImagePress(true);
    }

    private void UpdateImagePress(bool pressed = false)
    {
        if (changeImageOnPointerDown && btnPlay.enabled)
        {
            btnPlay.image.sprite =pressed ? spriteChange : spriteBase;
            btnPlay.image.SetNativeSize();
        }

        if (smallButtonWhenPress)
        {
            gameObject.transform.localScale = Vector3.one * (pressed ? 0.98f: 1f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp?.Invoke();
        UpdateImagePress();
    }
    
}