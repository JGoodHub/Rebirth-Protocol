using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class SpritePositionSorter : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private float _multiplier = 10f;

    [SerializeField]
    private bool _updateLive;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * _multiplier);
    }

    private void LateUpdate()
    {
        if (_updateLive == false)
            return;

        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * _multiplier);
    }
}