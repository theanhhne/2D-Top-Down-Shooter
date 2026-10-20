
using UnityEngine;

[RequireComponent(typeof(HealthController))]
public class EnemySFX : MonoBehaviour
{
    [Header("Zombie Sounds")]
    [SerializeField] private AudioClip idleOrMoveClip;
    [SerializeField] private AudioClip damageClip;

    [Header("Distance Sound")]
    [SerializeField] private Transform player;
    [SerializeField] private float maxHearingDistance = 15f;
    [SerializeField, Range(0f, 1f)] private float minVolume = 0.05f;
    [SerializeField, Range(0f, 1f)] private float maxVolume = 0.7f;

    [Header("Timing")]
    [SerializeField] private float soundInterval = 4f;
    [SerializeField, Range(0f, 1f)] private float damageVolume = 0.8f;

    private HealthController health;
    private AudioSource audioSource;
    private float soundTimer;

    private void Awake()
    {
        health = GetComponent<HealthController>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;

        soundTimer = Random.Range(0f, soundInterval);
    }

    private void Start()
    {
        // Tự tìm Player nếu chưa gán trong Inspector
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;
        }
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

    private void Update()
    {
        if (idleOrMoveClip == null || player == null)
            return;

        soundTimer -= Time.deltaTime;

        if (soundTimer <= 0f)
        {
            float distance = Vector2.Distance(
                transform.position,
                player.position
            );

            if (distance <= maxHearingDistance)
            {
                float t = 1f - Mathf.Clamp01(
                    distance / maxHearingDistance
                );

                // Ở gần thì to, ở xa thì nhỏ
                float volume = Mathf.Lerp(minVolume, maxVolume, t);
                audioSource.PlayOneShot(idleOrMoveClip, volume);
            }

            soundTimer = soundInterval;
        }
    }

    private void PlayDamageSound()
    {
        if (damageClip == null)
            return;

        float volume = damageVolume;

        if (player != null)
        {
            float distance = Vector2.Distance(
                transform.position,
                player.position
            );

            float t = 1f - Mathf.Clamp01(
                distance / maxHearingDistance
            );

            volume *= t;
        }

        if (volume > 0.01f)
        {
            AudioSource.PlayClipAtPoint(
                damageClip,
                transform.position,
                volume
            );
        }
    }
}
