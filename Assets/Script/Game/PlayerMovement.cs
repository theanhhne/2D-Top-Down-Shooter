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
    [Tooltip("Khoảng cách giữ lại giữa người chơi và mép ArenaBounds (đơn vị world).")]
    private float _boundsPadding = 0.6f;

    private Rigidbody2D _rigidbody;
    private Vector2 _movementInput;
    private Vector2 _smoothedMovementInput;
    private Vector2 _movementInputSmoothVelocity;
    private Camera _camera;
    private Animator _animator;
    private float _speedMultiplier = 1f;
    private bool _warnedNoArena;

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

        Vector2 moveDirection = _smoothedMovementInput;

_rigidbody.linearVelocity = moveDirection * (_speed * _speedMultiplier);

        ConstrainToArena();
    }

    private void ConstrainToArena()
    {
        ArenaBounds arena = ArenaBounds.Instance;
        if (arena == null)
        {
            if (!_warnedNoArena)
            {
                _warnedNoArena = true;
                Debug.LogWarning("PlayerMovement: không tìm thấy ArenaBounds trong scene, người chơi sẽ không bị giới hạn.", this);
            }
            return;
        }

        Vector2 min = arena.Min + Vector2.one * _boundsPadding;
        Vector2 max = arena.Max - Vector2.one * _boundsPadding;

        Vector2 position = _rigidbody.position;
        Vector2 velocity = _rigidbody.linearVelocity;

        // Chạm biên thì chặn thành phần vận tốc đang đẩy ra ngoài (vẫn trượt dọc theo biên).
        if ((position.x <= min.x && velocity.x < 0f) || (position.x >= max.x && velocity.x > 0f))
        {
            velocity.x = 0f;
        }

        if ((position.y <= min.y && velocity.y < 0f) || (position.y >= max.y && velocity.y > 0f))
        {
            velocity.y = 0f;
        }

        _rigidbody.linearVelocity = velocity;

        // Phòng trường hợp bị đẩy lố (knockback, va chạm) ra ngoài biên.
        Vector2 clamped = new Vector2(
            Mathf.Clamp(position.x, min.x, max.x),
            Mathf.Clamp(position.y, min.y, max.y));

        if (clamped != position)
        {
            _rigidbody.position = clamped;
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