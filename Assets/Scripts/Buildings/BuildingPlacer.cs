using System.Collections.Generic;
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

        if (selectedBuilding.requiresMainBuilding && !selectedBuilding.isMainBuilding && !BuildingRegistry.HasConstructedMainBuilding())
        {
            CancelBuilding();
            return;
        }

        isPlacingBuilding = true;

        UpdatePreview();

        // LPM = budowanie
        if (Input.GetMouseButtonDown(0) && canBuild)
            TryPlaceBuilding();

        // PPM = anuluj
        if (Input.GetMouseButtonDown(1))
            CancelBuilding();
    }

    void UpdatePreview()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        // Snap do grida.
        Vector2Int gridPos = GridManager.Instance.WorldToGrid(mousePos);
        Vector3 snappedPos = GridManager.Instance.GridToWorld(gridPos);

        if (previewObject == null)
        {
            previewObject = Instantiate(selectedBuilding.prefab);
            previewRenderer = previewObject.GetComponentInChildren<SpriteRenderer>();
        }

        previewObject.transform.position = snappedPos;

        CheckBuildValidity(snappedPos);
        UpdateColor();
    }

    void CheckBuildValidity(Vector3 pos)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(pos, new Vector2(1.0f, 1.0f), 0f);

        canBuild = true;

        foreach (Collider2D hit in hits)
        {
            if (previewObject != null && hit.transform.IsChildOf(previewObject.transform))
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
        List<Worker> workers = GetSelectedWorkers();
        if (workers.Count == 0)
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

        PlaceBuilding(workers);
    }

    void PlaceBuilding(List<Worker> workers)
    {
        Vector3 pos = previewObject.transform.position;

        GameObject obj = Instantiate(selectedBuilding.prefab, pos, Quaternion.identity);
        Building building = obj.GetComponent<Building>();

        if (building != null)
            building.StartConstruction(selectedBuilding);

        // Build Assist: wielu workerów może wspólnie budować ten sam obiekt.
        foreach (Worker worker in workers)
            worker.StartBuilding(building);

        CancelBuilding();

        consumeNextLeftMouseUp = true;
        inputLockFrame = Time.frameCount;

        Invoke(nameof(ResetPlacingFlag), 0.1f);
    }

    private List<Worker> GetSelectedWorkers()
    {
        List<Worker> workers = new List<Worker>();

        if (SelectionManager.Instance == null)
            return workers;

        foreach (UnitMovement unit in SelectionManager.Instance.selectedUnits)
        {
            Worker worker = unit.GetComponent<Worker>();
            if (worker != null)
                workers.Add(worker);
        }

        return workers;
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

        previewRenderer.color = canBuild
            ? new Color(0, 1, 0, 0.5f)
            : new Color(1, 0, 0, 0.5f);
    }
}
