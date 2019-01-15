using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public static class Systems
    {
        public static readonly EmptySystem Empty;

        static Systems()
        {
            Empty = new EmptySystem();
        }
    }
}