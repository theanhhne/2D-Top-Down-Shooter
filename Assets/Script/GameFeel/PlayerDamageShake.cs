using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class PlayerDamageShake : MonoBehaviour
{
    [Header("Screen Shake Settings")]
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private float magnitude = 0.3f;

    private HealthController health;

    private void Awake()
    {
        health = GetComponent<HealthController>();
    }

    private void OnEnable()
    {
        if (health == null)
            health = GetComponent<HealthController>();

        if (health != null && health.OnDamaged != null)
            health.OnDamaged.AddListener(OnDamaged);
    }

    private void OnDisable()
    {
        if (health != null && health.OnDamaged != null)
            health.OnDamaged.RemoveListener(OnDamaged);
    }

    private void OnDamaged()
    {
        if (GameFeelManager.Instance != null)
        {
            GameFeelManager.Instance.Shake(duration, magnitude);
        }
    }
}
