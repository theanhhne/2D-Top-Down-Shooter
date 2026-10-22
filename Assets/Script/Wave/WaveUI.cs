
using System.Collections;
using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    [Header("Wave Text")]
    [SerializeField] private TMP_Text waveText;

    [Header("Boss Warning")]
    [SerializeField] private GameObject bossWarning;
    [SerializeField] private float bossShowDuration = 3f;

    private Coroutine bossCoroutine;

    private void Awake()
    {
        // Wave luôn hiển thị.
        if (waveText != null)
        {
            waveText.text = "WAVE 1";
            waveText.gameObject.SetActive(true);
        }

        // Boss warning mặc định ẩn.
        if (bossWarning != null)
        {
            bossWarning.SetActive(false);
        }
    }

    public void ShowWave(int wave, bool isBossWave)
    {
        if (waveText == null) return;

        // Chỉ đổi nội dung, không tự ẩn Wave Text.
        waveText.text = $"WAVE {wave}";
        waveText.gameObject.SetActive(true);
    }

    public void ShowBossWarning()
    {
        if (bossWarning == null) return;

        bossWarning.SetActive(true);

        if (bossCoroutine != null)
        {
            StopCoroutine(bossCoroutine);
        }

        bossCoroutine = StartCoroutine(HideBossAfterDelay());
    }

    private IEnumerator HideBossAfterDelay()
    {
        yield return new WaitForSeconds(bossShowDuration);

        if (bossWarning != null)
        {
            bossWarning.SetActive(false);
        }

        bossCoroutine = null;
    }
}
