using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public static BuildingPlacer Instance;
    public static int inputLockFrame = -1;
    public static bool consumeNextLeftMouseUp = false;

    public BuildingData selectedBuilding;
    public bool isPlacingBuilding = false;
    public bool IsInBuildMode()
    {
        return selectedBuilding != null;
    }

    private GameObject previewObject;
    private SpriteRenderer previewRenderer;

    private bool canBuild = true;

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
            DestroyPreview();
            return;
        }

        UpdatePreview();

        // LPM = buduj (zablokuj resztę w tej klatce)
        if (Input.GetMouseButtonDown(0))
        {
            inputLockFrame = Time.frameCount;
            consumeNextLeftMouseUp = true;

            if (canBuild)
                TryPlaceBuilding();

            return;
        }

        // PPM = anuluj (zablokuj resztę w tej klatce)
        if (Input.GetMouseButtonDown(1))
        {
            inputLockFrame = Time.frameCount;

            CancelBuilding();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, 0.5f);

        canBuild = true;

        foreach (var hit in hits)
        {
            if (hit.GetComponent<ResourceNode>() != null ||
                hit.GetComponent<Building>() != null)
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
            Debug.Log("Brak jednostki!");
            return; // NIE anulujemy trybu
        }

        bool success = ResourceManager.Instance.SpendResources(
            selectedBuilding.woodCost,
            selectedBuilding.foodCost,
            selectedBuilding.stoneCost,
            selectedBuilding.goldCost
        );

        if (!success)
        {
            Debug.Log("Za mało surowców!");
            return; // NIE CancelBuilding()
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

        bool keepPlacing = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (!keepPlacing)
        {
            CancelBuilding();
            isPlacingBuilding = false;
        }
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

    System.Collections.IEnumerator CancelNextFrame()
    {
        isPlacingBuilding = true;

        yield return null; // czekaj 1 klatkę

        CancelBuilding();
        isPlacingBuilding = false;
    }
}
