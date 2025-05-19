using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Nomad : MonoBehaviour, IPointerClickHandler
{
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");

    [SerializeField]
    private Transform _spriteTransform;

    [SerializeField]
    private GameObject _statsPanel;

    [SerializeField]
    private float _healthMax;

    [SerializeField]
    private float _healthDropRate;

    [SerializeField]
    private float _hydrationMax;

    [SerializeField]
    private float _hydrationDropRateIdle;

    [SerializeField]
    private float _hydrationDropRateMoving;

    [SerializeField]
    private float _energyMax;

    [SerializeField]
    private float _energyDropRateIdle;

    [SerializeField]
    private float _energyDropRateMoving;

    [SerializeField]
    private Image _healthFillImage;

    [SerializeField]
    private Image _hydrationFillImage;

    [SerializeField]
    private Image _energyFillImage;

    [SerializeField]
    private float _movementSpeed;

    [SerializeField]
    private Animator _animator;

    private bool _isSelected;
    private bool _wasSelectedThisFrame;
    private bool _isIdle;

    private float _healthCurrent;
    private float _hydrationCurrent;
    private float _energyCurrent;

    private bool _isEating;
    private bool _isDrinking;

    private Vector3 _lastFramePosition;

    private List<NomadAction> _currentActions = new List<NomadAction>();

    public float HydrationPercentage => _hydrationCurrent / _hydrationMax;

    public float EnergyPercentage => _energyCurrent / _energyMax;

    public Vector2Int Coords => new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

    private void Awake()
    {
        _healthCurrent = _healthMax;
        _hydrationCurrent = _hydrationMax;
        _energyCurrent = _energyMax;

        SetIdle();
        Deselect();

        InvokeRepeating(nameof(FlipLookDirectionWhenIdle), 0, Random.Range(5f, 7f));

        AssignNewActions();
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(TickStats), 0f, 1f);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(TickStats));
    }

    private void LateUpdate()
    {
        if (_wasSelectedThisFrame)
        {
            _wasSelectedThisFrame = false;
        }

        _lastFramePosition = transform.position;
    }

    public void CompleteActions(List<NomadAction> newActions)
    {
        if (newActions.Count == 0)
            return;

        StartCoroutine(CompleteActionsRoutine(newActions));
    }

    private IEnumerator CompleteActionsRoutine(List<NomadAction> currentActions)
    {
        _currentActions = currentActions;

        foreach (NomadAction currentAction in _currentActions)
        {
            currentAction.StartAction();
            yield return new WaitUntil(() => currentAction.IsComplete());
        }

        _currentActions.Clear();
        SetIdle();

        AssignNewActions();
    }

    private void AssignNewActions()
    {
        List<NomadAction> newActions = new List<NomadAction>();

        if (HydrationPercentage < 0.6f)
        {
            Harvestable nearestWaterSource = SceneryController.Singleton.GetNearestUnreservedWaterSource(Coords);

            if (nearestWaterSource != null)
            {
                nearestWaterSource.Reserve(this);
                Vector2Int closestSlot = nearestWaterSource.HarvestingSlots.MinItem(slot => Vector2.Distance(slot, transform.position));
                WalkToCoordNomadAction walkToCoordNomadAction = new WalkToCoordNomadAction(this, closestSlot);
                newActions.Add(walkToCoordNomadAction);

                HarvestNomadAction harvestNomadAction = new HarvestNomadAction(this, nearestWaterSource);
                newActions.Add(harvestNomadAction);
            }
            else
            {
                newActions.Add(new WaitNomadAction(this, 3f));
            }
        }
        else if (EnergyPercentage < 0.4f) { }
        else
        {
            newActions.Add(new WaitNomadAction(this, 3f));
        }

        CompleteActions(newActions);
    }

    private void TickStats()
    {
        if (_isDrinking == false)
        {
            _hydrationCurrent = Mathf.Clamp(_hydrationCurrent - (_isIdle ? _hydrationDropRateIdle : _hydrationDropRateMoving), 0f, _hydrationMax);
        }

        if (_isEating == false)
        {
            _energyCurrent = Mathf.Clamp(_energyCurrent - (_isIdle ? _energyDropRateIdle : _energyDropRateMoving), 0f, _energyMax);
        }

        if (_hydrationCurrent <= 0f || _energyCurrent <= 0f)
        {
            if (_hydrationCurrent <= 0f)
            {
                _healthCurrent = Mathf.Clamp(_healthCurrent - (_healthDropRate / 2f), 0f, _healthMax);
            }

            if (_energyCurrent <= 0f)
            {
                _healthCurrent = Mathf.Clamp(_healthCurrent - (_healthDropRate / 2f), 0f, _healthMax);
            }
        }
        else
        {
            _healthCurrent = Mathf.Clamp(_healthCurrent + _healthDropRate, 0f, _healthMax);
        }

        _healthFillImage.fillAmount = _healthCurrent / _healthMax;
        _hydrationFillImage.fillAmount = _hydrationCurrent / _hydrationMax;
        _energyFillImage.fillAmount = _energyCurrent / _energyMax;
    }

    public void SetIdle()
    {
        _isIdle = true;
        _animator.SetBool(IsMoving, false);
    }

    private void FlipLookDirectionWhenIdle()
    {
        if (_isIdle == false)
            return;

        _spriteTransform.localScale = new Vector3(-_spriteTransform.localScale.x, _spriteTransform.localScale.y, 1);
    }

    private void UpdateMovementDirection()
    {
        if (transform.position.x <= _lastFramePosition.x)
        {
            _spriteTransform.localScale = new Vector3(-1f, _spriteTransform.localScale.y, 1);
        }
        else
        {
            _spriteTransform.localScale = new Vector3(1f, _spriteTransform.localScale.y, 1);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _isSelected = true;
        _statsPanel.gameObject.SetActive(true);

        _wasSelectedThisFrame = true;

        NomadController.Singleton.SetSelectedNomad(this);
    }

    public void Deselect()
    {
        if (_wasSelectedThisFrame)
            return;

        _isSelected = false;
        _statsPanel.gameObject.SetActive(false);

        NomadController.Singleton.ClearSelectedNomad();
    }

    public void FollowPath(List<Vector2Int> path, Action completedCallback = null)
    {
        StartCoroutine(FollowPathCoroutine(path, completedCallback));
    }

    private IEnumerator FollowPathCoroutine(List<Vector2Int> path, Action completedCallback = null)
    {
        if (path.Count <= 1)
        {
            completedCallback?.Invoke();
            yield break;
        }

        _isIdle = false;
        _animator.SetBool(IsMoving, true);

        float pathLength = 0f;
        List<float> segmentLengths = new List<float>();

        for (int i = 0; i < path.Count - 1; i++)
        {
            float segmentLength = Vector2Int.Distance(path[i], path[i + 1]);
            segmentLengths.Add(segmentLength);
            pathLength += segmentLength;
        }

        float time = 0f;
        float duration = pathLength / _movementSpeed;

        Vector3 start = (Vector2) path[0];
        transform.position = start;

        while (time < duration)
        {
            float distanceTravelled = time * _movementSpeed;

            int segmentIndex = 0;
            float segmentSum = 0f;
            while (segmentIndex < segmentLengths.Count && segmentSum + segmentLengths[segmentIndex] < distanceTravelled)
            {
                segmentSum += segmentLengths[segmentIndex];
                segmentIndex++;
            }

            if (segmentIndex >= segmentLengths.Count)
                break;

            Vector2 segmentStart = path[segmentIndex];
            Vector2 segmentEnd = path[segmentIndex + 1];
            float segmentDistance = segmentLengths[segmentIndex];
            float segmentProgress = (distanceTravelled - segmentSum) / segmentDistance;

            transform.position = Vector2.Lerp(segmentStart, segmentEnd, segmentProgress);
            UpdateMovementDirection();

            yield return null;
            time += Time.deltaTime;
        }

        transform.position = (Vector2) path[^1];

        SetIdle();

        completedCallback?.Invoke();
    }

    public void ApplyStatChanges(float healthChange, float hydrationChange, float energyChange)
    {
        _healthCurrent = Mathf.Clamp(_healthCurrent + healthChange, 0f, _healthMax);
        _hydrationCurrent = Mathf.Clamp(_hydrationCurrent + hydrationChange, 0f, _hydrationMax);
        _energyCurrent = Mathf.Clamp(_energyCurrent + energyChange, 0f, _energyMax);
    }
}