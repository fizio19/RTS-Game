using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public ResourceType resourceType;
    public int amount = 100;

    public int Harvest(int value)
    {
        int harvested = Mathf.Min(value, amount);
        amount -= harvested;

        if (amount <= 0)
        {
            Destroy(gameObject);
        }

        return harvested;
    }
}