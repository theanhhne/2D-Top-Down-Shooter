using UnityEngine;

public class SpeedBoostCollectableBehaviour : MonoBehaviour, ICollectableBehaviour
{
    [SerializeField]
    private float _speedMultiplier = 1.5f;

    [SerializeField]
    private float _boostDuration = 5f;

    public void OnCollected(GameObject player)
    {
        SpeedBoostController speedBoostController = player.GetComponent<SpeedBoostController>();

        if (speedBoostController == null)
        {
            speedBoostController = player.AddComponent<SpeedBoostController>();
        }

        speedBoostController.StartBoost(_speedMultiplier, _boostDuration);
    }
}
