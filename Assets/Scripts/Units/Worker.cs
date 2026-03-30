using UnityEngine;

public class Worker : MonoBehaviour
{
    public UnitData unitData;

    private ResourceNode targetResource;
    private DropOffPoint targetDropOff;

    private BuildingData buildingToBuild;
    private Vector3 buildPosition;

    private int carriedAmount;
    private ResourceType carriedType;

    private bool isGathering;
    private bool isReturning;
    private bool isBuilding;

    private UnitMovement movement;

    private Vector3 currentTarget;
    private bool hasMoveTarget;

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
        {
            Gather();
        }
        else if (isReturning && targetDropOff != null)
        {
            ReturnResources();
        }
    }

    public void SetTarget(ResourceNode resource)
    {
        StopWork();

        targetResource = resource;
        isGathering = true;
    }

    public void StartBuilding(BuildingData data, Vector3 position)
    {
        Debug.Log("START BUILDING");

        StopWork();

        buildingToBuild = data;
        buildPosition = position;
        isBuilding = true;
        hasMoveTarget = false;
    }

    private void Build()
    {
        float distance = Vector2.Distance(transform.position, buildPosition);

        // zwiêkszamy zasiêg (wa¿ne)
        if (distance > 2.5f)
        {
            MoveOnce(buildPosition);
            return;
        }

        Debug.Log("BUDUJE OBIEKT");

        hasMoveTarget = false;

        if (buildingToBuild == null || buildingToBuild.prefab == null)
        {
            Debug.LogError("BRAK PREFABU!");
            StopWork();
            return;
        }

        GameObject obj = Instantiate(buildingToBuild.prefab, buildPosition, Quaternion.identity);

        Building building = obj.GetComponent<Building>();
        if (building != null)
            building.Init(buildingToBuild);

        StopWork();
    }

    private void Gather()
    {
        if (targetResource == null)
        {
            StopWork();
            return;
        }

        float distance = Vector2.Distance(transform.position, targetResource.transform.position);

        if (distance > 1f)
        {
            MoveOnce(targetResource.transform.position);
            return;
        }

        hasMoveTarget = false;

        int gathered = targetResource.Harvest((int)unitData.gatherRate);
        carriedAmount += gathered;
        carriedType = targetResource.resourceType;

        if (carriedAmount >= unitData.carryCapacity)
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
            StopWork();
            return;
        }

        isReturning = true;
        hasMoveTarget = false;
    }

    private void ReturnResources()
    {
        if (targetDropOff == null)
        {
            StopWork();
            return;
        }

        float distance = Vector2.Distance(transform.position, targetDropOff.transform.position);

        if (distance > 1f)
        {
            MoveOnce(targetDropOff.transform.position);
            return;
        }

        hasMoveTarget = false;

        if (carriedType == ResourceType.Wood)
            ResourceManager.Instance.AddWood(carriedAmount);

        carriedAmount = 0;

        if (targetResource == null)
        {
            StopWork();
            return;
        }

        isReturning = false;
        isGathering = true;
    }

    private void MoveOnce(Vector3 target)
    {
        if (hasMoveTarget && Vector3.Distance(currentTarget, target) < 0.1f)
            return;

        currentTarget = target;
        hasMoveTarget = true;

        if (movement != null)
            movement.MoveTo(target);
    }

    private void StopWork()
    {
        isGathering = false;
        isReturning = false;
        isBuilding = false;

        targetResource = null;
        targetDropOff = null;
        buildingToBuild = null;

        hasMoveTarget = false;
    }

    public void StopWorkExternal()
    {
        if (isBuilding)
            return;

        isGathering = false;
        isReturning = false;

        targetResource = null;
        targetDropOff = null;
        buildingToBuild = null;
    }
}