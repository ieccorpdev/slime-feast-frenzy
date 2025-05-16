using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using IEC;
using UnityEngine;

public class GameResourceDisplay : MonoBehaviour
{
    public GameObject darkBG;
    public Canvas canvas;
    public GameObject playerAvatar;
    public List<UICurrency> listResourceDisplay = new();
    
    private List<UIMoveOutAnimation> _listObjMoveOut = new();
    
    public bool IsShow { get; private set; }

    private void Awake()
    {
        Pool.Set(GetType(), this);
        InitMoveOutAnimation();
        ToggleDisplayAllResources(false);
        SetAllObjPosOut();
    }

    public UICurrency GetDisplay(ResourceType resourceType)
    {
        return listResourceDisplay.Find(display => display.typeCurrency == resourceType);
    }

    public void ToggleDisplayAllResources(bool isShow = true)
    {
        //ResetPosAllObj();
        IsShow = isShow;
        if (isShow)
        {
            //darkBG.SetActive(false);
            canvas.sortingOrder = 1000;
        }
        playerAvatar.SetActive(isShow);
        foreach (var display in listResourceDisplay)
        {
            display.gameObject.SetActive(isShow);
        }
    }

    public void HideDisplayUnderBG()
    {
        //darkBG.SetActive(true);
        canvas.sortingOrder = -1;
    }

    public void ToggleDisplayOneResource(ResourceType resourceType, bool isShow = true)
    {
        listResourceDisplay.Find(display => display.typeCurrency == resourceType)?.gameObject.SetActive(isShow);
    }

    #region Moveout animation
    private void InitMoveOutAnimation()
    {
        // foreach (var display in listResourceDisplay)
        // {
        //     _listObjMoveOut.Add(new UIMoveOutAnimation(display.GetComponent<RectTransform>(),UIElementMoveDir.UP));
        // }
        
        _listObjMoveOut.Add(new UIMoveOutAnimation(GetComponent<RectTransform>(),UIElementMoveDir.UP));
    }

    private void ResetPosAllObj()
    {
        foreach (var obj in _listObjMoveOut)
        {
            obj.SetObjDefaultPos();
        }
    }

    public void SetAllObjPosOut()
    {
        foreach (var obj in _listObjMoveOut)
        {
            obj.SetObjPosOut();
        }
    }

    public void DoMoveOutAnim(float timeAnim = 0.3f,Action onComplete = null, float delay = 0f, bool doAnim = true)
    {
        for (int i = 0; i < _listObjMoveOut.Count; i++)
        {
            DOTween.Kill($"MV_T{_listObjMoveOut[i].target.GetInstanceID()}");
            if (doAnim)
            {
                _listObjMoveOut[i]
                    .MoveOut(
                    $"MV_T{_listObjMoveOut[i].target.GetInstanceID()}"
                    , timeAnim
                    ,i == _listObjMoveOut.Count - 1 
                        ? () =>
                        {
                            onComplete?.Invoke();
                        } 
                        : null);
            }
            else
            {
                _listObjMoveOut[i].SetObjPosOut();
                if (i == _listObjMoveOut.Count - 1)
                {
                    onComplete?.Invoke();
                }
            }
        }
    }
    
    public void DoMoveBackAnim(float timeAnim = 0.3f,Action onComplete = null, float delay = 0f, bool doAnim = true)
    {
        for (int i = 0; i < _listObjMoveOut.Count; i++)
        {
            DOTween.Kill($"MV_T{_listObjMoveOut[i].target.GetInstanceID()}");
            _listObjMoveOut[i].SetObjPosOut();
            if (doAnim)
            {
                _listObjMoveOut[i]
                    .MoveBack(
                        $"MV_T{_listObjMoveOut[i].target.GetInstanceID()}"
                        , timeAnim
                        ,i == _listObjMoveOut.Count - 1 
                            ? () =>
                            {
                                onComplete?.Invoke();
                            } 
                            : null);
            }
            else
            {
                _listObjMoveOut[i].SetObjDefaultPos();
                if (i == _listObjMoveOut.Count - 1)
                {
                    onComplete?.Invoke();
                }
            }
        }
    }
    
    #endregion
    
}
