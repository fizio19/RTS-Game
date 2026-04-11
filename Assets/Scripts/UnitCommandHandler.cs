using UnityEngine;

public class UnitCommandHandler : MonoBehaviour
{
    void Update()
    {
        if (BuildingPlacer.inputLockFrame == Time.frameCount)
            return;

        if (BuildingPlacer.Instance != null && BuildingPlacer.Instance.selectedBuilding != null)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            HandleRightClick();
        }
    }

    void HandleRightClick()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D hit = Physics2D.OverlapPoint(mousePos);

        if (hit != null)
            Debug.Log("Kliknięto: " + hit.name);

        foreach (var unit in SelectionManager.Instance.selectedUnits)
        {
            Worker worker = unit.GetComponent<Worker>();

            if (hit != null)
            {
                // ===== RESOURCE =====
                ResourceNode resource = hit.GetComponent<ResourceNode>();
                if (resource == null)
                    resource = hit.GetComponentInParent<ResourceNode>();

                if (resource != null && worker != null)
                {
                    Debug.Log("Zbieranie START");

                    worker.SetTarget(resource);

                    Vector3 pos = hit.ClosestPoint(unit.transform.position);
                    unit.MoveTo(pos);

                    continue;
                }

                // ===== BUILDING =====
                Building building = hit.GetComponent<Building>();
                if (building == null)
                    building = hit.GetComponentInParent<Building>();

                if (building != null)
                {
                    if (worker != null && !building.IsConstructed)
                    {
                        worker.StartBuilding(building);
                        continue;
                    }

                    Vector3 pos = hit.ClosestPoint(unit.transform.position);
                    unit.MoveTo(pos);

                    if (worker != null)
                        worker.StopWorkExternal();

                    continue;
                }
            }

            unit.MoveTo(mousePos);

            if (worker != null)
                worker.StopWorkExternal();
        }
    }
}
