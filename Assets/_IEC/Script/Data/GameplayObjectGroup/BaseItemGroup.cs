using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _IEC.Features.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;

namespace IEC
{
    public class BaseItemGroup : _BaseBehaviour
    {
        public Bounds Bounds { get; private set; }

        public List<BaseItem> listAllItems = new List<BaseItem>();

        [ShowInInspector]public float TotalExp { get; private set; }

        private void Start()
        {
            Setup();
        }

        [Button]
        public void Setup()
        {
            listAllItems = GetComponentsInChildren<BaseItem>().ToList();
            Bounds = GroundExtend.CalculateBounds(listAllItems);
            TotalExp = listAllItems.Sum(item => item.ItemDefinition.GetXp());
        }
    }
}
