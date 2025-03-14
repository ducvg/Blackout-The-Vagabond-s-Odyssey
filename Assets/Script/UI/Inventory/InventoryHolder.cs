﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Script.UI.Inventory
{
    [System.Serializable]
    public class InventoryHolder : MonoBehaviour
    {
        [SerializeField] private int inventorySize;
        [SerializeField] protected InventorySystem inventorySystem;

        public InventorySystem InventorySystem => inventorySystem;

        public static UnityAction<InventorySystem> OnDynamicInventoryDisplayRequested;

        private void Awake()
        {
            inventorySystem = new InventorySystem(inventorySize);
        }
    }
}
