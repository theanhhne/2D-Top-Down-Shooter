using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _rotationSpeed;

    [SerializeField]
    private float _screenBorder;

    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput;
    private Vector2 _smoothedMovementInput;
    private Vector2 _movementInputSmoothVelocity;
    private Camera _camera;
    private Animator _animator;
    private float _speedMultiplier = 1f;

    public float SpeedMultiplier
    {
        get => _speedMultiplier;
        set
        {
            _speedMultiplier = Mathf.Max(0f, value);

            if (_animator != null)
            {
                _animator.speed = _speedMultiplier;
            }
        }
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _camera = Camera.main;
        _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        SetPlayerVelocity();
        SetAnimation();
        RotateTowardsMouse();
    }
    private void SetAnimation()
    {
        bool isMoving = _movementInput != Vector2.zero;
        _animator.SetBool("IsMoving", isMoving);
    }

    private void SetPlayerVelocity()
    {
        _smoothedMovementInput = Vector2.SmoothDamp(
                    _smoothedMovementInput,
                    _movementInput,
                    ref _movementInputSmoothVelocity,
                    0.1f);

      Vector2 moveDirection =
    (Vector2)transform.right * _smoothedMovementInput.x +
    (Vector2)transform.up * _smoothedMovementInput.y;

_rigidbody.linearVelocity = moveDirection * (_speed * _speedMultiplier);

        PreventPlayerGoingOffScreen();
    }

    private void PreventPlayerGoingOffScreen()
    {
        Vector2 screenPosition = _camera.WorldToScreenPoint(transform.position);

        if ((screenPosition.x < _screenBorder && _rigidbody.linearVelocity.x < 0) ||
            (screenPosition.x > _camera.pixelWidth - _screenBorder && _rigidbody.linearVelocity.x > 0))
        {
            _rigidbody.linearVelocity = new Vector2(0, _rigidbody.linearVelocity.y);
        }

        if ((screenPosition.y < _screenBorder && _rigidbody.linearVelocity.y < 0) ||
            (screenPosition.y > _camera.pixelHeight - _screenBorder && _rigidbody.linearVelocity.y > 0))
        {
            _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0);
        }
    }

    private void RotateInDirectionOfInput()
    {
        if (_movementInput != Vector2.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _smoothedMovementInput);
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            _rigidbody.MoveRotation(rotation);
        }
    }
    private void RotateTowardsMouse()
{
    if (Mouse.current == null || _camera == null) return;

    Vector2 mouseScreen = Mouse.current.position.ReadValue();
    Vector3 mouseWorld = _camera.ScreenToWorldPoint(new Vector3(
        mouseScreen.x, mouseScreen.y, Mathf.Abs(_camera.transform.position.z)));

    Vector2 direction = (Vector2)mouseWorld - _rigidbody.position;
    if (direction.sqrMagnitude < 0.0001f) return;

    Quaternion target = Quaternion.LookRotation(Vector3.forward, direction);
    Quaternion rotation = Quaternion.RotateTowards(
        transform.rotation, target, _rotationSpeed * Time.fixedDeltaTime);

    _rigidbody.MoveRotation(rotation);
}

    private void OnMove(InputValue inputValue)
{
    _movementInput = inputValue.Get<Vector2>();

    if (_movementInput != Vector2.zero)
    {
        TutorialManager.Instance?.NotifyMovement();
    }
}
}
