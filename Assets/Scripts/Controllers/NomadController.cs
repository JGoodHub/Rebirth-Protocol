using System.Collections.Generic;
using UnityEngine;


public class NomadController : SceneSingleton<NomadController>
{
    [SerializeField]
    private GameObject _tentPrefab;

    [SerializeField]
    private int _startingTentsCount;

    private List<Vector2Int> _tentPositions = new List<Vector2Int>();

    public void Initialise()
    {
        List<Vector2Int> walkableSpaces = ObstaclesController.Singleton.GetWalkableSpaces();

        for (int i = 0; i < _startingTentsCount; i++)
        {
            Vector2Int walkableSpace = walkableSpaces[Random.Range(0, walkableSpaces.Count)];

            if (ObstaclesController.Singleton.IsAreaWalkable(walkableSpace, Vector2Int.one * 3) == false)
                continue;

            Instantiate(_tentPrefab, new Vector3(walkableSpace.x, walkableSpace.y), Quaternion.identity,
                transform);

            _tentPositions.Add(walkableSpace);

            ObstaclesController.Singleton.RegisterObstacle(walkableSpace, Vector2Int.one * 3);
        }
    }
}