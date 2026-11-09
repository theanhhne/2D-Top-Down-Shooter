using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private float _punchScale = 1.25f;
    [SerializeField] private float _punchDuration = 0.15f;
    [SerializeField] private float _countDuration = 0.25f;

    private TMP_Text _scoreText;
    private Vector3 _originalScale;
    private int _displayedScore = 0;
    private Coroutine _punchCoroutine;
    private Coroutine _countCoroutine;

    private void Awake()
    {
        _scoreText = GetComponent<TMP_Text>();
        _originalScale = transform.localScale;
    }

    private void OnDisable()
    {
        if (_punchCoroutine != null)
        {
            StopCoroutine(_punchCoroutine);
            _punchCoroutine = null;
        }

        if (_countCoroutine != null)
        {
            StopCoroutine(_countCoroutine);
            _countCoroutine = null;
        }

        transform.localScale = _originalScale;
    }

    public void UpdateScore(ScoreController scoreController)
    {
        if (scoreController == null || _scoreText == null) return;

        int targetScore = scoreController.Score;

        if (!gameObject.activeInHierarchy)
        {
            _displayedScore = targetScore;
            _scoreText.text = $"Score: {_displayedScore}";
            return;
        }

        if (_punchCoroutine != null)
        {
            StopCoroutine(_punchCoroutine);
        }
        _punchCoroutine = StartCoroutine(PunchRoutine());

        if (targetScore < _displayedScore)
        {
            if (_countCoroutine != null)
            {
                StopCoroutine(_countCoroutine);
                _countCoroutine = null;
            }
            _displayedScore = targetScore;
            _scoreText.text = $"Score: {_displayedScore}";
        }
        else if (targetScore > _displayedScore)
        {
            if (_countCoroutine != null)
            {
                StopCoroutine(_countCoroutine);
            }
            _countCoroutine = StartCoroutine(CountRoutine(targetScore));
        }
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

    private IEnumerator CountRoutine(int targetScore)
    {
        int startScore = _displayedScore;
        float elapsed = 0f;

        while (elapsed < _countDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = _countDuration > 0f ? Mathf.Clamp01(elapsed / _countDuration) : 1f;
            int nextScore = Mathf.RoundToInt(Mathf.Lerp(startScore, targetScore, progress));

            if (nextScore <= _displayedScore && _displayedScore < targetScore)
            {
                nextScore = _displayedScore + 1;
            }

            _displayedScore = Mathf.Min(nextScore, targetScore);
            _scoreText.text = $"Score: {_displayedScore}";

            if (_displayedScore >= targetScore)
            {
                break;
            }

            yield return null;
        }

        _displayedScore = targetScore;
        _scoreText.text = $"Score: {_displayedScore}";
        _countCoroutine = null;
    }
}