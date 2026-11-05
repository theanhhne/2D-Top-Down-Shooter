using UnityEngine;
using UnityEngine.UI;


public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image foreground;
    [SerializeField] private Canvas healthCanvas;

    [Header("Settings")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 0.64f, 0f);
    [SerializeField] private bool hideWhenFull = false;
    [SerializeField] private string sortingLayerName = "Enemy";
    [SerializeField] private int sortingOrder = 10;

    private HealthController healthController;
    private Transform target;
    private Transform barRoot;
    private bool followTarget;

    private void Awake()
    {
        healthController = GetComponentInParent<HealthController>();

        // Tự tìm Canvas nếu chưa gán (hoạt động cả khi script ở gốc lẫn ở con)
        if (healthCanvas == null)
            healthCanvas = GetComponentInChildren<Canvas>(true);

        if (healthController == null || foreground == null)
        {
            Debug.LogError(
                "EnemyHealthBar: thiếu HealthController hoặc Foreground!",
                this
            );
            enabled = false;
            return;
        }

        target = healthController.transform;

        // Chỉ di chuyển Canvas của thanh máu, tuyệt đối không di chuyển enemy gốc
        if (healthCanvas != null && healthCanvas.transform != target)
        {
            barRoot = healthCanvas.transform;
            followTarget = true;
        }

        if (healthCanvas != null)
        {
            // Sorting Layer được so sánh trước Order, nên phải cùng layer với sprite enemy
            healthCanvas.overrideSorting = true;
            healthCanvas.sortingLayerName = sortingLayerName;
            healthCanvas.sortingOrder = sortingOrder;
        }

        SetupFill();

        healthController.OnHealthChanged.AddListener(UpdateBar);
        UpdateBar();
        FollowTarget();
    }

    private void SetupFill()
    {
        foreground.type = Image.Type.Filled;
        foreground.fillMethod = Image.FillMethod.Horizontal;

        // Nếu Foreground bị scale X âm thì thanh sẽ rút ngược chiều -> đổi gốc fill
        bool flipped = foreground.rectTransform.localScale.x < 0f;
        foreground.fillOrigin = flipped
            ? (int)Image.OriginHorizontal.Right
            : (int)Image.OriginHorizontal.Left;
    }

    private void LateUpdate()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        if (!followTarget || target == null) return;

        // Giữ thanh máu thẳng đứng và nằm trên đầu enemy, bất kể enemy xoay
        barRoot.rotation = Quaternion.identity;
        barRoot.position = target.position + worldOffset;
    }

    private void UpdateBar()
    {
        if (foreground == null || healthController == null) return;

        float percent = healthController.RemainingHealthPercentage;
        foreground.fillAmount = percent;

        if (healthCanvas != null)
        {
            // Enemy chết (máu = 0) thì ẩn thanh máu ngay, không đợi enemy bị Destroy
            bool isDead = percent <= 0f;
            bool hideBecauseFull = hideWhenFull && percent >= 1f;
            healthCanvas.enabled = !isDead && !hideBecauseFull;
        }
    }

    private void OnDestroy()
    {
        if (healthController != null)
            healthController.OnHealthChanged.RemoveListener(UpdateBar);
    }
}