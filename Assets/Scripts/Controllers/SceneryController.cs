using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SceneryController : SceneSingleton<SceneryController>
{
    [SerializeField]
    private GameObject _deadPlantPrefab;

    [SerializeField]
    private int _startingDeadPlantsCount = 25;

    [SerializeField]
    private GameObject _cactusPrefab;

    [SerializeField]
    private int _startingCactusCount = 10;

    [SerializeField]
    private GameObject[] _rockPrefabs;

    [SerializeField]
    private int _startingRocksCount = 20;

    private List<Vector2Int> _deadPlantPositions = new List<Vector2Int>();
    private List<Harvestable> _cacti = new List<Harvestable>();
    private List<Vector2Int> _rockPositions = new List<Vector2Int>();

    public void Initialise()
    {
        List<Vector2Int> walkableSpaces = PathfindingController.Singleton.GetWalkableSpaces();

        for (int i = 0; i < _startingDeadPlantsCount; i++)
        {
            Vector2Int walkableSpace = walkableSpaces[Random.Range(0, walkableSpaces.Count)];

            if (PathfindingController.Singleton.IsAreaWalkable(walkableSpace) == false)
                continue;

            Instantiate(_deadPlantPrefab, new Vector3(walkableSpace.x, walkableSpace.y), Quaternion.identity,
                transform);

            _deadPlantPositions.Add(walkableSpace);

            PathfindingController.Singleton.RegisterObstacle(walkableSpace, Vector2Int.one);
        }

        walkableSpaces = PathfindingController.Singleton.GetWalkableSpaces();

        for (int i = 0; i < _startingCactusCount; i++)
        {
            Vector2Int walkableSpace = walkableSpaces[Random.Range(0, walkableSpaces.Count)];

            if (PathfindingController.Singleton.IsAreaWalkable(walkableSpace) == false)
                continue;

            Harvestable cactus = Instantiate(_cactusPrefab, new Vector3(walkableSpace.x, walkableSpace.y), Quaternion.identity,
                transform).GetComponent<Harvestable>();

            _cacti.Add(cactus);

            PathfindingController.Singleton.RegisterObstacle(walkableSpace, new Vector2Int(1, 2));
        }

        walkableSpaces = PathfindingController.Singleton.GetWalkableSpaces();

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

    public Harvestable GetNearestUnreservedWaterSource(Vector2Int position)
    {
        return _cacti
            .Where(cactus => cactus.HealthPercentage > 0.8f && cactus.IsReserved == false)
            .MinItem(cactus => Vector2.Distance(cactus.transform.position, position));
    }
}