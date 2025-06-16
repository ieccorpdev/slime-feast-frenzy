
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _IEC.Features.ExportLevel.OriginalLevel
{
    [Serializable]
    public class Position
    {
        public float x;
        public float y;
        public float z;

        public Position()
        {
            
        }
        
        public Position(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Position(Vector3 pos)
        {
            this.x = pos.x;
            this.y = pos.y;
            this.z = pos.z;
        }
        
        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [Serializable]
    public class Rotation
    {
        public float x;
        public float y;
        public float z;

        public Rotation()
        {
            
        }
        
        public Rotation(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Rotation(Vector3 eulerAngles)
        {
            this.x = eulerAngles.x;
            this.y = eulerAngles.y;
            this.z = eulerAngles.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [Serializable]
    public class Scale
    {
        public float x;
        public float y;
        public float z;
        
        public Scale()
        {
            
        }
        
        public Scale(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        
        public Scale(Vector3 scale)
        {
            this.x = scale.x;
            this.y = scale.y;
            this.z = scale.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    public class ItemData
    {
        public Position pos { get; set; }
        public Rotation rot { get; set; }
        public Scale scale { get; set; }
        public bool sk { get; set; }
        public string cont { get; set; }
        public int oLvl { get; set; }
    }

    public class InstantiateData
    {
        public string itemId { get; set; }
        public List<ItemData> itemData { get; set; }
    }

    public class Root
    {
        public List<InstantiateData> InstantiateData { get; set; }
    }

}
