using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float _knockbackStrength = 3f;

    [SerializeField]
    private GameObject _hitEffectPrefab;

    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;    
    }

    private void Update()
    {
        DestroyWhenOffScreen();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyMovement enemyMovement = collision.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.ApplyKnockback(transform.up, _knockbackStrength);
            HealthController healthController = collision.GetComponent<HealthController>();
            healthController.TakeDamage(10);

            if (_hitEffectPrefab != null)
            {
                Instantiate(_hitEffectPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    private void DestroyWhenOffScreen()
    {
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);

        if (screenPosition.x < 0 ||
            screenPosition.x > _camera.pixelWidth ||
            screenPosition.y < 0 ||
            screenPosition.y > _camera.pixelHeight)
        {

            Destroy(gameObject);
        }
    }
}
