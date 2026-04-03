using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public SelectionManager selectionManager;

    void Update()
    {
        if (BuildingPlacer.inputLockFrame == Time.frameCount)
            return;

        if (BuildingPlacer.Instance != null && BuildingPlacer.Instance.selectedBuilding != null)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Ground"))
                return;

            Vector3 target = hit.point;
            target.z = 0;

            MoveInFormation(target);
        }
    }

    void MoveInFormation(Vector3 target)
    {
        List<UnitMovement> units = selectionManager.selectedUnits;

        int count = units.Count;
        if (count == 0) return;

        int columns = Mathf.CeilToInt(Mathf.Sqrt(count));
        float spacing = 0.5f;

        for (int i = 0; i < count; i++)
        {
            int row = i / columns;
            int col = i % columns;

            Vector3 offset = new Vector3(col * spacing, -row * spacing, 0);

            Vector3 finalPos = target + offset;

            units[i].MoveTo(finalPos);
        }
    }
}
