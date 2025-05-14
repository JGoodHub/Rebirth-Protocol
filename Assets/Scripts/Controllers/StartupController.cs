using System;
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
    }
}