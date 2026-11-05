using System.Collections;
using UnityEngine;

public class GameFeelManager : MonoBehaviour
{
    public static GameFeelManager Instance { get; private set; }

    [Header("Camera Settings")]
    [SerializeField] private Camera targetCamera;

    private Coroutine _shakeCoroutine;
    private Vector3 _originalLocalPos;

    private Coroutine _hitStopCoroutine;
    private bool _isHitStopping;
    private float _currentHitStopTimeScale;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera != null)
        {
            _originalLocalPos = targetCamera.transform.localPosition;
        }
    }

    public void Shake(float duration, float magnitude)
    {
        if (duration <= 0f || magnitude <= 0f)
            return;

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null)
                return;
            _originalLocalPos = targetCamera.transform.localPosition;
        }

        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            targetCamera.transform.localPosition = _originalLocalPos;
        }
        else
        {
            _originalLocalPos = targetCamera.transform.localPosition;
        }

        _shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            targetCamera.transform.localPosition = _originalLocalPos + new Vector3(x, y, 0f);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        targetCamera.transform.localPosition = _originalLocalPos;
        _shakeCoroutine = null;
    }

    public void HitStop(float duration, float timeScale = 0.05f)
    {
        // (1) Kiểm tra thời lượng; (3) nếu đang pause (timeScale == 0) thì bỏ qua
        if (duration <= 0f || Mathf.Approximately(Time.timeScale, 0f))
            return;

        // (5) nếu đang có hit-stop khác thì không chồng lên nhau
        if (_isHitStopping)
            return;

        _hitStopCoroutine = StartCoroutine(HitStopRoutine(duration, timeScale));
    }

    private IEnumerator HitStopRoutine(float duration, float targetTimeScale)
    {
        _isHitStopping = true;
        _currentHitStopTimeScale = targetTimeScale;
        Time.timeScale = targetTimeScale;

        // (2) Chờ bằng WaitForSecondsRealtime
        yield return new WaitForSecondsRealtime(duration);

        // (4) khi hết thời gian chỉ khôi phục về 1 nếu Time.timeScale vẫn đúng bằng giá trị hit-stop (không ai khác đổi nó)
        if (Mathf.Approximately(Time.timeScale, targetTimeScale))
        {
            Time.timeScale = 1f;
        }

        _isHitStopping = false;
        _hitStopCoroutine = null;
    }

    private void ResetHitStop()
    {
        if (_hitStopCoroutine != null)
        {
            StopCoroutine(_hitStopCoroutine);
            _hitStopCoroutine = null;
        }

        if (_isHitStopping)
        {
            // (6) trong OnDisable/OnDestroy nếu đang trong hit-stop thì khôi phục timeScale về 1
            if (Mathf.Approximately(Time.timeScale, _currentHitStopTimeScale))
            {
                Time.timeScale = 1f;
            }
            _isHitStopping = false;
        }
    }

    private void OnDisable()
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            _shakeCoroutine = null;
            if (targetCamera != null)
            {
                targetCamera.transform.localPosition = _originalLocalPos;
            }
        }

        ResetHitStop();
    }

    private void OnDestroy()
    {
        ResetHitStop();
    }
}
