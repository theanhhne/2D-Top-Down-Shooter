using System.Collections;
using UnityEngine;

/// <summary>
/// Quản lý buff tốc độ chạy của Player. Tự được thêm vào Player khi nhặt vật phẩm,
/// không cần gắn tay. Nhặt thêm khi đang buff sẽ làm mới thời gian (không cộng dồn hệ số).
/// </summary>
[RequireComponent(typeof(PlayerMovement))]
public class SpeedBoostController : MonoBehaviour
{
    private PlayerMovement _playerMovement;
    private Coroutine _boostCoroutine;

    public bool IsBoosted => _boostCoroutine != null;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
    }

    public void StartBoost(float speedMultiplier, float duration)
    {
        if (_boostCoroutine != null)
        {
            StopCoroutine(_boostCoroutine);
        }

        _boostCoroutine = StartCoroutine(BoostCoroutine(speedMultiplier, duration));
    }

    private IEnumerator BoostCoroutine(float speedMultiplier, float duration)
    {
        _playerMovement.SpeedMultiplier = speedMultiplier;
        yield return new WaitForSeconds(duration);

        _playerMovement.SpeedMultiplier = 1f;
        _boostCoroutine = null;
    }

    private void OnDisable()
    {
        // Phòng khi Player bị tắt giữa chừng buff
        if (_playerMovement != null)
        {
            _playerMovement.SpeedMultiplier = 1f;
        }

        _boostCoroutine = null;
    }
}
