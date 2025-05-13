using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RandomSpriteSelector : MonoBehaviour
{
    [Tooltip("Array of sprites to randomly choose from.")]
    public Sprite[] sprites;

    private void Start()
    {
        if (sprites == null || sprites.Length == 0)
            return;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        int randomIndex = Random.Range(0, sprites.Length);
        spriteRenderer.sprite = sprites[randomIndex];
    }
}