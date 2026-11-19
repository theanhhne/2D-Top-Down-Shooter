using UnityEngine;
using UnityEngine.InputSystem;

/// Camera đi theo Player và lệch về phía con trỏ chuột (look-ahead).
/// Gắn vào Main Camera. Rung màn hình đi qua ShakeOffset (xem GameFeelManager).
[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform _target;

    [Header("Mouse Look-Ahead")]
    [Tooltip("0 = camera dính chặt Player, 1 = camera nhìn thẳng vào con trỏ.")]
    [SerializeField, Range(0f, 1f)] private float _mouseInfluence = 0.25f;
    [Tooltip("Khoảng lệch tối đa khỏi Player (đơn vị world).")]
    [SerializeField] private float _maxLookDistance = 3f;
    [SerializeField] private float _smoothTime = 0.15f;

    [Header("Arena Bounds")]
    [Tooltip("Kẹp camera trong ArenaBounds để không lộ ra ngoài map.")]
    [SerializeField] private bool _clampToArena = true;

    /// GameFeelManager ghi vào đây để rung camera.
    public Vector3 ShakeOffset { get; set; }

    private Camera _camera;
    private Vector3 _basePosition;
    private Vector3 _velocity;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _basePosition = transform.position;
    }

    private void Start()
    {
        if (_target != null)
        {
            _basePosition = ComputeDesiredPosition();
            transform.position = _basePosition;
        }
    }

    private void OnDisable()
    {
        ShakeOffset = Vector3.zero;
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        // Đang pause thì giữ nguyên camera.
        if (Mathf.Approximately(Time.timeScale, 0f)) return;

        Vector3 desired = ComputeDesiredPosition();

        // unscaledDeltaTime để camera không bị chậm lại trong lúc hit-stop.
        _basePosition = Vector3.SmoothDamp(
            _basePosition, desired, ref _velocity, _smoothTime,
            Mathf.Infinity, Time.unscaledDeltaTime);

        transform.position = _basePosition + ShakeOffset;
    }

    private Vector3 ComputeDesiredPosition()
    {
        Vector2 look = Vector2.zero;

        if (Mouse.current != null)
        {
            // Tính offset từ vị trí chuột trên viewport (không phụ thuộc vị trí camera),
            // nên không có vòng lặp phản hồi camera <-> chuột.
            Vector2 viewport = _camera.ScreenToViewportPoint(Mouse.current.position.ReadValue());
            Vector2 fromCentre = (viewport - new Vector2(0.5f, 0.5f)) * 2f;   // -1..1
            fromCentre = Vector2.ClampMagnitude(fromCentre, 1f);

            float halfH = _camera.orthographicSize;
            float halfW = halfH * _camera.aspect;

            look = new Vector2(fromCentre.x * halfW, fromCentre.y * halfH) * _mouseInfluence;
            look = Vector2.ClampMagnitude(look, _maxLookDistance);
        }

        Vector3 desired = new Vector3(
            _target.position.x + look.x,
            _target.position.y + look.y,
            _basePosition.z);

        ArenaBounds arena = ArenaBounds.Instance;
        if (_clampToArena && arena != null)
        {
            float halfH = _camera.orthographicSize;
            float halfW = halfH * _camera.aspect;

            desired.x = ClampAxis(desired.x, arena.Min.x, arena.Max.x, halfW);
            desired.y = ClampAxis(desired.y, arena.Min.y, arena.Max.y, halfH);
        }

        return desired;
    }

    // Nếu đấu trường nhỏ hơn khung nhìn thì căn giữa, ngược lại kẹp để không lộ ra ngoài map.
    private static float ClampAxis(float value, float min, float max, float half)
    {
        if (max - min <= half * 2f) return (min + max) * 0.5f;
        return Mathf.Clamp(value, min + half, max - half);
    }
}