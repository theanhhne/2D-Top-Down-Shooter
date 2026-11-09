using System.Collections;
using UnityEngine;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Image _healthBarForegroundImage;

    [SerializeField]
    private UnityEngine.UI.Image _delayedImage;

    [SerializeField]
    private float _delayBeforeTrail = 0.35f;

    [SerializeField]
    private float _trailSpeed = 0.6f;

    [SerializeField]
    private float _punchScale = 1.1f;

    [SerializeField]
    private float _punchDuration = 0.15f;

    private Vector3 _originalScale;
    private bool _initialized;
    private float _lastHealthPercentage;
    private Coroutine _trailCoroutine;
    private Coroutine _punchCoroutine;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    private void OnDisable()
    {
        if (_trailCoroutine != null)
        {
            StopCoroutine(_trailCoroutine);
            _trailCoroutine = null;
        }

        if (_punchCoroutine != null)
        {
            StopCoroutine(_punchCoroutine);
            _punchCoroutine = null;
        }

        transform.localScale = _originalScale;
    }

    public void UpdateHealthBar(HealthController healthController)
    {
        if (healthController == null) return;

        float targetFill = Mathf.Clamp01(healthController.RemainingHealthPercentage);

        if (_healthBarForegroundImage != null)
        {
            _healthBarForegroundImage.fillAmount = targetFill;
        }

        if (!_initialized)
        {
            _initialized = true;
            _lastHealthPercentage = targetFill;
            if (_delayedImage != null)
            {
                _delayedImage.fillAmount = targetFill;
            }
            return;
        }

        bool tookDamage = targetFill < _lastHealthPercentage;
        _lastHealthPercentage = targetFill;

        if (tookDamage && gameObject.activeInHierarchy)
        {
            if (_punchCoroutine != null)
            {
                StopCoroutine(_punchCoroutine);
            }
            _punchCoroutine = StartCoroutine(PunchRoutine());
        }

        if (_delayedImage != null)
        {
            if (targetFill >= _delayedImage.fillAmount)
            {
                if (_trailCoroutine != null)
                {
                    StopCoroutine(_trailCoroutine);
                    _trailCoroutine = null;
                }
                _delayedImage.fillAmount = targetFill;
            }
            else
            {
                if (gameObject.activeInHierarchy)
                {
                    if (_trailCoroutine != null)
                    {
                        StopCoroutine(_trailCoroutine);
                    }
                    _trailCoroutine = StartCoroutine(TrailRoutine(targetFill));
                }
                else
                {
                    _delayedImage.fillAmount = targetFill;
                }
            }
        }
    }

    private IEnumerator TrailRoutine(float targetFill)
    {
        float waitTime = _delayBeforeTrail;
        while (waitTime > 0f)
        {
            waitTime -= Time.unscaledDeltaTime;
            yield return null;
        }

        while (_delayedImage != null && _delayedImage.fillAmount > targetFill)
        {
            _delayedImage.fillAmount = Mathf.MoveTowards(_delayedImage.fillAmount, targetFill, _trailSpeed * Time.unscaledDeltaTime);
            yield return null;
        }

        if (_delayedImage != null)
        {
            _delayedImage.fillAmount = targetFill;
        }

        _trailCoroutine = null;
    }

    private IEnumerator PunchRoutine()
    {
        transform.localScale = _originalScale;
        float elapsed = 0f;
        while (elapsed < _punchDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = _punchDuration > 0f ? Mathf.Clamp01(elapsed / _punchDuration) : 1f;
            float scaleMultiplier = Mathf.Lerp(1f, _punchScale, Mathf.Sin(progress * Mathf.PI));
            transform.localScale = _originalScale * scaleMultiplier;
            yield return null;
        }

        transform.localScale = _originalScale;
        _punchCoroutine = null;
    }
}
