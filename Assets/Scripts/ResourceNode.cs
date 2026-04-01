using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public ResourceType resourceType;
    public int amount = 100;

    public GameObject gatherEffectPrefab;

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

    public void PlayGatherEffect()
    {
        if (gatherEffectPrefab == null)
            return;

        GameObject fx = Instantiate(gatherEffectPrefab, transform.position, Quaternion.identity);
        Destroy(fx, 0.5f);
    }
}