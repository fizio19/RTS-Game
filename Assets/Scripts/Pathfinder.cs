using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public static Pathfinder Instance;

    void Awake()
    {
        Instance = this;
    }

    public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Vector2Int start = GridManager.Instance.WorldToGrid(startPos);
        Vector2Int target = GridManager.Instance.WorldToGrid(targetPos);

        target = GetClosestWalkable(target);

        List<Vector2Int> openList = new List<Vector2Int>();
        HashSet<Vector2Int> closedList = new HashSet<Vector2Int>();

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> gCost = new Dictionary<Vector2Int, int>();

        openList.Add(start);
        gCost[start] = 0;

        int safety = 0;

        while (openList.Count > 0)
        {
            safety++;

            if (safety > 5000)
            {
                Debug.LogError("Pathfinding stopped - too many iterations");
                return null;
            }

            Vector2Int current = openList[0];

            foreach (var node in openList)
            {
                if (GetFCost(node, target, gCost) < GetFCost(current, target, gCost))
                    current = node;
            }

            if (current == target)
                return ReconstructPath(cameFrom, current);

            openList.Remove(current);
            closedList.Add(current);

            foreach (var neighbor in GetNeighbors(current))
            {
                if (closedList.Contains(neighbor)) continue;
                if (GridManager.Instance.IsBlocked(neighbor)) continue;

                int tentativeG = gCost[current] + 1;

                if (!openList.Contains(neighbor))
                    openList.Add(neighbor);
                else if (tentativeG >= gCost.GetValueOrDefault(neighbor, int.MaxValue))
                    continue;

                cameFrom[neighbor] = current;
                gCost[neighbor] = tentativeG;
            }
        }

        return null;
    }

    Vector2Int GetClosestWalkable(Vector2Int target)
    {
        if (!GridManager.Instance.IsBlocked(target))
            return target;

        int radius = 1;

        while (radius < 10)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    Vector2Int check = new Vector2Int(target.x + x, target.y + y);

                    if (!GridManager.Instance.IsBlocked(check))
                        return check;
                }
            }

            radius++;
        }

        return target;
    }

    List<Vector2Int> GetNeighbors(Vector2Int node)
    {
        return new List<Vector2Int>
        {
            node + Vector2Int.up,
            node + Vector2Int.down,
            node + Vector2Int.left,
            node + Vector2Int.right
        };
    }

    int GetFCost(Vector2Int node, Vector2Int target, Dictionary<Vector2Int, int> gCost)
    {
        int h = Mathf.Abs(node.x - target.x) + Mathf.Abs(node.y - target.y);
        int g = gCost.GetValueOrDefault(node, int.MaxValue);
        return g + h;
    }

    List<Vector3> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector3> path = new List<Vector3>();

        while (cameFrom.ContainsKey(current))
        {
            path.Add(GridManager.Instance.GridToWorld(current));
            current = cameFrom[current];
        }

        path.Reverse();

        return SmoothPath(path);
    }

    List<Vector3> SmoothPath(List<Vector3> path)
    {
        if (path == null || path.Count < 2)
            return path;

        List<Vector3> smooth = new List<Vector3>();
        int current = 0;

        smooth.Add(path[0]);

        while (current < path.Count - 1)
        {
            int next = current + 1;

            for (int i = path.Count - 1; i > current; i--)
            {
                if (!Physics2D.Linecast(path[current], path[i], LayerMask.GetMask("Building")))
                {
                    next = i;
                    break;
                }
            }

            smooth.Add(path[next]);
            current = next;
        }

        return smooth;
    }
}