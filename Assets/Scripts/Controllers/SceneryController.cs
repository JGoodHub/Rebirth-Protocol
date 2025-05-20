using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SceneryController : SceneSingleton<SceneryController>
{
    [SerializeField]
    private GameObject _plantPrefab;

    [SerializeField]
    private int _startingPlantsCount = 30;

    [SerializeField]
    private int _spreadRadius = 4;

    [SerializeField]
    private GameObject _grassTilePrefab;

    [SerializeField]
    private GameObject _cactusPrefab;

    [SerializeField]
    private int _startingCactiCount = 10;

    [SerializeField]
    private GameObject[] _rockPrefabs;

    [SerializeField]
    private int _startingRocksCount = 20;

    private List<Harvestable> _plants = new List<Harvestable>();
    private List<Harvestable> _cacti = new List<Harvestable>();
    private List<Vector2Int> _rockPositions = new List<Vector2Int>();

    private HashSet<Vector2Int> _barrenTiles;
    private HashSet<Vector2Int> _grassTiles;

    private void Awake()
    {
        InvokeRepeating(nameof(SpreadVegetation), 5f, 5f);
    }

    public void Initialise()
    {
        SpawnHarvestables(_plantPrefab, _startingPlantsCount, Vector2Int.one, false, ref _plants);

        SpawnHarvestables(_cactusPrefab, _startingCactiCount, new Vector2Int(1, 2), true, ref _cacti);

        List<Vector2Int> walkableSpaces = PathfindingController.Singleton.GetWalkableSpaces();

        for (int i = 0; i < _startingRocksCount; i++)
        {
            Vector2Int walkableSpace = walkableSpaces[Random.Range(0, walkableSpaces.Count)];

            if (PathfindingController.Singleton.IsAreaWalkable(walkableSpace, Vector2Int.one * 2) == false)
                continue;

            Instantiate(_rockPrefabs[Random.Range(0, _rockPrefabs.Length)],
                new Vector3(walkableSpace.x, walkableSpace.y), Quaternion.identity,
                transform);

            _rockPositions.Add(walkableSpace);

            PathfindingController.Singleton.RegisterObstacle(walkableSpace, Vector2Int.one * 2);
        }

        _barrenTiles = new HashSet<Vector2Int>(PathfindingController.Singleton.GetWalkableSpaces());
        _grassTiles = new HashSet<Vector2Int>();
    }

    public void SpawnHarvestables(GameObject prefab, int count, Vector2Int size, bool isObstacle,
        ref List<Harvestable> container)
    {
        List<Vector2Int> walkableSpaces = PathfindingController.Singleton.GetWalkableSpaces();

        for (int i = 0; i < count; i++)
        {
            Vector2Int walkableSpace = walkableSpaces[Random.Range(0, walkableSpaces.Count)];

            if (PathfindingController.Singleton.IsAreaWalkable(walkableSpace, size) == false)
                continue;

            Harvestable spawnedObject = Instantiate(prefab, new Vector3(walkableSpace.x, walkableSpace.y),
                Quaternion.identity,
                transform).GetComponent<Harvestable>();

            container.Add(spawnedObject);

            if (isObstacle)
            {
                PathfindingController.Singleton.RegisterObstacle(walkableSpace, size);
            }
        }
    }

    public Harvestable GetNearestUnreservedWaterSource(Vector2Int position)
    {
        return _cacti
            .Where(cactus => cactus.HealthPercentage > 0.8f && cactus.IsReserved == false)
            .MinItem(cactus => Vector2.Distance(cactus.transform.position, position));
    }

    public Harvestable GetNearestUnreservedHarvestablePlant(Vector2Int position)
    {
        return _plants
            .Where(plant => plant.HealthPercentage > 0.99f && plant.IsReserved == false)
            .MinItem(plant => Vector2.Distance(plant.transform.position, position));
    }

    public void RemoveReservationOnDeath(Nomad deadNomad)
    {
        _cacti.Find(item => item.IsReservedBy(deadNomad))?.ClearReservation();
    }

    public List<Harvestable> GetHarvestablesInArea(Vector2Int min, Vector2Int size)
    {
        HashSet<Vector2Int> positions = new HashSet<Vector2Int>();

        for (int x = min.x; x < min.x + size.x; x++)
        {
            for (int y = min.y; y < min.y + size.y; y++)
            {
                positions.Add(new Vector2Int(x, y));
            }
        }

        List<Harvestable> harvestables = new List<Harvestable>();

        foreach (Harvestable plant in _plants)
        {
            if (positions.Contains(plant.Coords))
            {
                harvestables.Add(plant);
            }
        }

        foreach (Harvestable cactus in _cacti)
        {
            if (positions.Contains(cactus.Coords))
            {
                harvestables.Add(cactus);
            }
        }

        return harvestables;
    }

    public Vector2Int PickRandomBarrenTile()
    {
        return _barrenTiles.ToList()[Random.Range(0, _barrenTiles.Count)];
    }

    public void PlantSeed(Vector2Int coord)
    {
        Harvestable newPlant = Instantiate(_plantPrefab, (Vector2)coord, Quaternion.identity, transform)
            .GetComponent<Harvestable>();

        newPlant.ChangeHealth(-100);

        _plants.Add(newPlant);
    }

    private void SpreadVegetation()
    {
        foreach (Harvestable plant in _plants)
        {
            if (plant.HealthPercentage < 0.82f)
                continue;

            Vector2Int nearestBarrenTile = _barrenTiles
                .Where(tile => Vector2Int.Distance(plant.Coords, tile) <= _spreadRadius)
                .OrderBy(tile => Vector2Int.Distance(plant.Coords, tile))
                .FirstOrDefault();

            if (nearestBarrenTile == default)
                continue;

            Instantiate(_grassTilePrefab, (Vector2)nearestBarrenTile, Quaternion.identity, transform);

            _barrenTiles.Remove(nearestBarrenTile);
            _grassTiles.Add(nearestBarrenTile);
        }
    }
}