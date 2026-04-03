using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public float cellSize = 1f;
    public LayerMask obstacleLayer;

    void Awake()
    {
        Instance = this;
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / cellSize);
        int y = Mathf.RoundToInt(worldPos.y / cellSize);
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * cellSize, gridPos.y * cellSize, 0f);
    }

    public bool IsBlocked(Vector2Int gridPos)
    {
        Vector3 worldPos = GridToWorld(gridPos);
        Collider2D hit = Physics2D.OverlapCircle(worldPos, 0.4f, obstacleLayer);
        return hit != null;
    }
}