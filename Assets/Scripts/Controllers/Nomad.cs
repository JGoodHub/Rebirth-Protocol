using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Nomad : MonoBehaviour, IPointerClickHandler
{
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

    private bool _isSelected;
    private bool _wasSelectedThisFrame;
    private bool _isIdle;

    private float _healthCurrent;
    private float _hydrationCurrent;
    private float _energyCurrent;

    private bool _isEating;
    private bool _isDrinking;

    private void Awake()
    {
        _healthCurrent = _healthMax;
        _hydrationCurrent = _hydrationMax;
        _energyCurrent = _energyMax;

        SetIdle();
        Deselect();
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
    }

    public void SetIdle()
    {
        _isIdle = true;
        InvokeRepeating(nameof(FlipLookDirection), 0, Random.Range(5f, 7f));
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

    public void SetMoving()
    {
        _isIdle = false;
    }

    private void FlipLookDirection()
    {
        _spriteTransform.localScale = new Vector3(-_spriteTransform.localScale.x, _spriteTransform.localScale.y, 1);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _isSelected = true;
        _statsPanel.gameObject.SetActive(true);

        _wasSelectedThisFrame = true;
    }

    public void Deselect()
    {
        if (_wasSelectedThisFrame)
            return;

        _isSelected = false;
        _statsPanel.gameObject.SetActive(false);
    }
}