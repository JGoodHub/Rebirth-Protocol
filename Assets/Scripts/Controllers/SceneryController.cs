using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneryController : SceneSingleton<SceneryController>
{
    [SerializeField]
    private GameObject _plantPrefab;

    [SerializeField]
    private int _startingPlantsCount = 30;

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

    public void Initialise()
    {
        SpawnHarvestable(_plantPrefab, _startingPlantsCount, Vector2Int.one, ref _plants);

        SpawnHarvestable(_cactusPrefab, _startingCactiCount, new Vector2Int(1, 2), ref _cacti);

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
    }

    public void SpawnHarvestable(GameObject prefab, int count, Vector2Int obstacleSize, ref List<Harvestable> container)
    {
        List<Vector2Int> walkableSpaces = PathfindingController.Singleton.GetWalkableSpaces();

        for (int i = 0; i < count; i++)
        {
            Vector2Int walkableSpace = walkableSpaces[Random.Range(0, walkableSpaces.Count)];

            if (PathfindingController.Singleton.IsAreaWalkable(walkableSpace, obstacleSize) == false)
                continue;

            Harvestable spawnedObject = Instantiate(prefab, new Vector3(walkableSpace.x, walkableSpace.y),
                Quaternion.identity,
                transform).GetComponent<Harvestable>();

            container.Add(spawnedObject);

            PathfindingController.Singleton.RegisterObstacle(walkableSpace, obstacleSize);
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
}