using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _rotationSpeed;

    [SerializeField]
    private float _knockbackDuration = 0.12f;

    [SerializeField, Range(0f, 1f)]
    private float _knockbackResistance = 0f;

    [SerializeField]
    private float _maxKnockbackSpeed = 6f;

    private Rigidbody2D _rigidbody;
    private PlayerAwareness _playerAwarenessController;
    private Vector2 _targetDirection;
    private float _changeDirectionCooldown;
    private Vector2 _knockbackVelocity;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _playerAwarenessController = GetComponent<PlayerAwareness>();
        _targetDirection = transform.up;
    }

    private void FixedUpdate()
    {
        UpdateTargetDirection();
        RotateTowardsTarget();
        _knockbackVelocity = Vector2.MoveTowards(_knockbackVelocity, Vector2.zero, (_maxKnockbackSpeed / _knockbackDuration) * Time.fixedDeltaTime);
        SetVelocity();
    }

    private void UpdateTargetDirection()
    {
        HandleRandomDirectionChange();
        HandlePlayerTargeting();
        HandleEnemyOutOfBounds();
    }

    private void HandleRandomDirectionChange()
    {
        _changeDirectionCooldown -= Time.deltaTime;

        if (_changeDirectionCooldown <= 0)
        {
            float angleChange = Random.Range(-90f, 90f);
            Quaternion rotation = Quaternion.AngleAxis(angleChange, transform.forward);
            _targetDirection = rotation * _targetDirection;

            _changeDirectionCooldown = Random.Range(1f, 5f);
        }
    }

    private void HandlePlayerTargeting()
    {
        if (_playerAwarenessController.AwareOfPlayer)
        {
            _targetDirection = _playerAwarenessController.DirectionToPlayer;
        }
    }

    // Enemy bật lại khi ra ngoài ArenaBounds và đang đi ra xa. Enemy spawn bên ngoài biên
    // (spawner đặt ngoài map) vẫn đi vào bình thường vì hướng của chúng đang quay vào trong.
    private void HandleEnemyOutOfBounds()
    {
        ArenaBounds arena = ArenaBounds.Instance;
        if (arena == null) return;

        Vector2 position = transform.position;

        if ((position.x < arena.Min.x && _targetDirection.x < 0f) ||
            (position.x > arena.Max.x && _targetDirection.x > 0f))
        {
            _targetDirection = new Vector2(-_targetDirection.x, _targetDirection.y);
        }

        if ((position.y < arena.Min.y && _targetDirection.y < 0f) ||
            (position.y > arena.Max.y && _targetDirection.y > 0f))
        {
            _targetDirection = new Vector2(_targetDirection.x, -_targetDirection.y);
        }
    }

    private void RotateTowardsTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _targetDirection);
        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        _rigidbody.SetRotation(rotation);
    }

    public void ApplyKnockback(Vector2 direction, float strength)
    {
        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Vector2 knockback = direction.normalized * strength * (1f - _knockbackResistance);
        _knockbackVelocity = Vector2.ClampMagnitude(_knockbackVelocity + knockback, _maxKnockbackSpeed);
    }

    private void SetVelocity()
    {
        _rigidbody.linearVelocity = (Vector2)transform.up * _speed + _knockbackVelocity;
    }
}