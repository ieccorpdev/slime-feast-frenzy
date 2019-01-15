using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public interface IComponentMap
    {
        int[] GetArrayObjID();

        bool IsContain(int objID);

        void Remove(int objID);
    }
}