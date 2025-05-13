using System.Collections.Generic;
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
    private List<Vector2Int> _cactusPositions = new List<Vector2Int>();
    private List<Vector2Int> _rockPositions = new List<Vector2Int>();

    public void Initialise()
    {
        List<Vector2Int> walkableSpaces = ObstaclesController.Singleton.GetWalkableSpaces();

        for (int i = 0; i < _startingDeadPlantsCount; i++)
        {
            Vector2Int walkableSpace = walkableSpaces[Random.Range(0, walkableSpaces.Count)];

            if (ObstaclesController.Singleton.IsAreaFree(walkableSpace) == false)
                continue;

            Instantiate(_deadPlantPrefab, new Vector3(walkableSpace.x, walkableSpace.y), Quaternion.identity,
                transform);

            _deadPlantPositions.Add(walkableSpace);

            ObstaclesController.Singleton.RegisterObstacle(walkableSpace, Vector2Int.one * 2);
        }

        for (int i = 0; i < _startingCactusCount; i++)
        {
            Vector2Int walkableSpace = walkableSpaces[Random.Range(0, walkableSpaces.Count)];

            if (ObstaclesController.Singleton.IsAreaFree(walkableSpace) == false)
                continue;

            Instantiate(_cactusPrefab, new Vector3(walkableSpace.x, walkableSpace.y), Quaternion.identity,
                transform);

            _cactusPositions.Add(walkableSpace);

            ObstaclesController.Singleton.RegisterObstacle(walkableSpace, Vector2Int.one * 2);
        }

        for (int i = 0; i < _startingRocksCount; i++)
        {
            Vector2Int walkableSpace = walkableSpaces[Random.Range(0, walkableSpaces.Count)];

            if (ObstaclesController.Singleton.IsAreaFree(walkableSpace, Vector2Int.one * 2) == false)
                continue;

            Instantiate(_rockPrefabs[Random.Range(0, _rockPrefabs.Length)],
                new Vector3(walkableSpace.x, walkableSpace.y), Quaternion.identity,
                transform);

            _rockPositions.Add(walkableSpace);

            ObstaclesController.Singleton.RegisterObstacle(walkableSpace, Vector2Int.one * 2);
        }
    }
}