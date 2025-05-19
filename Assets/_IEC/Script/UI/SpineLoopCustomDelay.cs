using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineLoopCustomDelay : MonoBehaviour
{
    // Start is called before the first frame update
    public SpineAnimOT spine;
    public List<string> listAnimAloop;
    public List<float> delayEachanim;
    private int index = 0;
    void Start()
    {
        StartCoroutine(DelayAnimLoop());
    }

    public IEnumerator DelayAnimLoop()
    {
        while (true)
        {
            spine.SetAnimation(listAnimAloop[index], false, () =>
            {
               
            });
            yield return new WaitForSeconds(delayEachanim[index]);
            index++;
            index = index >= listAnimAloop.Count ? 0 : index;
        }
    }
}
