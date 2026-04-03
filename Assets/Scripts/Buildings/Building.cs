using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingData data;
    public GameObject selectionIndicator;

    private float currentHealth;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => data != null ? data.maxHealth : 0f;
    public string BuildingName => data != null ? data.buildingName : gameObject.name;

    private void Start()
    {
        if (data != null && currentHealth <= 0f)
            currentHealth = data.maxHealth;

        if (selectionIndicator != null)
            selectionIndicator.SetActive(false);
    }

    public void Init(BuildingData buildingData)
    {
        data = buildingData;
        currentHealth = data.maxHealth;
    }

    public void Select()
    {
        if (selectionIndicator != null)
            selectionIndicator.SetActive(true);
    }

    public void Deselect()
    {
        if (selectionIndicator != null)
            selectionIndicator.SetActive(false);
    }
}
