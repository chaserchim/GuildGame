using UnityEngine;

public class Resource : MonoBehaviour
{
    public string resourceName;
    public int quantity;

    public bool UseResource(int amount)
    {
        if (quantity >= amount)
        {
            quantity -= amount;
            return true;
        }
        return false;
    }

    public void AddResource(int amount)
    {
        quantity += amount;
    }
}
