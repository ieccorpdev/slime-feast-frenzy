using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;

public class TutorialJourney : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> listGameObjectEffect;
    public bool canDisable;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show()
    {
        canDisable = false;
        foreach (var obj in listGameObjectEffect)
        {
            obj.gameObject.SetActive(false);
        }
        StartCoroutine(IEShow());
    }

    IEnumerator IEShow()
    {
        float timeEffect = 0.3f;
        foreach (var obj in listGameObjectEffect)
        {
            TweenControl.GetInstance().ScaleFromZero(obj,timeEffect);
            yield return new WaitForSeconds(timeEffect);
        }
        canDisable = true;
    }
}
