using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "RTS/Unit Data")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public float moveSpeed = 5f;
    public float maxHealth = 100f;
    public int carryCapacity = 10;
    public float gatherSpeed = 1f;

    [Header("Praca")]
    public float buildSpeed = 1f;

    [Header("Cost")]
    public int woodCost;
    public int foodCost;
    public int stoneCost;
    public int goldCost;

    public GameObject prefab;
}
