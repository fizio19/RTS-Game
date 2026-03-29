using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public RectTransform selectionBox;

    private Vector2 startPos;
    public bool isDragging = false;

    public List<UnitMovement> selectedUnits = new List<UnitMovement>();

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            isDragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            if (Vector2.Distance(startPos, Input.mousePosition) > 10f)
            {
                isDragging = true;
                selectionBox.gameObject.SetActive(true);
                UpdateBox(Input.mousePosition);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                bool addToSelection = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                SelectUnitsInBox(addToSelection);
            }

            selectionBox.gameObject.SetActive(false);
        }
    }

    void UpdateBox(Vector2 currentMousePos)
    {
        Vector2 size = currentMousePos - startPos;

        selectionBox.position = startPos;
        selectionBox.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));

        if (size.x < 0)
        {
            selectionBox.position = new Vector2(currentMousePos.x, selectionBox.position.y);
        }

        if (size.y < 0)
        {
            selectionBox.position = new Vector2(selectionBox.position.x, currentMousePos.y);
        }
    }

    public void SelectUnitsInBox(bool addToSelection)
    {
        if (!addToSelection)
        {
            ClearSelection();
        }

        foreach (var unit in FindObjectsOfType<UnitMovement>())
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);

            if (IsInside(screenPos) && !selectedUnits.Contains(unit))
            {
                unit.Select();
                selectedUnits.Add(unit);
            }
        }
    }

    public void SelectSingleUnit(UnitMovement unit, bool addToSelection)
    {
        if (unit == null)
        {
            if (!addToSelection)
            {
                ClearSelection();
            }

            return;
        }

        if (addToSelection)
        {
            if (selectedUnits.Contains(unit))
            {
                unit.Deselect();
                selectedUnits.Remove(unit);
            }
            else
            {
                unit.Select();
                selectedUnits.Add(unit);
            }

            return;
        }

        ClearSelection();
        unit.Select();
        selectedUnits.Add(unit);
    }

    public void ClearSelection()
    {
        foreach (var unit in selectedUnits)
        {
            unit.Deselect();
        }

        selectedUnits.Clear();
    }

    bool IsInside(Vector2 screenPos)
    {
        Vector2 boxPos = selectionBox.position;

        Vector2 min = boxPos;
        Vector2 max = boxPos + selectionBox.sizeDelta;

        if (min.x > max.x) (min.x, max.x) = (max.x, min.x);
        if (min.y > max.y) (min.y, max.y) = (max.y, min.y);

        return screenPos.x >= min.x && screenPos.x <= max.x &&
               screenPos.y >= min.y && screenPos.y <= max.y;
    }
}
