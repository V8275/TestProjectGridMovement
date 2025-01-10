using System.Collections.Generic;
using UnityEngine;

public static class GridAStar
{
    public static List<Node> FindPath(int[,] grid, Vector2Int startLocation, Vector2Int targetLocation)
    {
        Node start = new Node(startLocation.x, startLocation.y);
        Node target = new Node(targetLocation.x, targetLocation.y);
        var openList = new List<Node> { start };
        var closedList = new HashSet<Node>();

        start.G = 0;
        Debug.Log("StartG ");
        start.H = Heuristic(start, target);


        Debug.Log("StartH ");
        while (openList.Count > 0)
        {
            // Get node with lowest F cost
            Node currentNode = openList[0];
            foreach (var node in openList)
            {
                if (node.F < currentNode.F)
                    currentNode = node;
            }

            // Reconstruct the path
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
                else if (tentativeG >= neighbor.G) continue;

                neighbor.Parent = currentNode;
                neighbor.G = tentativeG;
                neighbor.H = Heuristic(neighbor, target);
            }
        }

        return null;
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
}

public class Node
{
    public int X { get; set; }
    public int Y { get; set; }
    public float G { get; set; } 
    public float H { get; set; } 
    public float F => G + H; 
    public Node Parent { get; set; }

    public Node(int x, int y)
    {
        X = x;
        Y = y;
        G = float.MaxValue;
        H = float.MaxValue;
    }
}