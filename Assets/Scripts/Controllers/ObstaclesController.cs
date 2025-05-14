using System.Collections.Generic;
using System.Text;
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
            if (IsWithinMapBounds(coords, size))
            {
                _walkableMap[coords.x, coords.y] = false;
            }
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

    public bool IsAreaWalkable(Vector2Int coords, Vector2Int size = default)
    {
        if (size == default)
        {
            return IsWithinMapBounds(coords, size) && _walkableMap[coords.x, coords.y];
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

    public bool IsWithinMapBounds(Vector2Int coords, Vector2Int size = default)
    {
        if (size == default)
        {
            return coords.x >= 0 && coords.x < _walkableMap.GetLength(0) &&
                   coords.y >= 0 && coords.y < _walkableMap.GetLength(1);
        }

        return false;
    }

    public byte GetFourPointIsWalkableSample(Vector2Int sampleCenter)
    {
        byte walkableSampleOutput = 0;

        Vector2Int[] sampleCoords =
        {
            new Vector2Int(sampleCenter.x, sampleCenter.y + 1),
            new Vector2Int(sampleCenter.x + 1, sampleCenter.y),
            new Vector2Int(sampleCenter.x, sampleCenter.y - 1),
            new Vector2Int(sampleCenter.x - 1, sampleCenter.y),
        };

        for (int i = 0; i < sampleCoords.Length; i++)
        {
            Vector2Int offsetSamplePoint = sampleCoords[i];
            if (IsAreaWalkable(offsetSamplePoint))
            {
                walkableSampleOutput |= (byte)(1 << 3 - i);
            }
        }

        return walkableSampleOutput;
    }

    public byte GetEightPointIsWalkableSample(Vector2Int sampleCenter)
    {
        byte walkableSampleOutput = 0;

        Vector2Int[] sampleCoords =
        {
            new Vector2Int(sampleCenter.x, sampleCenter.y + 1),
            new Vector2Int(sampleCenter.x + 1, sampleCenter.y + 1),
            new Vector2Int(sampleCenter.x + 1, sampleCenter.y),
            new Vector2Int(sampleCenter.x + 1, sampleCenter.y - 1),
            new Vector2Int(sampleCenter.x, sampleCenter.y - 1),
            new Vector2Int(sampleCenter.x - 1, sampleCenter.y - 1),
            new Vector2Int(sampleCenter.x - 1, sampleCenter.y),
            new Vector2Int(sampleCenter.x - 1, sampleCenter.y + 1),
        };

        for (int i = 0; i < sampleCoords.Length; i++)
        {
            Vector2Int offsetSamplePoint = sampleCoords[i];
            if (IsAreaWalkable(offsetSamplePoint))
            {
                walkableSampleOutput |= (byte)(1 << 7 - i);
            }
        }

        return walkableSampleOutput;
    }

    public List<Direction> GetEightPointWalkableDirections(Vector2Int sampleCenter)
    {
        List<Direction> walkableDirections = new List<Direction>();

        Vector2Int[] sampleCoords =
        {
            new Vector2Int(sampleCenter.x, sampleCenter.y + 1),
            new Vector2Int(sampleCenter.x + 1, sampleCenter.y + 1),
            new Vector2Int(sampleCenter.x + 1, sampleCenter.y),
            new Vector2Int(sampleCenter.x + 1, sampleCenter.y - 1),
            new Vector2Int(sampleCenter.x, sampleCenter.y - 1),
            new Vector2Int(sampleCenter.x - 1, sampleCenter.y - 1),
            new Vector2Int(sampleCenter.x - 1, sampleCenter.y),
            new Vector2Int(sampleCenter.x - 1, sampleCenter.y + 1),
        };

        for (int i = 0; i < sampleCoords.Length; i++)
        {
            Vector2Int offsetSamplePoint = sampleCoords[i];
            if (IsAreaWalkable(offsetSamplePoint))
            {
                walkableDirections.Add((Direction)i);
            }
        }

        return walkableDirections;
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