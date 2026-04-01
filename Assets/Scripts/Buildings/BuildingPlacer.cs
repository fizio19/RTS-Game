using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public static BuildingPlacer Instance;

    public static int inputLockFrame;
    public static bool consumeNextLeftMouseUp = false;

    public BuildingData selectedBuilding;

    private GameObject previewObject;
    private SpriteRenderer previewRenderer;

    private bool canBuild = true;
    private bool isPlacingBuilding = false;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (selectedBuilding == null)
        {
            isPlacingBuilding = false;
            DestroyPreview();
            return;
        }

        isPlacingBuilding = true;

        UpdatePreview();

        // LPM = budowanie
        if (Input.GetMouseButtonDown(0) && canBuild)
        {
            TryPlaceBuilding();
        }

        // PPM = anuluj
        if (Input.GetMouseButtonDown(1))
        {
            CancelBuilding();
        }
    }

    void UpdatePreview()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;

        if (previewObject == null)
        {
            previewObject = Instantiate(selectedBuilding.prefab);
            previewRenderer = previewObject.GetComponent<SpriteRenderer>();
        }

        previewObject.transform.position = pos;

        CheckBuildValidity(pos);
        UpdateColor();
    }

    void CheckBuildValidity(Vector3 pos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, 0.6f);

        canBuild = true;

        foreach (var hit in hits)
        {
            if (previewObject != null && hit.gameObject == previewObject)
                continue;

            if (hit.GetComponentInParent<ResourceNode>() != null ||
                hit.GetComponentInParent<Building>() != null ||
                hit.GetComponentInParent<UnitMovement>() != null)
            {
                canBuild = false;
                return;
            }
        }
    }

    void TryPlaceBuilding()
    {
        if (SelectionManager.Instance.selectedUnits.Count == 0)
        {
            CancelBuilding();
            return;
        }

        bool success = ResourceManager.Instance.SpendResources(
            selectedBuilding.woodCost,
            selectedBuilding.foodCost,
            selectedBuilding.stoneCost,
            selectedBuilding.goldCost
        );

        if (!success)
        {
            CancelBuilding();
            return;
        }

        PlaceBuilding();
    }

    void PlaceBuilding()
    {
        Vector3 pos = previewObject.transform.position;

        foreach (UnitMovement unit in SelectionManager.Instance.selectedUnits)
        {
            Worker worker = unit.GetComponent<Worker>();

            if (worker != null)
            {
                worker.StartBuilding(selectedBuilding, pos);
            }
        }

        CancelBuilding();

        consumeNextLeftMouseUp = true;
        inputLockFrame = Time.frameCount;

        Invoke(nameof(ResetPlacingFlag), 0.1f);
    }

    void ResetPlacingFlag()
    {
        isPlacingBuilding = false;
    }

    void DestroyPreview()
    {
        if (previewObject != null)
            Destroy(previewObject);
    }

    void CancelBuilding()
    {
        DestroyPreview();
        selectedBuilding = null;
    }

    void UpdateColor()
    {
        if (previewRenderer == null)
            return;

        Color c = previewRenderer.color;

        if (canBuild)
        {
            c = new Color(0, 1, 0, 0.5f);
        }
        else
        {
            c = new Color(1, 0, 0, 0.5f);
        }

        previewRenderer.color = c;
    }

    void Start()
    {
        selectedBuilding = null;
    }
}