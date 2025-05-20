using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NomadAction
{
    protected Nomad _nomad;

    protected bool _isCompleted;

    protected NomadAction(Nomad nomad)
    {
        _nomad = nomad;
    }

    public void StartAction()
    {
        ProcessAction();
    }

    protected abstract void ProcessAction();

    public virtual bool IsComplete()
    {
        return _isCompleted;
    }
}

public class WalkToCoordNomadAction : NomadAction
{
    private Vector2Int _destination;

    public WalkToCoordNomadAction(Nomad nomad, Vector2Int destination) : base(nomad)
    {
        _destination = destination;
    }

    protected override void ProcessAction()
    {
        List<Vector2Int> shortestPath = PathfindingController.Singleton.GetShortestPath(_nomad.Coords, _destination);

        _nomad.FollowPath(shortestPath, () =>
        {
            _isCompleted = true;
            _nomad.SetIdle();
        });
    }
}

public class WaitNomadAction : NomadAction
{
    private float _waitTime;

    public WaitNomadAction(Nomad nomad, float waitTime) : base(nomad)
    {
        _waitTime = waitTime;
    }

    protected override void ProcessAction()
    {
        _nomad.StartCoroutine(WaitRoutine());
    }

    private IEnumerator WaitRoutine()
    {
        yield return new WaitForSeconds(_waitTime);
        _isCompleted = true;
        _nomad.SetIdle();
    }
}

public class HarvestNomadAction : NomadAction
{
    private Harvestable _harvestable;

    public HarvestNomadAction(Nomad nomad, Harvestable harvestable) : base(nomad)
    {
        _harvestable = harvestable;
    }

    protected override void ProcessAction()
    {
        _harvestable.Harvest(_nomad, () =>
        {
            _isCompleted = true;
            _nomad.SetIdle();
        });
    }
}

public class PlantSeedNomadAction : NomadAction
{
    private Vector2Int _coord;

    public PlantSeedNomadAction(Nomad nomad, Vector2Int coord) : base(nomad)
    {
        _coord = coord;
    }

    protected override void ProcessAction()
    {
        SceneryController.Singleton.PlantSeed(_coord);
        _nomad.RemoveSeed();
        _isCompleted = true;
        _nomad.SetIdle();

    }
}