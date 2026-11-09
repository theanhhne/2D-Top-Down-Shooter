
using System.Collections;
using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    [Header("Wave Text")]
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private float _wavePunchScale = 1.4f;
    [SerializeField] private float _wavePunchDuration = 0.35f;

    [Header("Boss Warning")]
    [SerializeField] private GameObject bossWarning;
    [SerializeField] private float bossShowDuration = 3f;
    [SerializeField] private float _bossPulseMin = 1.0f;
    [SerializeField] private float _bossPulseMax = 1.1f;
    [SerializeField] private float _bossPulseSpeed = 8f;

    private Vector3 _waveOriginalScale;
    private Vector3 _bossOriginalScale;
    private Coroutine _wavePunchCoroutine;
    private Coroutine _bossPulseCoroutine;

    private Coroutine bossCoroutine;

    private void Awake()
    {
        // Wave luôn hiển thị.
        if (waveText != null)
        {
            _waveOriginalScale = waveText.transform.localScale;
            waveText.text = "WAVE 1";
            waveText.gameObject.SetActive(true);
        }

        // Boss warning mặc định ẩn.
        if (bossWarning != null)
        {
            _bossOriginalScale = bossWarning.transform.localScale;
        }
        if (bossWarning != null)
        {
            bossWarning.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (_wavePunchCoroutine != null)
        {
            StopCoroutine(_wavePunchCoroutine);
            _wavePunchCoroutine = null;
        }

        if (waveText != null)
        {
            waveText.transform.localScale = _waveOriginalScale;
        }

        if (bossCoroutine != null)
        {
            StopCoroutine(bossCoroutine);
            bossCoroutine = null;
        }

        if (_bossPulseCoroutine != null)
        {
            StopCoroutine(_bossPulseCoroutine);
            _bossPulseCoroutine = null;
        }

        if (bossWarning != null)
        {
            bossWarning.transform.localScale = _bossOriginalScale;
        }
    }

    public void ShowWave(int wave, bool isBossWave)
    {
        if (waveText == null) return;

        // Chỉ đổi nội dung, không tự ẩn Wave Text.
        if (gameObject.activeInHierarchy)
        {
            if (_wavePunchCoroutine != null)
            {
                StopCoroutine(_wavePunchCoroutine);
            }
            _wavePunchCoroutine = StartCoroutine(WavePunchRoutine());
        }
        waveText.text = $"WAVE {wave}";
        waveText.gameObject.SetActive(true);
    }

    private IEnumerator WavePunchRoutine()
    {
        if (waveText == null) yield break;

        waveText.transform.localScale = _waveOriginalScale;
        float elapsed = 0f;
        while (elapsed < _wavePunchDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = _wavePunchDuration > 0f ? Mathf.Clamp01(elapsed / _wavePunchDuration) : 1f;
            float scaleMultiplier = Mathf.Lerp(1f, _wavePunchScale, Mathf.Sin(progress * Mathf.PI));
            if (waveText != null)
            {
                waveText.transform.localScale = _waveOriginalScale * scaleMultiplier;
            }
            yield return null;
        }

        if (waveText != null)
        {
            waveText.transform.localScale = _waveOriginalScale;
        }
        _wavePunchCoroutine = null;
    }

    public void ShowBossWarning()
    {
        if (bossWarning == null) return;

        bossWarning.SetActive(true);

        if (bossCoroutine != null)
        {
            StopCoroutine(bossCoroutine);
        }

        if (_bossPulseCoroutine != null)
        {
            StopCoroutine(_bossPulseCoroutine);
        }

        if (gameObject.activeInHierarchy)
        {
            bossCoroutine = StartCoroutine(HideBossAfterDelay());
            _bossPulseCoroutine = StartCoroutine(BossPulseRoutine());
        }
    }

    private IEnumerator BossPulseRoutine()
    {
        if (bossWarning == null) yield break;

        float time = 0f;
        while (bossWarning != null && bossWarning.activeSelf)
        {
            time += Time.unscaledDeltaTime * _bossPulseSpeed;
            float sin01 = (Mathf.Sin(time) + 1f) * 0.5f;
            float scaleMultiplier = Mathf.Lerp(_bossPulseMin, _bossPulseMax, sin01);
            bossWarning.transform.localScale = _bossOriginalScale * scaleMultiplier;
            yield return null;
        }

        if (bossWarning != null)
        {
            bossWarning.transform.localScale = _bossOriginalScale;
        }
        _bossPulseCoroutine = null;
    }

    private IEnumerator HideBossAfterDelay()
    {
        float elapsed = 0f;
        while (elapsed < bossShowDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (_bossPulseCoroutine != null)
        {
            StopCoroutine(_bossPulseCoroutine);
            _bossPulseCoroutine = null;
        }

        if (bossWarning != null)
        {
            bossWarning.transform.localScale = _bossOriginalScale;
        }

        if (bossWarning != null)
        {
            bossWarning.SetActive(false);
        }

        bossCoroutine = null;
    }
}
