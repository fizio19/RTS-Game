using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "RTS/Unit Data")]
public class UnitData : ScriptableObject
{
    public string unitName;

    public float maxHealth;
    public float moveSpeed;

    public float attackDamage;
    public float attackRange;
    public float attackRate;

    public float gatherRate;
    public float carryCapacity;

    public GameObject prefab;
}