using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PathfindingController : SceneSingleton<PathfindingController>
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

    public void UnregisterObstacle(Vector2Int coords, Vector2Int size = default)
    {
        if (size == default)
        {
            if (IsWithinMapBounds(coords, size))
            {
                _walkableMap[coords.x, coords.y] = true;
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
                    _walkableMap[mapX, mapY] = true;
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

    public List<Vector2Int> GetShortestPath(Vector2Int start, Vector2Int end)
    {
        if (_walkableMap[start.x, start.y] == false || _walkableMap[end.x, end.y] == false)
        {
            Debug.LogError("Attempted to navigate to/from an unwalkable space.");
            return new List<Vector2Int>();
        }

        if (start == end)
        {
            Debug.LogWarning("Start and End coordinate are the same.");
            return new List<Vector2Int>();
        }

        List<Vector2Int> path = new List<Vector2Int>();
        Dictionary<Vector2Int, float> distanceMap = new Dictionary<Vector2Int, float>();

        Queue<Vector2Int> searchQueue = new Queue<Vector2Int>();
        HashSet<Vector2Int> searchedTilesSet = new HashSet<Vector2Int>();

        searchQueue.Enqueue(start);
        distanceMap.Add(start, 0);

        // Calculate the max distance from the start tile to all other tiles in the dungeon
        while (searchQueue.Count > 0)
        {
            Vector2Int searchTile = searchQueue.Dequeue();
            searchedTilesSet.Add(searchTile);

            float searchTileDistance = distanceMap[searchTile];

            List<Vector2Int> walkableAdjacentCoords = GetWalkableAdjacentCoords(searchTile);

            foreach (Vector2Int walkableAdjacentCoord in walkableAdjacentCoords)
            {
                float distanceToCoord = searchTileDistance + Vector2Int.Distance(searchTile, walkableAdjacentCoord);

                // If we've already found a distance for this coord only replace it if it's shorter from the current tile
                if (distanceMap.ContainsKey(walkableAdjacentCoord) &&
                    distanceToCoord < distanceMap[walkableAdjacentCoord])
                {
                    distanceMap[walkableAdjacentCoord] = distanceToCoord;
                }
                else
                {
                    distanceMap.TryAdd(walkableAdjacentCoord, distanceToCoord);
                }

                // Check we've not already searched it, and it's not already in the qeue to be searched
                if (searchedTilesSet.Contains(walkableAdjacentCoord) == false &&
                    searchQueue.Contains(walkableAdjacentCoord) == false)
                {
                    searchQueue.Enqueue(walkableAdjacentCoord);
                }
            }
        }

        // From the end tile find the shortest path back to the start using the distance map
        Vector2Int nextTile = end;
        path.Add(end);

        while (nextTile != start)
        {
            List<Vector2Int> adjacentTiles = GetWalkableAdjacentCoords(nextTile);

            float bestNextTileDistance = float.MaxValue;

            foreach (Vector2Int adjacentTile in adjacentTiles)
            {
                if (distanceMap[adjacentTile] >= bestNextTileDistance)
                    continue;

                nextTile = adjacentTile;
                bestNextTileDistance = distanceMap[adjacentTile];
            }

            path.Add(nextTile);
        }

        path.Reverse();

        return path;
    }

    private List<Vector2Int> GetWalkableAdjacentCoords(Vector2Int centerCoord)
    {
        List<Vector2Int> walkableAdjacentCoords = new List<Vector2Int>();

        Vector2Int[] sampleCoords =
        {
            new Vector2Int(centerCoord.x, centerCoord.y + 1),
            new Vector2Int(centerCoord.x + 1, centerCoord.y + 1),
            new Vector2Int(centerCoord.x + 1, centerCoord.y),
            new Vector2Int(centerCoord.x + 1, centerCoord.y - 1),
            new Vector2Int(centerCoord.x, centerCoord.y - 1),
            new Vector2Int(centerCoord.x - 1, centerCoord.y - 1),
            new Vector2Int(centerCoord.x - 1, centerCoord.y),
            new Vector2Int(centerCoord.x - 1, centerCoord.y + 1),
        };

        for (int i = 0; i < sampleCoords.Length; i++)
        {
            Vector2Int offsetSamplePoint = sampleCoords[i];

            if (IsAreaWalkable(offsetSamplePoint))
            {
                walkableAdjacentCoords.Add(offsetSamplePoint);
            }
        }

        return walkableAdjacentCoords;
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

        for (int y = 1; y < _walkableMap.GetLength(1) - 1; y++)
        {
            for (int x = 1; x < _walkableMap.GetLength(0) - 1; x++)
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