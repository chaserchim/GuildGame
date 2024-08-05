using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingStation : MonoBehaviour
{
    public string stationName;
    public int capacity;
    public float speed; // Time in seconds per craft
    public float efficiency; // Efficiency multiplier
    public bool isOccupied;

    public Queue<CraftingTask> queue = new Queue<CraftingTask>();
    public bool isOperational = true;

    private void Start()
    {
        AddTask(ItemDatabase.Instance.allItems[1].itemRecipe, 1);
    }

    public void AddTask(Recipe recipe, int quantity)
    {
        if (queue.Count < capacity)
        {
            queue.Enqueue(new CraftingTask(this, recipe, quantity));
            Debug.Log($"Added task: {recipe.output}, Quantity: {quantity}");
        }
        else
        {
            Debug.Log("Queue is full!");
        }
    }

    private IEnumerator CraftingCoroutine(Recipe recipe, int quantity)
    {
        float timeToCraft = speed * quantity / efficiency;
        Debug.Log($"Started crafting {quantity} of {recipe.output}. Time: {timeToCraft}s");
        yield return new WaitForSeconds(timeToCraft);

        // Handle resource deduction and item production
        Debug.Log($"Crafted {quantity} of {recipe.output}");

        // Optionally: Notify other systems about the completion of crafting
    }

    public void UpgradeStation(int newCapacity, float newSpeed, float newEfficiency)
    {
        capacity = newCapacity;
        speed = newSpeed;
        efficiency = newEfficiency;
        Debug.Log($"{stationName} upgraded: Capacity={capacity}, Speed={speed}, Efficiency={efficiency}.");
    }

    public void MaintainStation()
    {
        isOperational = true;
        Debug.Log($"{stationName} has been maintained and is now operational.");
    }

    public CraftingTask GetFirstTask()
    {
        if (queue.Count > 0)
        {
            return queue.Peek(); // Returns the first task in the queue without removing it
        }
        else
        {
            return null; // Or handle the case where the queue is empty
        }
    }

    public Recipe GetFirstRecipe()
    {
        CraftingTask firstTask = GetFirstTask();
        if (firstTask != null)
        {
            return firstTask.recipe; // Return the recipe associated with the first task
        }
        else
        {
            return null; // Or handle the case where there is no recipe
        }
    }
}

[System.Serializable]
public class CraftingTask
{
    public CraftingStation station;
    public Recipe recipe;
    public int quantity;

    public CraftingTask(CraftingStation station, Recipe recipe, int quantity)
    {
        this.station = station;
        this.recipe = recipe;
        this.quantity = quantity;
    }
}
