
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GunSFX : MonoBehaviour
{
    [Header("Gun Sound")]
    [SerializeField] private AudioClip shootClip;

    [Header("Settings")]
    [SerializeField, Range(0f, 1f)]
    private float volume = 0.7f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    public void PlayShootSound()
    {
        if (shootClip == null)
            return;

        audioSource.PlayOneShot(shootClip, volume);
    }
}
