
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneFade : MonoBehaviour
{
    private Image _sceneFadeImage;

    private void Awake()
    {
        _sceneFadeImage = GetComponent<Image>();
    }

    public IEnumerator FadeInCoroutine(float duration)
    {
        if (_sceneFadeImage == null)
            yield break;

        gameObject.SetActive(true);

        Color color = _sceneFadeImage.color;
        color.a = 1f;
        _sceneFadeImage.color = color;

        yield return FadeCoroutine(0f, duration);

        gameObject.SetActive(false);
    }

    public IEnumerator FadeOutCoroutine(float duration)
    {
        if (_sceneFadeImage == null)
            yield break;

        gameObject.SetActive(true);

        Color color = _sceneFadeImage.color;
        color.a = 0f;
        _sceneFadeImage.color = color;

        yield return FadeCoroutine(1f, duration);
    }

    private IEnumerator FadeCoroutine(float targetAlpha, float duration)
    {
        Color color = _sceneFadeImage.color;
        float startAlpha = color.a;

        if (duration <= 0f)
        {
            color.a = targetAlpha;
            _sceneFadeImage.color = color;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            _sceneFadeImage.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        _sceneFadeImage.color = color;
    }
}
