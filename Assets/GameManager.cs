using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SelectionManager selectionManager;

    void Update()
    {
        // PPM = ruch
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            foreach (var unit in selectionManager.selectedUnits)
            {
                unit.MoveTo(mousePos);
            }
        }

        // klik pojedynczej jednostki (tylko jeśli nie było drag)
        if (Input.GetMouseButtonUp(0))
        {
            if (!selectionManager.isDragging)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider != null)
                {
                    UnitMovement unit = hit.collider.GetComponent<UnitMovement>();

                    if (unit != null)
                    {
                        ClearSelection();
                        unit.Select();
                        selectionManager.selectedUnits.Add(unit);
                    }
                }
            }
        }
    }

    void ClearSelection()
    {
        foreach (var unit in selectionManager.selectedUnits)
        {
            unit.Deselect();
        }

        selectionManager.selectedUnits.Clear();
    }
}