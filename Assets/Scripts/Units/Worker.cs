using UnityEngine;

public class Worker : MonoBehaviour
{
    public UnitData unitData;

    private ResourceNode targetResource;
    private DropOffPoint targetDropOff;

    private BuildingData buildingToBuild;
    private Vector3 buildPosition;
    private Vector3 buildTargetPosition;

    private int carriedAmount;
    private ResourceType carriedType;

    private bool isGathering;
    private bool isReturning;
    private bool isBuilding;

    private float gatherTimer;

    private UnitMovement movement;

    private void Awake()
    {
        movement = GetComponent<UnitMovement>();
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
        ResetAll();

        targetResource = resource;
        isGathering = true;
        gatherTimer = 0f;

        Vector3 targetPos = GetClosestPoint(resource.transform.position);
        movement.MoveDirect(targetPos);
    }

    private void Gather()
    {
        float dist = Vector2.Distance(transform.position, targetResource.transform.position);

        if (dist > 2.0f)
            return;

        gatherTimer += Time.deltaTime;

        if (gatherTimer < 1f)
            return;

        gatherTimer = 0f;

        int gathered = targetResource.Harvest(1);

        if (gathered <= 0)
        {
            ResetAll();
            return;
        }

        carriedAmount += gathered;
        carriedType = targetResource.resourceType;

        targetResource.PlayGatherEffect();

        if (carriedAmount >= 10)
        {
            isGathering = false;
            FindDropOff();
        }
    }

    private void FindDropOff()
    {
        targetDropOff = FindObjectOfType<DropOffPoint>();

        if (targetDropOff == null)
        {
            ResetAll();
            return;
        }

        isReturning = true;

        Vector3 targetPos = GetClosestPoint(targetDropOff.transform.position);
        movement.MoveDirect(targetPos);
    }

    private void ReturnResources()
    {
        float dist = Vector2.Distance(transform.position, targetDropOff.transform.position);

        if (dist > 2.0f)
            return;

        if (carriedType == ResourceType.Wood)
            ResourceManager.Instance.AddWood(carriedAmount);

        carriedAmount = 0;

        if (targetResource == null)
        {
            ResetAll();
            return;
        }

        isReturning = false;
        isGathering = true;

        Vector3 targetPos = GetClosestPoint(targetResource.transform.position);
        movement.MoveDirect(targetPos);
    }

    public void StartBuilding(BuildingData data, Vector3 pos)
    {
        ResetAll();

        buildingToBuild = data;
        buildPosition = pos;
        isBuilding = true;

        // zapamiêtujemy punkt obok
        buildTargetPosition = GetBuildPosition(pos);

        movement.MoveDirect(buildTargetPosition);
    }

    private void Build()
    {
        // sprawdzamy dystans do pozycji OBOK
        float dist = Vector2.Distance(transform.position, buildTargetPosition);

        if (dist > 0.5f)
            return;

        GameObject obj = Instantiate(buildingToBuild.prefab, buildPosition, Quaternion.identity);

        Building b = obj.GetComponent<Building>();
        if (b != null)
            b.Init(buildingToBuild);

        ResetAll();
    }

    private void ResetAll()
    {
        isGathering = false;
        isReturning = false;
        isBuilding = false;

        targetResource = null;
        targetDropOff = null;
        buildingToBuild = null;

        carriedAmount = 0;
        gatherTimer = 0f;
    }

    public void StopWorkExternal()
    {
        ResetAll();
    }

    private Vector3 GetClosestPoint(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position).normalized;
        return targetPos - dir * 0.6f;
    }

    Vector3 GetBuildPosition(Vector3 buildPos)
    {
        float offset = 2.0f;

        Vector3 bestPos = buildPos;
        float bestDist = float.MaxValue;

        Vector3[] directions = new Vector3[]
        {
        Vector3.up,
        Vector3.down,
        Vector3.left,
        Vector3.right,
        new Vector3(1,1,0).normalized,
        new Vector3(-1,1,0).normalized,
        new Vector3(1,-1,0).normalized,
        new Vector3(-1,-1,0).normalized
        };

        foreach (var dir in directions)
        {
            Vector3 checkPos = buildPos + dir * offset;

            Collider2D hit = Physics2D.OverlapCircle(checkPos, 0.35f, LayerMask.GetMask("Building"));

            if (hit != null)
                continue;

            float dist = Vector3.Distance(transform.position, checkPos);

            if (dist < bestDist)
            {
                bestDist = dist;
                bestPos = checkPos;
            }
        }

        return bestPos;
    }
}