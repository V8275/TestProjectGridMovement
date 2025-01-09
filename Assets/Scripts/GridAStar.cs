using System.Collections.Generic;
using UnityEngine;

public class GridAStar : MonoBehaviour
{
    [SerializeField] private Vector2Int startLocation;
    [SerializeField] private Vector2Int targetLocation;

    private GridManager gridManager;
    private int[,] obstacles;

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();

        obstacles = gridManager.GetObstacles();
    }

    private static List<Node> GetNeighbors(Node node, int[,] grid)
    {
        var neighbors = new List<Node>();
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (Mathf.Abs(x) == Mathf.Abs(y)) continue; // Skip diagonals

                int newX = node.X + x;
                int newY = node.Y + y;

                if (newX >= 0 && newX < rows && newY >= 0 && newY < cols && grid[newX, newY] == 0)
                {
                    neighbors.Add(new Node(newX, newY));
                }
            }
        }
        return neighbors;
    }

    private static float Heuristic(Node a, Node b)
    {
        return Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y); // Manhattan distance
    }

    public static List<Node> FindPath(int[,] grid, Node start, Node target)
    {
        var openList = new List<Node> { start };
        var closedList = new HashSet<Node>();

        start.G = 0;
        start.H = Heuristic(start, target);

        while (openList.Count > 0)
        {
            // Get the node with the lowest F cost
            Node currentNode = openList[0];
            foreach (var node in openList)
            {
                if (node.F < currentNode.F)
                    currentNode = node;
            }

            // If we reached the target node, reconstruct the path
            if (currentNode.X == target.X && currentNode.Y == target.Y)
            {
                var path = new List<Node>();
                while (currentNode != null)
                {
                    path.Add(currentNode);
                    currentNode = currentNode.Parent;
                }
                path.Reverse();
                return path;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neighbor in GetNeighbors(currentNode, grid))
            {
                if (closedList.Contains(neighbor)) continue;

                float tentativeG = currentNode.G + 1; // Assuming cost of 1 for each move

                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
                else if (tentativeG >= neighbor.G)
                {
                    continue; // Not a better path
                }

                neighbor.Parent = currentNode;
                neighbor.G = tentativeG;
                neighbor.H = Heuristic(neighbor, target);
            }
        }

        return null; // No path found
    }

    public void SetStartLocation(Vector2Int location)
    {
        startLocation = location;
    }

    public void SetTargetLocation(Vector2Int location)
    {
        targetLocation = location;
    }

    public void FindPath()
    {
        Node startNode = new Node(startLocation.x, startLocation.y);
        Node targetNode = new Node(targetLocation.x, targetLocation.y);

        var path = FindPath(obstacles, startNode, targetNode);

        if (path != null)
        {
            foreach (var node in path)
            {
                // Здесь можно визуализировать путь, например, изменив цвет или добавив объект
                Debug.Log($"Path Node: ({node.X}, {node.Y})");
            }

            foreach (var node in path)
            {
                // Пример изменения цвета клетки
                GameObject cell = GameObject.Find($"Cell_{node.X}_{node.Y}");
                if (cell != null)
                {
                    Renderer renderer = cell.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = Color.green; // Изменение цвета на зеленый
                    }
                }
            }
        }
        else
        {
            Debug.Log("Path not found!");
        }
    }
}

public class Node
{
    public int X { get; set; }
    public int Y { get; set; }
    public float G { get; set; } // Cost from start to this node
    public float H { get; set; } // Heuristic cost to target
    public float F => G + H; // Total cost
    public Node Parent { get; set; }

    public Node(int x, int y)
    {
        X = x;
        Y = y;
        G = float.MaxValue;
        H = float.MaxValue;
    }
}