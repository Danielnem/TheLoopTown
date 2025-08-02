using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RandomSpriteSelector : MonoBehaviour
{
    [Tooltip("Assign all possible sprites here")]
    public Sprite[] spriteOptions;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void RerollSprite()
    {
        if (spriteOptions == null || spriteOptions.Length == 0)
        {
            Debug.LogWarning("No sprites assigned to RandomSpriteSelector on " + gameObject.name);
            return;
        }

        Sprite chosenSprite = spriteOptions[Random.Range(0, spriteOptions.Length)];
        spriteRenderer.sprite = chosenSprite;
    }
}
