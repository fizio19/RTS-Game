using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
                SelectUnits();
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
            selectionBox.position = new Vector2(currentMousePos.x, selectionBox.position.y);

        if (size.y < 0)
            selectionBox.position = new Vector2(selectionBox.position.x, currentMousePos.y);
    }

    void SelectUnits()
    {
        foreach (var unit in selectedUnits)
        {
            unit.Deselect();
        }

        selectedUnits.Clear();

        foreach (var unit in FindObjectsOfType<UnitMovement>())
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);

            if (IsInside(screenPos))
            {
                unit.Select();
                selectedUnits.Add(unit);
            }
        }
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