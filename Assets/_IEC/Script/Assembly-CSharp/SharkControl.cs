using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;

public class SharkControl : MonoBehaviour
{

    public void DriveInWater()
    {
      //Call by animator triggerd event 
      Pool.Get<GamePlayGroup>().levelController.GetHolePlayer().PlayEffectSharkDriveIn();
    }
    public void EndAnimation()
    {
        //Call by animator triggerd event 
        Pool.Get<GamePlayGroup>().levelController.GetHolePlayer().EndAnimationCharacter();
    }
}
