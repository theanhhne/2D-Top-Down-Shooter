
using UnityEngine;

public class EnemyWaveMember : MonoBehaviour
{
    private HealthController healthController;
    private WaveManager waveManager;
    private bool reportedDeath;

    private void Awake()
    {
        healthController = GetComponent<HealthController>();

        if (healthController == null)
        {
            Debug.LogError("Enemy cần có HealthController!", gameObject);
            return;
        }

        healthController.OnDied.AddListener(HandleDied);
    }

    public void Initialize(WaveManager manager)
    {
        waveManager = manager;
    }

    private void HandleDied()
    {
        if (reportedDeath) return;

        reportedDeath = true;

        if (waveManager != null)
        {
            waveManager.OnEnemyDied(this);
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
