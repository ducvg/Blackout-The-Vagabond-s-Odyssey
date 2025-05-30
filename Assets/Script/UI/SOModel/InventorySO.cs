using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class InventorySO : ScriptableObject
{
    [SerializeField]
    public List<InventoryItem> inventoryItems;

    [field: SerializeField]
    public int size { get; private set; } = 10;

    public event Action<Dictionary<int, InventoryItem>> OnInventoryChanged;

    public void Initialize()
    {
        inventoryItems = new List<InventoryItem>();
        for (int i = 0; i < size; i++)
        {
            inventoryItems.Add(InventoryItem.GetEmptyItem());
        }
    }

    public int AddItem(ItemSO item, int quantity)
    {
        if (item.IsStackable == false)
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (IsInventoryFull())
                    return quantity;
                while (quantity > 0 && IsInventoryFull() == false)
                {
                    quantity -= AddItemToFirstFreeSlot(item, 1);
                }
                InformAboutChange();
                return quantity;
            }
        }
        quantity = AddStackableItem(item, quantity);
        InformAboutChange();

        return quantity;
    }

    public int AddItemToFirstFreeSlot(ItemSO item, int quantity)
    {
        InventoryItem newItem = new InventoryItem
        {
            item = item,
            quantity = quantity,
        };

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].isEmpty)
            {
                inventoryItems[i] = newItem;
                return quantity;
            }
        }
        return 0;
    }

    public bool IsInventoryFull()
        => inventoryItems.Where(item => item.isEmpty).Any() == false;

    public int AddStackableItem(ItemSO item, int quantity)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].isEmpty)
                continue;
            if (inventoryItems[i].item.ID == item.ID)
            {
                int amoutToTake = 
                    inventoryItems[i].item.maxStackSize - inventoryItems[i].quantity;

                if (quantity > amoutToTake)
                {
                    inventoryItems[i] = inventoryItems[i]
                        .ChangeQuantity(inventoryItems[i].item.maxStackSize);
                    quantity -= amoutToTake;
                }
                else
                {
                    inventoryItems[i] = inventoryItems[i]
                        .ChangeQuantity(inventoryItems[i].quantity + quantity);
                    InformAboutChange();
                    return 0;
                }
            }
        }
        while (quantity > 0 && IsInventoryFull() == false)
        {
            int newQuantity = Mathf.Clamp(quantity, 0, item.maxStackSize);
            quantity -= newQuantity;
            AddItemToFirstFreeSlot(item, newQuantity);
        }

        return quantity;
    }

    public Dictionary<int, InventoryItem> GetCurrentState()
    {
        Dictionary<int, InventoryItem> returnValue = new Dictionary<int, InventoryItem>();
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].isEmpty) continue;
            returnValue[i] = inventoryItems[i];
        }
        return returnValue;
    }

    public InventoryItem GetItemAt(int itemIndex)
    {
        return inventoryItems[itemIndex];
    }

    public void AddItem(InventoryItem item)
    {
        AddItem(item.item, item.quantity);
    }

    public void SwapItems(int itemIndex1, int itemIndex2)
    {
        InventoryItem item1 = inventoryItems[itemIndex1];
        inventoryItems[itemIndex1] = inventoryItems[itemIndex2];
        inventoryItems[itemIndex2] = item1;
        InformAboutChange();
    }

    public void InformAboutChange()
    {
        OnInventoryChanged?.Invoke(GetCurrentState());
    }

    public void RemoveItem(int itemIndex, int amount)
    {
        if (inventoryItems.Count > itemIndex)
        {
            if (inventoryItems[itemIndex].isEmpty)
                return;
            int reminder = inventoryItems[itemIndex].quantity - amount;
            if (reminder <= 0)
            {
                inventoryItems[itemIndex] = InventoryItem.GetEmptyItem();
            }
            else inventoryItems[itemIndex] = inventoryItems[itemIndex]
                    .ChangeQuantity(reminder);

            InformAboutChange();
        }
    }
}

[System.Serializable]
public struct InventoryItem
{
    public int quantity;
    public ItemSO item;
    public bool isEmpty => item == null;

    public InventoryItem ChangeQuantity(int newQuantity)
    {
        return new InventoryItem
        {
            item = this.item,
            quantity = newQuantity
        };
    }

    public static InventoryItem GetEmptyItem()
        => new InventoryItem
        {
            item = null,
            quantity = 0
        };

}
