using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ProceduralGeneration
{
    public static HashSet<Vector2Int> BoxGenerator(Vector2Int startPos, int width, int height)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();
        Vector2Int currentPos = startPos;
        path.Add(currentPos);
        for (int x = -width; x <= width; x++)
        {
            for (int y = -height; y <= height; y++)
            {
                Vector2Int offset = new Vector2Int(x, y);
                path.Add(currentPos + offset);
            }
        }
        return path;
    }

    public static (List<Vector2Int> corridorPath, List<Vector2Int> minPath) DirectedCorridor(Vector2Int startPos, int numSteps, Vector2Int direction)
    {
        List<Vector2Int> corridorPath = new List<Vector2Int>();
        List<Vector2Int> minPath = new List<Vector2Int>();

        Vector2Int currentPos = startPos;
        for (int i = 0; i <= numSteps; i++)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2Int offset = new Vector2Int(x, y);
                    corridorPath.Add(currentPos + offset);
                }
            }
            minPath.Add(currentPos);
            currentPos += direction;
            corridorPath.Add(currentPos);
        }
        return (corridorPath, minPath);
    }

    public static HashSet<Vector2Int> BFS(Vector2Int startPost, HashSet<Vector2Int> map)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new();
        
        frontier.Enqueue(startPost);

        while(frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();
            visited.Add(current);
            foreach (var direction in Direction2D.Directions)
            {
                Vector2Int neighbour = current + direction;
                if(map.Contains(neighbour) && !visited.Contains(neighbour) && !frontier.Contains(neighbour))
                {
                    frontier.Enqueue(neighbour);
                }
            }
        }

        // Debug.Log("reached nodes order: "+String.Join(", ",reachedNodes));
        return visited;
    }
}

public static class Direction2D
{
    public static List<Vector2Int> Directions = new List<Vector2Int>
    {
        new Vector2Int(0, 1),  //up
        new Vector2Int(0, -1), //down
        new Vector2Int(1, 0),  //right
        new Vector2Int(-1, 0)  //left
    };

    public static List<Vector2Int> AllDirections = new List<Vector2Int>
    {
        new Vector2Int(0, 1),  //up
        new Vector2Int(0, -1), //down
        new Vector2Int(1, 0),  //right
        new Vector2Int(-1, 0),  //left
        new Vector2Int(1, 1),  //up right
        new Vector2Int(-1, 1),  //up left
        new Vector2Int(1, -1),  //down right
        new Vector2Int(-1, -1)  //down left
    };

    public static Vector2Int GetRandomDirection()
    {
        return Directions[Random.Range(0, Directions.Count)];
    }
}

