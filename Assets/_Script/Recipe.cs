using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/Recipe")]
public class Recipe : ScriptableObject
{
    public string recipeName;
    public List<MaterialQuantity> requiredMaterialsList; // List of materials and quantities
    public Item output; // Output item
    public int outputQuantity; // Quantity of the output item
    public int craftTime;

    // Create a dictionary for runtime use
    private Dictionary<Item, int> requiredMaterialsDict;

    private void OnEnable()
    {
        // Convert list to dictionary for runtime access
        requiredMaterialsDict = new Dictionary<Item, int>();
        foreach (var material in requiredMaterialsList)
        {
            if (!requiredMaterialsDict.ContainsKey(material.item))
            {
                requiredMaterialsDict.Add(material.item, material.quantity);
            }
        }
    }

    public Dictionary<Item, int> GetRequiredMaterials()
    {
        return requiredMaterialsDict;
    }

    [System.Serializable]
    public class MaterialQuantity
    {
        public Item item;
        public int quantity;
    }
}
