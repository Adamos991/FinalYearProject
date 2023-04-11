using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGeneration : MonoBehaviour
{
    public int width, height;
    public float corridorSize = 1.0f;
    public Vector2 entrance;
    public Vector2 exit;
    public GameObject wallPrefab;

    private enum CellType { Wall, Open }
    private CellType[,] grid;
    private List<Vector2> frontier;

    void Start()
    {
        entrance = new Vector2(1, 1);
        exit = new Vector2(width-2, height - 1);
        grid = new CellType[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = CellType.Wall;

        GenerateMaze();
        CreateMazeVisuals();
    }

    void GenerateMaze()
{
    grid[(int)entrance.x, (int)entrance.y] = CellType.Open;
    frontier = new List<Vector2>();

    AddNeighborsToFrontier(entrance);

    while (frontier.Count > 0)
    {
        Vector2 current = frontier[Random.Range(0, frontier.Count)];
        Vector2 next = GetNextOpenNeighbor(current);

        if (next != Vector2.zero)
        {
            Vector2 corridor = (current + next) / 2;
            grid[(int)corridor.x, (int)corridor.y] = CellType.Open;
            grid[(int)next.x, (int)next.y] = CellType.Open;
            AddNeighborsToFrontier(next);
        }

        frontier.Remove(current);
    }

    grid[(int)entrance.x, (int)entrance.y] = CellType.Open;
    grid[(int)exit.x, (int)exit.y] = CellType.Open;
}

    void AddNeighborsToFrontier(Vector2 pos)
    {
        List<Vector2> neighbors = GetValidNeighbors(pos);
        foreach (Vector2 neighbor in neighbors)
        {
            if (!frontier.Contains(neighbor))
            {
                frontier.Add(neighbor);
            }
        }
    }

    Vector2 GetNextOpenNeighbor(Vector2 pos)
    {
        List<Vector2> neighbors = GetValidNeighbors(pos);

        foreach (Vector2 neighbor in neighbors)
        {
            if (grid[(int)neighbor.x, (int)neighbor.y] == CellType.Open)
            {
                return neighbor;
            }
        }

        return Vector2.zero;
    }

    List<Vector2> GetValidNeighbors(Vector2 pos)
{
    List<Vector2> neighbors = new List<Vector2>();
    Vector2[] possibleNeighbors = {
        new Vector2(pos.x, pos.y + 2),
        new Vector2(pos.x, pos.y - 2),
        new Vector2(pos.x + 2, pos.y),
        new Vector2(pos.x - 2, pos.y)
    };

    foreach (Vector2 neighbor in possibleNeighbors)
    {
        if (neighbor.x > 0 && neighbor.x < width - 1 &&
            neighbor.y > 0 && neighbor.y < height - 1 &&
            grid[(int)neighbor.x, (int)neighbor.y] == CellType.Wall)
        {
            int wallCount = 0;

            for (int x = (int)neighbor.x - 1; x <= (int)neighbor.x + 1; x++)
            {
                for (int y = (int)neighbor.y - 1; y <= (int)neighbor.y + 1; y++)
                {
                    if (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        if (grid[x, y] == CellType.Wall)
                        {
                            wallCount++;
                        }
                    }
                }
            }

            if (wallCount == 8) // Ensures spacious corridors
            {
                neighbors.Add(neighbor);
            }
        }
    }

    return neighbors;
}

    void CreateMazeVisuals()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == CellType.Wall)
                {
                    GameObject wall = Instantiate(wallPrefab, new Vector3(x, 0, y), Quaternion.identity);
                    wall.transform.localScale = new Vector3(corridorSize, wall.transform.localScale.y, corridorSize);
                    wall.transform.SetParent(transform);
                }
            }
        }
    }
}