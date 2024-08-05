using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Crafting/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public int value; // Value of the item
    public Recipe itemRecipe;

    public Item(string name, int value)
    {
        itemName = name;
        this.value = value;
    }
}
