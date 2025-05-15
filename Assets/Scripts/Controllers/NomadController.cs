using System.Collections.Generic;
using UnityEngine;


public class NomadController : SceneSingleton<NomadController>
{
    [SerializeField]
    private GameObject _tentPrefab;

    [SerializeField]
    private int _startingTentsCount;

    [SerializeField]
    private GameObject _nomadPrefab;

    [SerializeField]
    private int _startingNomadsCount;

    private List<Vector2Int> _tentPositions = new List<Vector2Int>();

    private List<Nomad> _nomads = new List<Nomad>();

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

        for (int i = 0; i < _startingNomadsCount; i++)
        {
            Vector2Int walkableSpace = walkableSpaces[Random.Range(0, walkableSpaces.Count)];

            if (ObstaclesController.Singleton.IsAreaWalkable(walkableSpace) == false)
                continue;

            Nomad nomad = Instantiate(_nomadPrefab, new Vector3(walkableSpace.x, walkableSpace.y), Quaternion.identity,
                transform).GetComponent<Nomad>();

            _nomads.Add(nomad);
        }
    }
}