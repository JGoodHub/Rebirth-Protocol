using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Harvestable : MonoBehaviour
{
    [Serializable]
    public class DisplayStage
    {
        public Vector2 ValueRange;
        public GameObject VisualGameObject;
    }

    [SerializeField]
    private float _hydrationAmount;

    [SerializeField]
    private float _energyAmount;

    [SerializeField]
    private float _seedsAmount;

    [SerializeField]
    private float _maxHealth;

    [SerializeField]
    private float _harvestRate;

    [SerializeField]
    private float _replenishRate;

    [SerializeField]
    private List<DisplayStage> _displayStages;

    [SerializeField]
    private List<Vector2Int> _harvestingSlots;

    [SerializeField]
    private bool _startAtMaxHealth;

    [SerializeField]
    private bool _startAtRandomHealth;

    private bool _beingHarvested;

    private float _health;

    private Nomad _reservee;

    public float HealthPercentage => _health / _maxHealth;

    public Vector2Int Coords =>
        new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

    public List<Vector2Int> HarvestingSlots => _harvestingSlots.Select(slot => Coords + slot).ToList();

    public bool IsReserved => _reservee != null;

    private void Awake()
    {
        if (_startAtMaxHealth)
        {
            _health = _maxHealth;
        }
        else if (_startAtRandomHealth)
        {
            _health = UnityEngine.Random.Range(0, _maxHealth);
        }

        UpdateVisualGameObject();
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(TickStats), 0f, 1f);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(TickStats));
    }

    private void TickStats()
    {
        if (_beingHarvested == false)
        {
            _health = Mathf.Clamp(_health + _replenishRate, 0f, _maxHealth);
        }

        UpdateVisualGameObject();
    }

    private void UpdateVisualGameObject()
    {
        foreach (DisplayStage displayStage in _displayStages)
        {
            displayStage.VisualGameObject.SetActive(displayStage.ValueRange.x <= _health &&
                                                    _health < displayStage.ValueRange.y);
        }
    }

    public void Reserve(Nomad nomad)
    {
        _reservee = nomad;
    }

    public void ClearReservation()
    {
        _beingHarvested = false;
        _reservee = null;
    }

    public void Harvest(Nomad nomad, Action completedCallback)
    {
        StartCoroutine(HarvestRoutine(nomad, completedCallback));
    }

    private IEnumerator HarvestRoutine(Nomad nomad, Action completedCallback)
    {
        _beingHarvested = true;

        float startingHydration = _hydrationAmount * HealthPercentage;
        float startingEnergy = _hydrationAmount * HealthPercentage;

        float harvestTicks = _health / _harvestRate;
        float hydrationPerTick = startingHydration / harvestTicks;
        float energyPerTick = startingEnergy / harvestTicks;

        while (_health > 0f)
        {
            nomad.ApplyStatChanges(0f, hydrationPerTick, energyPerTick, 0f);

            _health = Mathf.Clamp(_health - _harvestRate, 0f, _maxHealth);

            yield return new WaitForSeconds(1f);
        }

        nomad.ApplyStatChanges(0f, 0f, 0f, _seedsAmount);
        ClearReservation();

        completedCallback?.Invoke();
    }

    public bool IsReservedBy(Nomad nomad)
    {
        if (_reservee == null)
            return false;

        return _reservee == nomad;
    }

    public void BoostHealth(float bonus)
    {
        _health = Mathf.Clamp(_health + bonus, 0f, _maxHealth);
    }
}