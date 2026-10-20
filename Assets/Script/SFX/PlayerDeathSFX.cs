
using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class PlayerDeathSFX : MonoBehaviour
{
    [Header("Death Sound")]
    [SerializeField] private AudioClip deathClip;

    [SerializeField, Range(0f, 1f)]
    private float volume = 0.9f;

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

        if (health != null && health.OnDied != null)
            health.OnDied.AddListener(PlayDeathSound);
    }

    private void OnDisable()
    {
        if (health != null && health.OnDied != null)
            health.OnDied.RemoveListener(PlayDeathSound);
    }

    private void PlayDeathSound()
    {
        if (deathClip != null)
            audioSource.PlayOneShot(deathClip, volume);
    }
}
