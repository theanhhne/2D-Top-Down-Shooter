using UnityEngine;

public class EnemyDeathHitStop : MonoBehaviour
{
    [Header("Hit Stop Settings")]
    [SerializeField] private float duration = 0.04f;
    [SerializeField] private float timeScale = 0.05f;

    [Header("References (Optional)")]
    [SerializeField] private HealthController healthController;

    private void Awake()
    {
        if (healthController == null)
        {
            healthController = GetComponent<HealthController>();
        }

        if (healthController != null)
        {
            healthController.OnDied.AddListener(HandleDied);
        }
        else
        {
            Debug.LogError("EnemyDeathHitStop: Không tìm thấy HealthController trên " + gameObject.name, gameObject);
        }
    }

    private void HandleDied()
    {
        if (GameFeelManager.Instance != null)
        {
            GameFeelManager.Instance.HitStop(duration, timeScale);
        }
    }

    private void OnDestroy()
    {
        if (healthController != null)
        {
            healthController.OnDied.RemoveListener(HandleDied);
        }
    }
}
