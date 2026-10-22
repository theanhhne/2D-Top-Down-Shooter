
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image foreground;
    [SerializeField] private Canvas healthCanvas;

    private HealthController healthController;

    private void Awake()
    {
        healthController = GetComponentInParent<HealthController>();

        if (healthCanvas == null)
            healthCanvas = GetComponent<Canvas>();

        if (healthController == null || foreground == null)
        {
            Debug.LogError(
                "EnemyHealthBar: thiếu HealthController hoặc Foreground!",
                this
            );
            enabled = false;
            return;
        }

        if (healthCanvas != null)
        {
            healthCanvas.overrideSorting = true;
            healthCanvas.sortingOrder = 10;
        }

        healthController.OnHealthChanged.AddListener(UpdateBar);
        UpdateBar();
    }

    private void UpdateBar()
    {
        if (foreground != null && healthController != null)
        {
            foreground.fillAmount =
                healthController.RemainingHealthPercentage;
        }
    }

    private void OnDestroy()
    {
        if (healthController != null)
            healthController.OnHealthChanged.RemoveListener(UpdateBar);
    }
}
