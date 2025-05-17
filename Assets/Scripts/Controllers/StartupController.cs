using System;
using System.Collections.Generic;
using UnityEngine;

public class StartupController : MonoBehaviour
{
    private void Start()
    {
        WorldGenerator.Singleton.Initialise();
        PlayerController.Singleton.Initialise();
        RiverController.Singleton.Initialise();
        NomadController.Singleton.Initialise();
        SceneryController.Singleton.Initialise();

        RiverController.Singleton.MakeRiversWalkable();

        PathfindingController.Singleton.UnregisterObstacle(new Vector2Int(5, 5));
        PathfindingController.Singleton.UnregisterObstacle(new Vector2Int(95, 55));
        
        List<Vector2Int> shortestPath = PathfindingController.Singleton.GetShortestPath(new Vector2Int(5, 5), new Vector2Int(95, 55));

        for (int i = 0; i < shortestPath.Count - 1; i++)
        {
            Debug.DrawLine(new Vector3(shortestPath[i].x, shortestPath[i].y), new Vector3(shortestPath[i + 1].x, shortestPath[i + 1].y), Color.red, 20f);
        }
    }
}