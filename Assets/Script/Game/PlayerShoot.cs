using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField]
    private GameObject _bulletPrefab;

    [SerializeField]
    private float _bulletSpeed;

    [SerializeField]
    private Transform _gunOffset;

    [SerializeField]
    private float _timeBetweenShots;

    private bool _fireContinuously;
    private bool _fireSingle;
    private float _lastFireTime;
    [SerializeField] private int _bulletCount = 1;
    [SerializeField] private int _maxBulletCount = 7;
    [SerializeField] private float _spreadAngle = 10f;


    void Update()
    {
        if (_fireContinuously || _fireSingle)
        {
            float timeSinceLastFire = Time.time - _lastFireTime;

            if (timeSinceLastFire >= _timeBetweenShots)
            {
                FireBullet();

                _lastFireTime = Time.time;
                _fireSingle = false;
            }
        }
    }
    public void AddBullets(int amount)
    {
        _bulletCount = Mathf.Min(_bulletCount + amount, _maxBulletCount);
    }


    private void FireBullet()
    {
        for (int i = 0; i < _bulletCount; i++)
        {
            // Trải đều các viên đạn quanh hướng đang nhìn, viên giữa thẳng hướng
            float angleOffset = (i - (_bulletCount - 1) / 2f) * _spreadAngle;
            Quaternion rotation = transform.rotation * Quaternion.Euler(0f, 0f, angleOffset);

            GameObject bullet = Instantiate(_bulletPrefab, _gunOffset.position, rotation);
            Rigidbody2D rigidbody = bullet.GetComponent<Rigidbody2D>();

            rigidbody.linearVelocity = _bulletSpeed * (rotation * Vector3.up);
        }
    }

    private void OnAttack(InputValue inputValue)
    {
        _fireContinuously = inputValue.isPressed;

        if (inputValue.isPressed)
        {
             _fireSingle = true;
        }
    }
}
