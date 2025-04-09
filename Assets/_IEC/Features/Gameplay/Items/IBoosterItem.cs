using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;

public interface IBoosterItem
{
    public void Init();
    public void ApplyEffect();
    public void MoveToPos(Vector3 pos);
}
