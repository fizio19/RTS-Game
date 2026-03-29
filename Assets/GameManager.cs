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
        if (Input.GetMouseButtonUp(0) && !selectionManager.isDragging)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            UnitMovement unit = null;
            if (hit.collider != null)
            {
                unit = hit.collider.GetComponent<UnitMovement>();
            }

            bool addToSelection = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            selectionManager.SelectSingleUnit(unit, addToSelection);
        }
    }
}
