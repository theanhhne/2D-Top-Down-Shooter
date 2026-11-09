using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshPro _text;

    [Header("Sorting")]
    [SerializeField] private string _sortingLayerName = "Enemy";
    [SerializeField] private int _sortingOrder = 100;

    [Header("Movement & Animation")]
    [SerializeField] private float _riseDistance = 1.0f;
    [SerializeField] private float _lifetime = 0.6f;
    [SerializeField] private float _randomX = 0.35f;
    [SerializeField] private float _punchScale = 1.3f;
    [SerializeField] private float _punchDuration = 0.15f;

    [Header("Appearance & Thresholds")]
    [SerializeField] private float _bigHitThreshold = 20f;
    [SerializeField] private float _bigHitScaleMultiplier = 1.35f;
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _bigHitColor = new Color(1f, 0.85f, 0.2f);

    private MeshRenderer _meshRenderer;
    private Vector3 _originalScale;
    private Coroutine _animateCoroutine;
    private Action<DamagePopup> _onComplete;
    private bool _returned = true;

    private void Awake()
    {
        if (_text == null)
        {
            _text = GetComponent<TextMeshPro>();
        }

        _meshRenderer = GetComponent<MeshRenderer>();
        if (_text != null)
        {
            _text.margin = Vector4.zero;
            _text.alignment = TextAlignmentOptions.Center;
        }

        if (_meshRenderer == null && _text != null)
        {
            _meshRenderer = _text.GetComponent<MeshRenderer>();
        }

        ApplySorting();

        _originalScale = transform.localScale;
        if (_originalScale == Vector3.zero)
        {
            _originalScale = Vector3.one;
        }
    }

    public void ApplySorting()
    {
        if (_meshRenderer != null)
        {
            if (!string.IsNullOrEmpty(_sortingLayerName))
            {
                _meshRenderer.sortingLayerName = _sortingLayerName;
            }
            _meshRenderer.sortingOrder = _sortingOrder;
        }
    }

    public void Setup(float damage, Action<DamagePopup> onComplete)
    {
        _returned = false;
        _onComplete = onComplete;

        if (_animateCoroutine != null)
        {
            StopCoroutine(_animateCoroutine);
            _animateCoroutine = null;
        }

        ApplySorting();

        bool isBigHit = damage >= _bigHitThreshold;
        Color baseColor = isBigHit ? _bigHitColor : _normalColor;
        float baseScaleMult = isBigHit ? _bigHitScaleMultiplier : 1f;

        if (_text != null)
        {
            _text.margin = Vector4.zero;
            _text.alignment = TextAlignmentOptions.Center;
            _text.text = Mathf.RoundToInt(damage).ToString();
            _text.color = baseColor;
        }

        Vector3 startPosition = transform.position + new Vector3(UnityEngine.Random.Range(-_randomX, _randomX), 0f, 0f);
        transform.position = startPosition;
        transform.localScale = _originalScale * baseScaleMult;

        _animateCoroutine = StartCoroutine(AnimateRoutine(startPosition, baseColor, baseScaleMult));
    }

    private IEnumerator AnimateRoutine(Vector3 startPosition, Color baseColor, float baseScaleMult)
    {
        // startPosition is captured at call time
        Vector3 targetPosition = startPosition + Vector3.up * _riseDistance;

        Vector3 targetScale = _originalScale * baseScaleMult;
        Vector3 punchScale = targetScale * _punchScale;

        float elapsed = 0f;
        while (elapsed < _lifetime)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = _lifetime > 0f ? Mathf.Clamp01(elapsed / _lifetime) : 1f;

            // Di chuyển bay lên
            transform.position = Vector3.Lerp(startPosition, targetPosition, progress);

            // Punch scale lúc đầu
            if (elapsed < _punchDuration && _punchDuration > 0f)
            {
                float punchProgress = Mathf.Clamp01(elapsed / _punchDuration);
                transform.localScale = Vector3.Lerp(punchScale, targetScale, punchProgress);
            }
            else
            {
                transform.localScale = targetScale;
            }

            // Alpha mờ dần về 0
            if (_text != null)
            {
                float currentAlpha = Mathf.Lerp(baseColor.a, 0f, progress);
                _text.color = new Color(baseColor.r, baseColor.g, baseColor.b, currentAlpha);
            }

            yield return null;
        }

        ReturnToPool();
    }

    public void CancelAndReturn()
    {
        if (_returned) return;

        if (_animateCoroutine != null)
        {
            StopCoroutine(_animateCoroutine);
            _animateCoroutine = null;
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (_returned) return;
        _returned = true;

        if (_animateCoroutine != null)
        {
            StopCoroutine(_animateCoroutine);
            _animateCoroutine = null;
        }

        transform.localScale = _originalScale;
        if (_text != null)
        {
            _text.color = _normalColor;
        }

        Action<DamagePopup> callback = _onComplete;
        _onComplete = null;
        callback?.Invoke(this);
    }

    private void OnDisable()
    {
        if (!_returned)
        {
            ReturnToPool();
        }
    }
}
