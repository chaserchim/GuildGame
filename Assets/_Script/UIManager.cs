using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TMP_Text stationText;
    public TMP_Text recipeText;

    public void DisplayStationUI(CraftingStation station)
    {
        stationText.text = $"--- {station.stationName} ---\n" +
                           $"Capacity: {station.capacity}\n" +
                           $"Speed: {station.speed}\n" +
                           $"Efficiency: {station.efficiency}\n" +
                           $"Queue: {station.queue.Count} tasks";
    }

    public void DisplayRecipeUI(Recipe recipe)
    {
        string ingredients = "";
        var requiredMaterials = recipe.GetRequiredMaterials(); // Get the dictionary

        foreach (var ingredient in requiredMaterials)
        {
            ingredients += $"{ingredient.Key.itemName}: {ingredient.Value}\n"; // Assuming `Item` has an `itemName` property or similar
        }

        recipeText.text = $"Recipe: {recipe.recipeName}\n" +
                          "Ingredients:\n" + ingredients +
                          $"Produces: {recipe.output.itemName} x {recipe.outputQuantity}"; // Assuming `Item` has an `itemName` property or similar
    }
}
