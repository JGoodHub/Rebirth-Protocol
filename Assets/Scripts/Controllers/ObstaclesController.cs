using System.Collections.Generic;
using UnityEngine;

public class ObstaclesController : SceneSingleton<ObstaclesController>
{
    private bool[,] _walkableMap;

    private void Awake()
    {
        _walkableMap = new bool[WorldGenerator.Singleton.MapSize.x, WorldGenerator.Singleton.MapSize.y];

        for (int y = 0; y < _walkableMap.GetLength(1); y++)
        {
            for (int x = 0; x < _walkableMap.GetLength(0); x++)
            {
                _walkableMap[x, y] = true;
            }
        }
    }

    public void RegisterObstacle(Vector2Int coords, Vector2Int size = default)
    {
        if (size == default)
        {
            size = Vector2Int.one;
        }

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                int mapX = coords.x + x;
                int mapY = coords.y + y;

                if (mapX >= 0 && mapX < _walkableMap.GetLength(0) &&
                    mapY >= 0 && mapY < _walkableMap.GetLength(1))
                {
                    _walkableMap[mapX, mapY] = false;
                }
            }
        }
    }

    public bool IsAreaFree(Vector2Int coords, Vector2Int size = default)
    {
        if (size == default)
        {
            size = Vector2Int.one;
        }

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                int mapX = coords.x + x;
                int mapY = coords.y + y;

                if (mapX < 0 || mapX >= _walkableMap.GetLength(0) ||
                    mapY < 0 || mapY >= _walkableMap.GetLength(1))
                    return false;

                if (_walkableMap[mapX, mapY] == false)
                    return false;
            }
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        if (_walkableMap == null)
            return;

        Gizmos.color = Color.red;

        for (int y = 0; y < _walkableMap.GetLength(1); y++)
        {
            for (int x = 0; x < _walkableMap.GetLength(0); x++)
            {
                if (_walkableMap[x, y])
                    continue;

                Gizmos.DrawSphere(new Vector3(x, y), 0.3f);
            }
        }
    }

    public List<Vector2Int> GetWalkableSpaces()
    {
        List<Vector2Int> walkableSpaces = new List<Vector2Int>();

        for (int y = 0; y < _walkableMap.GetLength(1); y++)
        {
            for (int x = 0; x < _walkableMap.GetLength(0); x++)
            {
                if (_walkableMap[x, y])
                {
                    walkableSpaces.Add(new Vector2Int(x, y));
                }
            }
        }

        return walkableSpaces;
    }
}