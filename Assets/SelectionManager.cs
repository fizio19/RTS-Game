using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public RectTransform selectionBox;

    private Vector2 startPos;
    private Rect currentSelectionRect;

    private RectTransform selectionBoxParent;
    private Canvas selectionCanvas;

    public bool isDragging = false;

    public List<UnitMovement> selectedUnits = new List<UnitMovement>();

    void Awake()
    {
        if (selectionBox != null)
        {
            selectionBoxParent = selectionBox.parent as RectTransform;
            selectionCanvas = selectionBox.GetComponentInParent<Canvas>();
            selectionBox.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            isDragging = false;
            currentSelectionRect = new Rect();
        }

        if (Input.GetMouseButton(0))
        {
            if (Vector2.Distance(startPos, Input.mousePosition) > 10f)
            {
                isDragging = true;
                currentSelectionRect = GetScreenRect(startPos, Input.mousePosition);
                selectionBox.gameObject.SetActive(true);
                UpdateBoxVisual(currentSelectionRect);
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

    void UpdateBoxVisual(Rect screenRect)
    {
        if (selectionBox == null || selectionBoxParent == null)
        {
            return;
        }

        Camera uiCamera = null;
        if (selectionCanvas != null && selectionCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = selectionCanvas.worldCamera;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(selectionBoxParent, screenRect.min, uiCamera, out var localMin);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(selectionBoxParent, screenRect.max, uiCamera, out var localMax);

        Vector2 size = localMax - localMin;
        Vector2 center = (localMin + localMax) * 0.5f;

        selectionBox.anchoredPosition = center;
        selectionBox.sizeDelta = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
    }

    Rect GetScreenRect(Vector2 pointA, Vector2 pointB)
    {
        float xMin = Mathf.Min(pointA.x, pointB.x);
        float yMin = Mathf.Min(pointA.y, pointB.y);
        float xMax = Mathf.Max(pointA.x, pointB.x);
        float yMax = Mathf.Max(pointA.y, pointB.y);

        return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
    }

    public void SelectUnitsInBox(bool addToSelection)
    {
        if (!addToSelection)
        {
            ClearSelection();
        }

        foreach (var unit in FindObjectsOfType<UnitMovement>())
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(unit.transform.position);
            if (screenPoint.z < 0f)
            {
                continue;
            }

            if (currentSelectionRect.Contains(screenPoint) && !selectedUnits.Contains(unit))
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
}
