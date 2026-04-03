using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    public RectTransform selectionBox;

    private Vector2 startPos;
    private bool isDragging = false;

    public List<UnitMovement> selectedUnits = new List<UnitMovement>();

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            isDragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            if (Vector2.Distance(startPos, Input.mousePosition) > 5f)
            {
                isDragging = true;
                selectionBox.gameObject.SetActive(true);
                UpdateBox(Input.mousePosition);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            bool add = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            if (isDragging)
                SelectUnits(add);
            else
                SelectSingle(add);

            selectionBox.gameObject.SetActive(false);
        }
    }

    void UpdateBox(Vector2 currentMousePos)
    {
        Vector2 center = (startPos + currentMousePos) / 2f;
        Vector2 size = new Vector2(
            Mathf.Abs(startPos.x - currentMousePos.x),
            Mathf.Abs(startPos.y - currentMousePos.y)
        );

        selectionBox.position = center;
        selectionBox.sizeDelta = size;
    }

    void SelectUnits(bool add)
    {
        if (!add)
            ClearSelection();

        Vector2 start = startPos;
        Vector2 end = Input.mousePosition;

        float minX = Mathf.Min(start.x, end.x);
        float maxX = Mathf.Max(start.x, end.x);
        float minY = Mathf.Min(start.y, end.y);
        float maxY = Mathf.Max(start.y, end.y);

        foreach (var unit in FindObjectsOfType<UnitMovement>())
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(unit.transform.position);

            if (pos.x >= minX && pos.x <= maxX &&
                pos.y >= minY && pos.y <= maxY)
            {
                if (!selectedUnits.Contains(unit))
                {
                    unit.Select();
                    selectedUnits.Add(unit);
                }
            }
        }
    }

    void SelectSingle(bool add)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (!add)
            ClearSelection();

        if (hit.collider != null)
        {
            UnitMovement unit = hit.collider.GetComponent<UnitMovement>();

            if (unit != null)
            {
                unit.Select();
                selectedUnits.Add(unit);
            }
        }
    }

    void ClearSelection()
    {
        foreach (var unit in selectedUnits)
            unit.Deselect();

        selectedUnits.Clear();
    }
}