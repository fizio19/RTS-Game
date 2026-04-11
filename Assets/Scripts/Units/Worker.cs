using UnityEngine;

public class Worker : MonoBehaviour
{
    public UnitData unitData;
    public int attackDamage = 1;

    [Header("Zbieranie")]
    [SerializeField] private float gatherRange = 1.2f;

    [Header("Budowanie")]
    [SerializeField] private float buildDistanceFromOutline = 0.7f;
    [SerializeField] private float hammerEffectInterval = 0.35f;
    [SerializeField] private GameObject buildHammerEffectPrefab;

    private ResourceNode targetResource;
    private DropOffPoint targetDropOff;
    private Building targetBuilding;

    private int carriedAmount;
    private ResourceType carriedType;

    private bool isGathering;
    private bool isReturning;
    private bool isBuilding;

    private float gatherTimer;
    private float buildEffectTimer;

    private float currentHealth;

    private UnitMovement movement;

    public int CarriedAmount => carriedAmount;
    public int CarryCapacity => unitData != null ? unitData.carryCapacity : 10;
    public int AttackDamage => attackDamage;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => unitData != null ? unitData.maxHealth : 100f;
    public string UnitName => unitData != null ? unitData.unitName : gameObject.name;

    private void Awake()
    {
        movement = GetComponent<UnitMovement>();

        if (unitData != null)
            currentHealth = unitData.maxHealth;
        else
            currentHealth = 100f;
    }

    private void Update()
    {
        if (isBuilding)
        {
            Build();
            return;
        }

        if (isGathering && targetResource != null)
            Gather();

        if (isReturning && targetDropOff != null)
            ReturnResources();
    }

    public void SetTarget(ResourceNode resource)
    {
        ResetWorkState();

        targetResource = resource;
        isGathering = true;
        gatherTimer = 0f;

        MoveToResourceRange();
    }

    private void Gather()
    {
        if (targetResource == null)
        {
            ResetWorkState();
            return;
        }

        if (!IsInResourceRange(targetResource))
        {
            MoveToResourceRange();
            return;
        }

        gatherTimer += Time.deltaTime;

        float gatherInterval = unitData != null ? unitData.gatherSpeed : 1f;
        if (gatherTimer < gatherInterval)
            return;

        gatherTimer = 0f;

        int gathered = targetResource.Harvest(1);

        if (gathered <= 0)
        {
            ResetWorkState();
            return;
        }

        carriedAmount += gathered;
        carriedType = targetResource.resourceType;

        targetResource.PlayGatherEffect();

        if (carriedAmount >= CarryCapacity)
        {
            isGathering = false;
            FindDropOff();
        }
    }

    private void FindDropOff()
    {
        DropOffPoint[] points = FindObjectsOfType<DropOffPoint>();
        targetDropOff = GetNearestActiveDropOff(points);

        if (targetDropOff == null)
        {
            ResetWorkState();
            return;
        }

        isReturning = true;

        Vector3 targetPos = GetClosestPoint(targetDropOff.transform.position, 0.8f);
        movement.MoveTo(targetPos);
    }

    private void ReturnResources()
    {
        float dist = Vector2.Distance(transform.position, targetDropOff.transform.position);

        if (dist > 1.8f)
            return;

        switch (carriedType)
        {
            case ResourceType.Wood:
                ResourceManager.Instance.AddWood(carriedAmount);
                break;
            case ResourceType.Food:
                ResourceManager.Instance.AddFood(carriedAmount);
                break;
            case ResourceType.Stone:
                ResourceManager.Instance.AddStone(carriedAmount);
                break;
            case ResourceType.Gold:
                ResourceManager.Instance.AddGold(carriedAmount);
                break;
        }

        carriedAmount = 0;

        if (targetResource == null)
        {
            ResetWorkState();
            return;
        }

        isReturning = false;
        isGathering = true;
        MoveToResourceRange();
    }

    public void StartBuilding(Building building)
    {
        if (building == null)
            return;

        ResetWorkState();

        targetBuilding = building;
        isBuilding = true;
        buildEffectTimer = 0f;

        MoveToBuildRange();
    }

