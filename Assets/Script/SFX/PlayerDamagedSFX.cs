
using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class PlayerDamagedSFX : MonoBehaviour
{
    [Header("Damage Sound")]
    [SerializeField] private AudioClip damageClip;

    [SerializeField, Range(0f, 1f)]
    private float volume = 0.8f;

    private HealthController health;
    private AudioSource audioSource;

    private void Awake()
    {
        health = GetComponent<HealthController>();

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    private void OnEnable()
    {
        if (health == null)
            health = GetComponent<HealthController>();

        if (health != null && health.OnDamaged != null)
            health.OnDamaged.AddListener(PlayDamageSound);
    }

    private void OnDisable()
    {
        if (health != null && health.OnDamaged != null)
            health.OnDamaged.RemoveListener(PlayDamageSound);
    }

    private void PlayDamageSound()
{
    if (health == null || health.RemainingHealthPercentage <= 0f)
        return;

    if (damageClip != null)
        audioSource.PlayOneShot(damageClip, volume);
}
}
