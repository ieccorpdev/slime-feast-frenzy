using System;
using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;

public interface IBoosterItem
{
     void Init();
     void ApplyEffect();
     void MoveToPos(Vector3 pos);
}
