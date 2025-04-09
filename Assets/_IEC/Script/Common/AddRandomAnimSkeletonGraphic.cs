using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class AddRandomAnimSkeletonGraphic : _BaseBehaviour
    {
        public SpineAnimOT spineAnim;
        public float minAnimInterval, maxAnimInterval;

        public string mainLoopAnimString;
        public List<string> listAnimStringLoop = new();

        private Coroutine m_randomAnimCoroutine;
        
        
        // Start is called before the first frame update
        void Start()
        {
            // if (listAnimStringLoop.Count == 0)
            //     return;
            //
            // if(m_randomAnimCoroutine != null)
            //     StopCoroutine(m_randomAnimCoroutine);
            //     
            // m_randomAnimCoroutine = StartCoroutine(IERandomAnim());
        }

        IEnumerator IERandomAnim()
        {
            //switch min max if max < min
            if (maxAnimInterval < minAnimInterval)
                (minAnimInterval, maxAnimInterval) = (maxAnimInterval, minAnimInterval);
            
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minAnimInterval, maxAnimInterval));

                var randomString = listAnimStringLoop[Random.Range(0, listAnimStringLoop.Count)];
                bool animCompleted = false;
                // spineAnim.AddCallBack(randomString, () =>
                // {
                    spineAnim.SetAnimation(randomString,false, () =>
                    {
                        spineAnim.SetAnimation(mainLoopAnimString, true, ()=>{});
                        animCompleted = true;
                    });
                //});

                
                while (!animCompleted)
                    yield return null;
                
                
            }
            
            StopCoroutine(m_randomAnimCoroutine);
            m_randomAnimCoroutine = null;
        }
        
    }
}
