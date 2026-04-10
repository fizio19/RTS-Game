using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "RTS/Building Data")]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    public float maxHealth = 800f;
    public float buildTime = 2f;

    [Header("Wymagania technologiczne")]
    public bool isMainBuilding;
    public bool requiresMainBuilding = true;

    [Header("Cost")]
    public int woodCost;
    public int foodCost;
    public int stoneCost;
    public int goldCost;

    public GameObject prefab;
}
