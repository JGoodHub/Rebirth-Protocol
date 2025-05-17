using UnityEngine;

public class PlayerController : SceneSingleton<PlayerController>
{
    [SerializeField]
    private GameObject _playerBasePrefab;

    [SerializeField]
    private Vector2Int _baseCoords;

    [SerializeField]
    private Vector2Int _baseSize;
    
    public void Initialise()
    {
        Instantiate(_playerBasePrefab, new Vector3(_baseCoords.x, _baseCoords.y), Quaternion.identity, transform);
        PathfindingController.Singleton.RegisterObstacle(_baseCoords, _baseSize);
    }
}