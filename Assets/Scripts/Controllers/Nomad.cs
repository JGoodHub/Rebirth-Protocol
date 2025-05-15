using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Nomad : MonoBehaviour
{
    [SerializeField]
    private Transform _spriteTransform;

    private bool _isIdle;

    private void Awake()
    {
        SetIdle();
    }

    public void SetIdle()
    {
        _isIdle = true;
        InvokeRepeating(nameof(FlipLookDirection), 0, Random.Range(5f, 7f));
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