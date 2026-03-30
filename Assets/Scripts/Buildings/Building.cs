using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingData data;

    private float currentHealth;

    public void Init(BuildingData buildingData)
    {
        data = buildingData;
        currentHealth = data.maxHealth;
    }
}