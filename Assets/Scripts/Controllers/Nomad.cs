using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Nomad : MonoBehaviour
{
    [SerializeField]
    private Transform _spriteTransform;

    private bool _isIdle;

    [SerializeField]
    private float _energyMax;

    [SerializeField]
    private float _energyDropRateIdle;
        
    [SerializeField]
    private float _energyDropRateMoving;
    
    [SerializeField]
    private float _hydrationMax;

    [SerializeField]
    private float _hydrationDropRateIdle;
        
    [SerializeField]
    private float _hydrationDropRateMoving;
    
    private float _energyCurrent;
    private float _hydrationCurrent;

    private bool _isEating;
    private bool _isDrinking;

    private void Awake()
    {
        SetIdle();
    }

    public void SetIdle()
    {
        _isIdle = true;
        InvokeRepeating(nameof(FlipLookDirection), 0, Random.Range(5f, 7f));
    }

    private void TickStats()
    {
        if (_isEating == false)
        {
            _energyCurrent -= _isIdle ? _energyDropRateIdle : _energyDropRateMoving;
        }

        if (_isDrinking == false)
        {
            _hydrationCurrent -= _isIdle ? _hydrationDropRateIdle : _hydrationDropRateMoving;
        }
    }

    public void SetMoving()
    {
        _isIdle = false;
    }

    private void FlipLookDirection()
    {
        _spriteTransform.localScale = new Vector3(-_spriteTransform.localScale.x, _spriteTransform.localScale.y, 1);
    }
}