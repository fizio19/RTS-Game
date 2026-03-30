using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "RTS/Building Data")]
public class BuildingData : ScriptableObject
{
    public string buildingName;

    public float maxHealth;

    public GameObject prefab;

    public int woodCost;
    public int foodCost;
    public int stoneCost;
    public int goldCost;

    public float buildTime;
}