    private void Build()
    {
        if (targetBuilding == null)
        {
            ResetWorkState();
            return;
        }

        if (targetBuilding.IsConstructed)
        {
            ResetWorkState();
            return;
        }

        Vector3 desiredBuildPos = targetBuilding.GetClosestBuildPoint(transform.position, buildDistanceFromOutline);

        if (Vector2.Distance(transform.position, desiredBuildPos) > 0.35f)
        {
            movement.MoveDirect(desiredBuildPos);
            return;
        }

        // Prędkość budowania jednostki, modyfikowana danymi jednostki.
        float buildSpeed = unitData != null ? unitData.buildSpeed : 1f;
        targetBuilding.AddBuildProgress(Time.deltaTime * buildSpeed);

        buildEffectTimer += Time.deltaTime;
        if (buildEffectTimer >= hammerEffectInterval)
        {
            buildEffectTimer = 0f;
            SpawnBuildEffect();
        }

        if (targetBuilding.IsConstructed)
            ResetWorkState();
    }

    private void SpawnBuildEffect()
    {
        if (buildHammerEffectPrefab == null)
            return;

        // Efekt uderzenia pojawia się przed pracownikiem, w kierunku budynku.
        Vector3 direction = targetBuilding != null
            ? (targetBuilding.transform.position - transform.position).normalized
            : Vector3.right;

        if (direction.sqrMagnitude < 0.01f)
            direction = Vector3.right;

        Vector3 effectPosition = transform.position + direction * 1.00f;
        GameObject fx = Instantiate(buildHammerEffectPrefab, effectPosition, Quaternion.identity);
        Destroy(fx, 0.5f);
    }

    private void ResetWorkState()
    {
        isGathering = false;
        isReturning = false;
        isBuilding = false;

        targetResource = null;
        targetDropOff = null;
        targetBuilding = null;

        gatherTimer = 0f;
        buildEffectTimer = 0f;
    }

    public void StopWorkExternal()
    {
        ResetWorkState();
    }

    private void MoveToBuildRange()
    {
        if (targetBuilding == null)
            return;

        Vector3 buildPos = targetBuilding.GetClosestBuildPoint(transform.position, buildDistanceFromOutline);
        movement.MoveTo(buildPos);
    }

    private void MoveToResourceRange()
    {
        if (targetResource == null)
            return;

        Vector3 targetPos = GetResourceGatherPoint(targetResource);
        movement.MoveTo(targetPos);
    }

    private bool IsInResourceRange(ResourceNode node)
    {
        Collider2D col = node.GetComponentInChildren<Collider2D>();
        if (col == null)
            return Vector2.Distance(transform.position, node.transform.position) <= gatherRange;

        Vector3 closest = col.ClosestPoint(transform.position);
        return Vector2.Distance(transform.position, closest) <= gatherRange;
    }

    private Vector3 GetResourceGatherPoint(ResourceNode node)
    {
        Collider2D col = node.GetComponentInChildren<Collider2D>();
        if (col == null)
            return GetClosestPoint(node.transform.position, gatherRange);

        Vector3 closest = col.ClosestPoint(transform.position);
        Vector3 dir = (closest - node.transform.position).normalized;

        if (dir.sqrMagnitude < 0.0001f)
            dir = (transform.position - node.transform.position).normalized;

        if (dir.sqrMagnitude < 0.0001f)
            dir = Vector3.right;

        return closest + dir * 0.25f;
    }

    private DropOffPoint GetNearestActiveDropOff(DropOffPoint[] points)
    {
        DropOffPoint best = null;
        float bestDist = float.MaxValue;

        foreach (DropOffPoint point in points)
        {
            if (point == null || !point.enabled || !point.gameObject.activeInHierarchy)
                continue;

            Building building = point.GetComponent<Building>();
            if (building != null && !building.IsConstructed)
                continue;

            float dist = Vector2.Distance(transform.position, point.transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                best = point;
            }
        }

        return best;
    }

    private Vector3 GetClosestPoint(Vector3 targetPos, float offset)
    {
        Vector3 dir = (targetPos - transform.position).normalized;

        if (dir.sqrMagnitude < 0.0001f)
            dir = Vector3.right;

        return targetPos - dir * offset;
    }
}
