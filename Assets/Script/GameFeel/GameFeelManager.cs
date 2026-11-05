using System.Collections;
using UnityEngine;

public class GameFeelManager : MonoBehaviour
{
    public static GameFeelManager Instance { get; private set; }

    [Header("Camera Settings")]
    [SerializeField] private Camera targetCamera;

    private Coroutine _shakeCoroutine;
    private Vector3 _originalLocalPos;

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
    }
}
