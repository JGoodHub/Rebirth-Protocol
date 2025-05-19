using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CloudsController : SceneSingleton<CloudsController>
{
    [SerializeField]
    private GameObject _cloudPrefab;

    [SerializeField]
    private int _durationInTicks;

    [SerializeField]
    private float _harvestableBonusPerTick;

    [SerializeField]
    private int _effectRadius;

    [SerializeField]
    private float _cooldown;

    [SerializeField]
    private Image _cooldownCover;

    [SerializeField]
    private GameObject _cancelCover;

    [SerializeField]
    private Button _spawnCloudButton;

    private float _cooldownRemaining;

    public void ListenForCloudPlacement()
    {
        TouchInput.OnTouchClick += OnTouchClick;
        _cancelCover.SetActive(true);
    }

    public void StopListeningForCloudPlacement()
    {
        TouchInput.OnTouchClick -= OnTouchClick;
        _cancelCover.SetActive(false);
    }

    private void OnTouchClick(TouchInput.TouchData touchData)
    {
        if (touchData.DownOverUI)
            return;

        StopListeningForCloudPlacement();

        Vector3 mouseDownPoint = RaycastPlane2D.QueryPlane();
        Vector2Int mouseDownCoords =
            new Vector2Int(Mathf.RoundToInt(mouseDownPoint.x), Mathf.RoundToInt(mouseDownPoint.y));

        SpawnRainCloud(mouseDownCoords);
    }

    public void SpawnRainCloud(Vector2Int coords)
    {
        StartCoroutine(SpawnRainCloudRoutine(coords));
        StartCoroutine(CooldownTimerRoutine());
    }

    private IEnumerator SpawnRainCloudRoutine(Vector2Int coords)
    {
        GameObject cloud = Instantiate(_cloudPrefab, (Vector2)coords, Quaternion.identity, transform);

        Vector2Int effectedAreaMin = new Vector2Int(coords.x - _effectRadius - 1, coords.y);
        Vector2Int effectedAreaSize = new Vector2Int((_effectRadius * 2) + 1, 1);

        List<Harvestable> harvestables =
            SceneryController.Singleton.GetHarvestablesInArea(effectedAreaMin, effectedAreaSize);

        for (int i = 0; i < _durationInTicks; i++)
        {
            foreach (Harvestable harvestable in harvestables)
            {
                harvestable.BoostHealth(_harvestableBonusPerTick);
            }

            yield return new WaitForSeconds(1f);
        }

        Destroy(cloud, 1f);
    }

    private IEnumerator CooldownTimerRoutine()
    {
        _cooldownRemaining = _cooldown;
        UpdateCooldownVisuals();

        while (_cooldownRemaining > 1f)
        {
            _cooldownRemaining -= 1f;
            UpdateCooldownVisuals();
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(_cooldownRemaining);
        _cooldownRemaining = 0f;

        UpdateCooldownVisuals();
    }

    private void UpdateCooldownVisuals()
    {
        _cooldownCover.enabled = _cooldownRemaining > 0f;
        _cooldownCover.fillAmount = _cooldownRemaining / _cooldown;
        _spawnCloudButton.enabled = _cooldownRemaining <= 0f;
    }
}