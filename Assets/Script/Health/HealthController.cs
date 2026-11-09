
using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _maximumHealth;
    [SerializeField] private bool _showDamagePopup = true;
    [SerializeField] private float _fallbackHeadOffset = 0.6f;
    [SerializeField] private SpriteRenderer _headReference;

    private SpriteRenderer _spriteRenderer;

    private Collider2D _collider;

    public float RemainingHealthPercentage =>
        _maximumHealth > 0f ? _currentHealth / _maximumHealth : 0f;

    public bool IsInvincible { get; set; }

    public UnityEvent OnDied;
    public UnityEvent OnDamaged;
    public UnityEvent OnHealthChanged;

    private void Awake()
    {
        _spriteRenderer = _headReference != null ? _headReference : GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        if (OnDied == null) OnDied = new UnityEvent();
        if (OnDamaged == null) OnDamaged = new UnityEvent();
        if (OnHealthChanged == null) OnHealthChanged = new UnityEvent();
    }

    public void SetMaxHealth(float amount)
    {
        _maximumHealth = Mathf.Max(1f, amount);
        _currentHealth = _maximumHealth;
        OnHealthChanged?.Invoke();
    }

    public void TakeDamage(float damageAmount)
    {
        if (_currentHealth <= 0f || IsInvincible) return;

        _currentHealth = Mathf.Max(0f, _currentHealth - damageAmount);

        if (_showDamagePopup && damageAmount > 0f && DamagePopupSpawner.Instance != null)
        {
            Vector3 spawnPosition;
            if (_spriteRenderer != null && _spriteRenderer.enabled)
            {
                spawnPosition = new Vector3(_spriteRenderer.bounds.center.x, _spriteRenderer.bounds.max.y, transform.position.z);
            }
            else if (_collider != null && _collider.enabled)
            {
                spawnPosition = new Vector3(transform.position.x, _collider.bounds.max.y, transform.position.z);
            }
            else
            {
                spawnPosition = transform.position + Vector3.up * _fallbackHeadOffset;
            }

            DamagePopupSpawner.Instance.Spawn(spawnPosition, damageAmount);
        }

        OnHealthChanged?.Invoke();
        OnDamaged?.Invoke();

        if (_currentHealth <= 0f)
            OnDied?.Invoke();
    }

    public void AddHealth(float amountToAdd)
    {
        if (_currentHealth >= _maximumHealth) return;

        _currentHealth = Mathf.Min(
            _currentHealth + amountToAdd,
            _maximumHealth
        );

        OnHealthChanged?.Invoke();
    }
}
