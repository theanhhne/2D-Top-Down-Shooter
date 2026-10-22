
using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _maximumHealth;

    public float RemainingHealthPercentage =>
        _maximumHealth > 0f ? _currentHealth / _maximumHealth : 0f;

    public bool IsInvincible { get; set; }

    public UnityEvent OnDied;
    public UnityEvent OnDamaged;
    public UnityEvent OnHealthChanged;

    private void Awake()
    {
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
