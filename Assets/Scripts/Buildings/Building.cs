using UnityEngine;

public class Building : MonoBehaviour
{
    public BuildingData data;
    public GameObject selectionIndicator;

    [Header("Etapy budowy")]
    [SerializeField] private GameObject floorStage;
    [SerializeField] private GameObject wallsStage;
    [SerializeField] private GameObject roofStage;
    [SerializeField] private GameObject finalStage;

    private float currentHealth;
    private float buildProgress;
    private bool isConstructed;

    private Collider2D cachedCollider;
    private DropOffPoint dropOffPoint;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => data != null ? data.maxHealth : 0f;
    public string BuildingName => data != null ? data.buildingName : gameObject.name;
    public float BuildProgress => buildProgress;
    public bool IsConstructed => isConstructed;

    private void Awake()
    {
        cachedCollider = GetComponentInChildren<Collider2D>();
        dropOffPoint = GetComponent<DropOffPoint>();
    }

    private void Start()
    {
        if (selectionIndicator != null)
            selectionIndicator.SetActive(false);

        if (data != null && currentHealth <= 0f)
        {
            // Dla budynków postawionych już jako ukończone w scenie.
            SetConstructedState(true);
        }
    }

    private void OnDestroy()
    {
        BuildingRegistry.Unregister(this);
    }

    public void Init(BuildingData buildingData)
    {
        data = buildingData;
        SetConstructedState(true);
    }

    public void StartConstruction(BuildingData buildingData)
    {
        data = buildingData;
        buildProgress = 0f;
        currentHealth = 1f;
        SetConstructedState(false);
        UpdateConstructionVisuals();
    }

    public bool AddBuildProgress(float deltaBuildTime)
    {
        if (isConstructed || data == null)
            return isConstructed;

        float buildDuration = Mathf.Max(0.01f, data.buildTime);
        buildProgress += deltaBuildTime / buildDuration;
        buildProgress = Mathf.Clamp01(buildProgress);

        // Punkty życia rosną proporcjonalnie do postępu budowy.
        currentHealth = Mathf.Lerp(1f, data.maxHealth, buildProgress);

        UpdateConstructionVisuals();

        if (buildProgress >= 1f)
        {
            SetConstructedState(true);
            return true;
        }

        return false;
    }

    public Vector3 GetClosestBuildPoint(Vector3 workerPosition, float buildDistance)
    {
        if (cachedCollider == null)
            return transform.position;

        // Dystans budowania liczony od obrysu budynku, aby worker nie wchodził w środek.
        Vector3 closest = cachedCollider.ClosestPoint(workerPosition);
        Vector3 direction = (closest - transform.position);

        if (direction.sqrMagnitude < 0.0001f)
            direction = (workerPosition - transform.position).normalized;

        if (direction.sqrMagnitude < 0.0001f)
            direction = Vector3.right;

        direction.Normalize();
        return closest + direction * buildDistance;
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

    private void SetConstructedState(bool constructed)
    {
        isConstructed = constructed;

        if (constructed)
        {
            buildProgress = 1f;
            currentHealth = data != null ? data.maxHealth : currentHealth;
            BuildingRegistry.Register(this);
        }
        else
        {
            BuildingRegistry.Unregister(this);
        }

        if (dropOffPoint != null)
            dropOffPoint.enabled = constructed;

        UpdateConstructionVisuals();
    }

    private void UpdateConstructionVisuals()
    {
        bool showFloor = buildProgress > 0f;
        bool showWalls = buildProgress >= 0.2f;
        bool showRoof = buildProgress >= 0.6f;
        bool showFinal = isConstructed;

        SetActiveIfAssigned(floorStage, showFloor);
        SetActiveIfAssigned(wallsStage, showWalls);
        SetActiveIfAssigned(roofStage, showRoof);
        SetActiveIfAssigned(finalStage, showFinal);
    }

    private void SetActiveIfAssigned(GameObject stage, bool active)
    {
        if (stage != null)
            stage.SetActive(active);
    }
}
