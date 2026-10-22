
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float _damageAmount = 10f;

    public void SetDamage(float damage)
    {
        _damageAmount = Mathf.Max(0f, damage);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.TryGetComponent<PlayerMovement>(out _))
            return;

        HealthController healthController =
            collision.gameObject.GetComponent<HealthController>();

        if (healthController != null)
            healthController.TakeDamage(_damageAmount);
    }
}
