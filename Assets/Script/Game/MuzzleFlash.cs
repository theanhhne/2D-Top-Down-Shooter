
using System.Collections;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [SerializeField] private float flashDuration = 0.06f;
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private Vector2 _scaleRange = new Vector2(0.8f, 1.2f);
    [SerializeField] private float _rotationJitter = 10f;

    private SpriteRenderer spriteRenderer;
    private Coroutine flashCoroutine;
    private Vector3 _originalScale;
    private Quaternion _originalRotation;
    private Color _initialColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        _originalScale = transform.localScale;
        _originalRotation = transform.localRotation;

        if (spriteRenderer != null)
        {
            _initialColor = spriteRenderer.color;
            spriteRenderer.enabled = false;
        }
    }

    public void Play()
    {
        if (spriteRenderer == null) return;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            ResetTransformAndColor();
        }

        if (_sprites != null && _sprites.Length > 0)
        {
            Sprite selectedSprite = _sprites[Random.Range(0, _sprites.Length)];
            if (selectedSprite != null)
            {
                spriteRenderer.sprite = selectedSprite;
            }
        }

        float randomScale = Random.Range(_scaleRange.x, _scaleRange.y);
        transform.localScale = _originalScale * randomScale;

        float angleOffset = Random.Range(-_rotationJitter, _rotationJitter);
        transform.localRotation = _originalRotation * Quaternion.Euler(0f, 0f, angleOffset);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = _initialColor;
        spriteRenderer.enabled = true;

        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = flashDuration > 0f ? Mathf.Clamp01(elapsed / flashDuration) : 1f;
            float currentAlpha = Mathf.Lerp(_initialColor.a, 0f, t);
            spriteRenderer.color = new Color(_initialColor.r, _initialColor.g, _initialColor.b, currentAlpha);
            yield return null;
        }

        spriteRenderer.enabled = false;
        ResetTransformAndColor();
        flashCoroutine = null;
    }

    private void ResetTransformAndColor()
    {
        transform.localScale = _originalScale;
        transform.localRotation = _originalRotation;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = _initialColor;
        }
    }
}
