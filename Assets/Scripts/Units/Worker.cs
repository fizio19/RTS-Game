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

        movement.MoveTo(resource.transform.position);
    }

    private void Gather()
    {
        float dist = Vector2.Distance(transform.position, targetResource.transform.position);

        if (dist > 1.2f)
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
        movement.MoveTo(targetDropOff.transform.position);
    }

    private void ReturnResources()
    {
        float dist = Vector2.Distance(transform.position, targetDropOff.transform.position);

        if (dist > 1f)
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

        movement.MoveTo(targetResource.transform.position);
    }

    public void StartBuilding(BuildingData data, Vector3 pos)
    {
        ResetAll();

        buildingToBuild = data;
        buildPosition = pos;
        isBuilding = true;

        movement.MoveTo(pos);
    }

    private void Build()
    {
        float dist = Vector2.Distance(transform.position, buildPosition);

        if (dist > 1f)
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
}