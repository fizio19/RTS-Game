using UnityEngine;

public class UnitCommandHandler : MonoBehaviour
{
    void Update()
    {
        // jeśli BuildingPlacer obsłużył klik w tej klatce -> NIC NIE RÓB
        if (BuildingPlacer.inputLockFrame == Time.frameCount)
            return;

        // jeśli budujemy -> brak komend
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

        foreach (UnitMovement unit in SelectionManager.Instance.selectedUnits)
        {
            Worker worker = unit.GetComponent<Worker>();

            if (hit.collider != null)
            {
                ResourceNode resource = hit.collider.GetComponent<ResourceNode>();

                if (resource != null && worker != null)
                {
                    worker.SetTarget(resource);
                    continue;
                }
            }

            // ruch
            unit.MoveTo(mousePos);

            if (worker != null)
                worker.StopWorkExternal();
        }
    }
}