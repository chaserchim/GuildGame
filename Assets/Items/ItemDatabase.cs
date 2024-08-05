using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    public List<Item> allItems; // List of all items in the game

    private Dictionary<string, Item> itemDictionary;

    private void Awake()
    {
        Instance = this;

        itemDictionary = new Dictionary<string, Item>();

        foreach (var item in allItems)
        {
            if (!itemDictionary.ContainsKey(item.itemName))
            {
                itemDictionary.Add(item.itemName, item);
            }
            else
            {
                Debug.LogWarning($"Duplicate item found: {item.itemName}");
            }
        }
    }

    public Item GetItemByName(string itemName)
    {
        if (itemDictionary.TryGetValue(itemName, out Item item))
        {
            return item;
        }
        else
        {
            Debug.LogError($"Item not found: {itemName}");
            return null;
        }
    }
}
