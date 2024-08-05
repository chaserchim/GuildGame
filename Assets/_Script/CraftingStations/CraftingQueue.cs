using System.Collections.Generic;
using UnityEngine;

public class CraftingQueue : MonoBehaviour
{
    private Queue<CraftingTask> queue = new Queue<CraftingTask>();

    public void AddToQueue(CraftingStation station, Recipe recipe, int quantity)
    {
        if (station)
        {
            CraftingTask newTask = new CraftingTask(station, recipe, quantity);
            station.AddTask(recipe, quantity); // Add to the station’s queue
            queue.Enqueue(newTask); // Add to the queue system
        }
        else
        {
            Debug.Log("Invalid crafting station!");
        }
    }

    public void ProcessAll()
    {
        while (queue.Count > 0)
        {
            CraftingTask task = queue.Dequeue();
            // Station will process its queue automatically through ProcessQueueCoroutine
            Debug.Log($"Task for station {task.station.stationName} has been dequeued.");
        }
    }
}
