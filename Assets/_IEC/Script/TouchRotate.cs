using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TouchRotate : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject[] _bg;
    private int _tapCount;

    void Start()
    {
        _tapCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            _tapCount++;

            for (int i = 0; i < _bg.Length; i++)
            {
                _bg[i].SetActive(i == _tapCount% _bg.Length);
            }
        }
    }
}
