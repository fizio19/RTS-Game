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
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        foreach (var unit in SelectionManager.Instance.selectedUnits)
        {
            Worker worker = unit.GetComponent<Worker>();

            // ======================
            // RESOURCE
            // ======================
            if (hit.collider != null)
            {
                ResourceNode resource = hit.collider.GetComponentInParent<ResourceNode>();

                if (resource != null && worker != null)
                {
                    worker.SetTarget(resource);

                    Vector3 pos = hit.collider.ClosestPoint(unit.transform.position);
                    unit.MoveTo(pos);

                    continue;
                }

                // ======================
                // BUDYNEK (tylko Building!)
                // ======================
                Building building = hit.collider.GetComponentInParent<Building>();

                if (building != null)
                {
                    if (worker != null && !building.IsConstructed)
                    {
                        // Wspólne budowanie: każdy zaznaczony worker dodaje własną prędkość budowy.
                        worker.StartBuilding(building);
                        continue;
                    }

                    Vector3 pos = hit.collider.ClosestPoint(unit.transform.position);
                    unit.MoveTo(pos);

                    if (worker != null)
                        worker.StopWorkExternal();

                    continue;
                }
            }

            // ======================
            // PUSTA MAPA (tilemap też tu trafia)
            // ======================
            unit.MoveTo(mousePos);

            if (worker != null)
                worker.StopWorkExternal();
        }
    }
}
