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

    void Start()
    {
        selectedBuilding = null;
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
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        // SNAP DO GRIDA
        Vector2Int gridPos = GridManager.Instance.WorldToGrid(mousePos);
        Vector3 snappedPos = GridManager.Instance.GridToWorld(gridPos);

        if (previewObject == null)
        {
            previewObject = Instantiate(selectedBuilding.prefab);
            previewRenderer = previewObject.GetComponent<SpriteRenderer>();
        }

        previewObject.transform.position = snappedPos;

        CheckBuildValidity(snappedPos);
        UpdateColor();
    }

    void CheckBuildValidity(Vector3 pos)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(pos, new Vector2(1.0f, 1.0f), 0f);

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

        if (canBuild)
        {
            previewRenderer.color = new Color(0, 1, 0, 0.5f);
        }
        else
        {
            previewRenderer.color = new Color(1, 0, 0, 0.5f);
        }
    }
}