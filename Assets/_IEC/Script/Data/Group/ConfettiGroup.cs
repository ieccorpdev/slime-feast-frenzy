using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace IEC
{
    public class ConfettiGroup : _BaseBehaviour, IGroup
    {
        public Vector2 areaX;
        public Vector2 areaY;
        public float radiusNoOverlap;

        public void LocateGroup()
        {
            Pool.Set(this);
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        [Button]
        public void ShowConfetti()
        {
            StartCoroutine(IEShowConfetti());
        }

        IEnumerator IEShowConfetti()
        {
            var listPos = new List<Vector3>();
            foreach (Transform child in transform)
            {
                if (child.gameObject.TryGetComponent<ParticleSystem>(out var ps))
                {
                    var pos = Vector3.zero;
                    var isOverlap = false;
                    var count = 0;
                    do
                    {
                        count++;
                        pos = new Vector3(Random.Range(areaX.x, areaX.y), Random.Range(areaY.x, areaY.y), 5f);
                        foreach (var prevPos in listPos)
                        {
                            if (Vector3.Distance(pos, prevPos) < radiusNoOverlap)
                            {
                                isOverlap = true;
                                break;
                            }
                        }
                    }
                    while (isOverlap && count < 10);
                    
                    ps.transform.localPosition = new Vector3(Random.Range(areaX.x, areaX.y), Random.Range(areaY.x, areaY.y), 5f);
                    ps.Play();
                    listPos.Add(pos);
                    yield return new WaitForSeconds(.5f);
                }
            }
        }
    }
}