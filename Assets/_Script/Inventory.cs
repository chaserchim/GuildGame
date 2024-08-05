using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public enum StorageType { Person, Nonperson, Delivery }
    public StorageType type;
    [SerializeField]
    private List<InventoryEntry> initialItems = new List<InventoryEntry>();

    private Dictionary<Item, int> materials = new Dictionary<Item, int>();

    private void Start()
    {
        // Initialize the dictionary from the list of InventoryEntry objects
        foreach (var entry in initialItems)
        {
            if (entry.item != null)  // Make sure the item is not null
            {
                materials[entry.item] = entry.quantity;
            }
        }
    }

    public bool IsNonpersonStorage()
    {
        return type == StorageType.Nonperson;
    }
    public bool IsDelivery()
    {
        return type == StorageType.Delivery;
    }

    public bool HasRequiredMaterials(Dictionary<Item, int> requiredMaterials)
    {
        foreach (var material in requiredMaterials)
        {
            if (!materials.ContainsKey(material.Key) || materials[material.Key] < material.Value)
            {
                return false;
            }
        }
        return true;
    }

    public void AddMaterial(Item item, int quantity)
    {
        if (materials.ContainsKey(item))
        {
            materials[item] += quantity;
        }
        else
        {
            materials.Add(item, quantity);
        }
    }

    public void RemoveMaterial(Item item, int quantity)
    {
        if (materials.ContainsKey(item))
        {
            materials[item] -= quantity;
            if (materials[item] <= 0)
            {
                materials.Remove(item);
            }
        }
    }

    public Dictionary<Item, int> GetMaterials()
    {
        return new Dictionary<Item, int>(materials); // Return a copy to avoid external modifications
    }
}

[System.Serializable]
public class InventoryEntry
{
    public Item item;  // Make sure Item is a serializable class or scriptable object
    public int quantity;
}
