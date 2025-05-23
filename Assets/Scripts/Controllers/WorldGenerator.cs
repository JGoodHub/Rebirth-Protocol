using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : SceneSingleton<WorldGenerator>
{
    [SerializeField]
    private Vector2Int _mapSize = new Vector2Int(100, 60);

    [SerializeField]
    private Transform _terrainParent;

    [SerializeField]
    private GameObject _terrainPrefab;

    private TerrainCell[,] _terrainCells;

    public Vector2Int MapSize => _mapSize;

    public void Initialise()
    {
        _terrainCells = GenerateWorldData(_mapSize);
        InstantiateWorldData(_terrainCells);
    }

    private TerrainCell[,] GenerateWorldData(Vector2Int mapSize)
    {
        TerrainCell[,] tileCells = new TerrainCell[mapSize.x, mapSize.y];

        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                tileCells[x, y] = new TerrainCell
                {
                    TerrainType = TerrainType.DESERT,
                    Coords = new Vector2Int(x, y)
                };
            }
        }

        return tileCells;
    }

    private void InstantiateWorldData(TerrainCell[,] terrainCells)
    {
        foreach (TerrainCell terrainCell in terrainCells)
        {
            Instantiate(_terrainPrefab, new Vector3(terrainCell.Coords.x, terrainCell.Coords.y), Quaternion.identity,
                _terrainParent);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(_mapSize.x / 2f, _mapSize.y / 2f) - (Vector3.one / 2f),
            new Vector3(_mapSize.x, _mapSize.y));
    }
}

public class TerrainCell
{
    public TerrainType TerrainType;
    public Vector2Int Coords;
}

public enum TerrainType
{
    DESERT
}