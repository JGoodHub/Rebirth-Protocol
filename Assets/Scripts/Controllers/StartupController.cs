using System;
using System.Collections.Generic;
using UnityEngine;

public class StartupController : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 60;
        
        WorldGenerator.Singleton.Initialise();
        PlayerController.Singleton.Initialise();
        RiverController.Singleton.Initialise();
        NomadController.Singleton.Initialise();
        SceneryController.Singleton.Initialise();

        RiverController.Singleton.MakeRiversWalkable();
    }
}